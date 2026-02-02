using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private float animDuration = 0.1f;
    [SerializeField] private float pressAngle = 5f;
    [SerializeField] private Piano piano;
    [SerializeField] private AudioClip audioClip;
    private bool isPressed = false;
    private float currentAngle = 0f;
    private int tweenId = -1;

    public void Press()
    {
        if (isPressed)
            return;

        isPressed = true;
        UpdateState();
    }

    public void Release()
    {
        if (isPressed)
        {
            isPressed = false;
            UpdateState();
        }
    }

    void UpdateState()
    {
        float speed = pressAngle / animDuration;
        float newDistance = isPressed ? pressAngle - currentAngle : currentAngle;
        float newAnimDuration = newDistance / speed;

        if (LeanTween.isTweening(tweenId))
            LeanTween.cancel(tweenId);

        if (isPressed)
            piano.PlaySound(audioClip);

        tweenId = LeanTween
        .value(gameObject, currentAngle, isPressed ? pressAngle : 0f, newAnimDuration)
        .setOnUpdate(val => {
                currentAngle = val;
                transform.localRotation = Quaternion.Euler(0f, -currentAngle, 0f);
            }
        ).id;
    }
}
