using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    // Action Map Name Reference
    const string ACTION_MAP_NAME = "Player";

    // Action Name References
    const string MOVEMENT = "Movement";
    const string ROTATION = "Rotation";
    const string JUMP = "Jump";
    const string CROUCH = "Crouch";

    // Player Props
    const float VERTICAL_VIEW_RANGE = 90f;
    const float DOUBLE_TAP_DURATION = 0.25f;

    [Header("Input Action Asset Reference")]
    [SerializeField] private InputActionAsset actionAsset;

    [Header("Player Component References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private GameObject head;

    [Header("Player Properties")]
    [SerializeField] private float baseSpeed = 7f;
    [SerializeField] private float crouchingSpeedMultiplier = 1 / 3;
    [SerializeField] private float sprintingSpeedMultiplier = 2;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravityMultiplier = 1f;
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float crouchingDuration = 0.1f;
    
    [Header("Camera Heights")]
    [SerializeField] private float standingViewHeight = 0.8f;
    [SerializeField] private float crouchingViewHeight = 0f;

    // Action References
    private InputAction movementAction;
    private InputAction rotationAction;
    private InputAction jumpAction;
    private InputAction crouchAction;

    // Input Props
    private Vector2 movementInput;
    private Vector2 rotationInput;
    private bool jumpTriggered;
    private bool crouchTriggered;

    // Player Props
    private Vector3 movement;
    // private float pitch;
    private float yaw;
    private int crouchTweenId = -1;
    private bool isCrouching;
    
    // Sprinting Handler
    private bool isSprinting;
    private bool isForwardHeld;
    private bool waitForSecondForwardTap;
    private float lastForwardTapTime;

    private void Awake()
    {
        InputActionMap actionMap = actionAsset.FindActionMap(ACTION_MAP_NAME);

        movementAction = actionMap.FindAction(MOVEMENT);
        rotationAction = actionMap.FindAction(ROTATION);
        jumpAction = actionMap.FindAction(JUMP);
        crouchAction = actionMap.FindAction(CROUCH);

        movementAction.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        movementAction.canceled += _ => movementInput = Vector2.zero;

        rotationAction.performed += ctx => rotationInput = ctx.ReadValue<Vector2>();
        rotationAction.canceled += _ => rotationInput = Vector2.zero;

        jumpAction.performed += _ => jumpTriggered = true;
        jumpAction.canceled += _ => jumpTriggered = false;

        crouchAction.performed += _ => crouchTriggered = true;
        crouchAction.canceled += _ => crouchTriggered = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleCrouch();
    }

    private void HandleSprint()
    {
        if (movementInput.y > 0f && !isForwardHeld)
        {
            float now = Time.time;
            
            isForwardHeld = true;

            if (waitForSecondForwardTap && now - lastForwardTapTime <= DOUBLE_TAP_DURATION)
            {
                isSprinting = true;
                waitForSecondForwardTap = false;
                lastForwardTapTime = 0f;
            }
            else
            {
                waitForSecondForwardTap = true;
                lastForwardTapTime = now;
            }
        }
        else if (movementInput.y == 0f)
        {
            isForwardHeld = false;

            if (isSprinting)
                isSprinting = false;
        }
    }

    private void HandleMovement()
    {
        HandleSprint();

        // print(movementInput);
        Vector3 localDirection = new(movementInput.x, 0f, movementInput.y);
        Vector3 worldDirection = transform.TransformDirection(localDirection).normalized;
        movement.x = worldDirection.x * GetSpeed();
        movement.z = worldDirection.z * GetSpeed();

        HandleJump();
        characterController.Move(movement * Time.deltaTime);
    }

    private float GetSpeed()
    {
        if (isCrouching)
        {
            return baseSpeed * crouchingSpeedMultiplier;
        }
        else if (isSprinting)
        {
            return baseSpeed * sprintingSpeedMultiplier;
        }
        else
        {
            return baseSpeed;
        }
    }

    private void HandleJump()
    {
        if (jumpTriggered && characterController.isGrounded)
        {
            movement.y = jumpForce;
        }
        movement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
    }

    private void HandleCrouch()
    {
        float distance = standingViewHeight - crouchingViewHeight;
        float time = crouchingDuration;
        float speed = distance / time;

        if (crouchTriggered && !isCrouching)
        {
            isCrouching = true;

            if (isSprinting)
                isSprinting = false;

            if (LeanTween.isTweening(crouchTweenId))
                LeanTween.cancel(crouchTweenId);

            distance = head.transform.localPosition.y;
            float newCrouchingDuration = distance / speed;
            crouchTweenId = LeanTween.moveLocalY(head.transform.gameObject, crouchingViewHeight, newCrouchingDuration).id;
        }
        else if (!crouchTriggered && isCrouching)
        {
            isCrouching = false;

            if (LeanTween.isTweening(crouchTweenId))
                LeanTween.cancel(crouchTweenId);

            distance = standingViewHeight - head.transform.localPosition.y;
            float newCrouchingDuration = distance / speed;
            crouchTweenId = LeanTween.moveLocalY(head.transform.gameObject, standingViewHeight, newCrouchingDuration).id;
        }
    }

    private void HandleRotation()
    {
        yaw += rotationInput.x * mouseSensitivity;
        // pitch = Mathf.Clamp(pitch - rotationInput.y * mouseSensitivity, -VERTICAL_VIEW_RANGE, VERTICAL_VIEW_RANGE);

        transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        // head.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void OnEnable()
    {
        actionAsset.FindActionMap(ACTION_MAP_NAME).Enable();
    }

    private void OnDisable()
    {
        actionAsset.FindActionMap(ACTION_MAP_NAME).Disable();
    }
}