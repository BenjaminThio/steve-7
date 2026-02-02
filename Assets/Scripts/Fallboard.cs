using UnityEngine;

public class Fallboard : MonoBehaviour
{
    [SerializeField] private float animDuration = 0.3f;
    [SerializeField] private float maxOpenAngle = 105f;
    private bool isOpen = false;
    private int tweenId = -1;

    public void ToggleState()
    {
        isOpen = !isOpen;

        float speed = maxOpenAngle / animDuration;
        float newDistance = isOpen ? maxOpenAngle - transform.localEulerAngles.y : transform.localEulerAngles.y;
        float newAnimDuration = newDistance / speed;

        if (LeanTween.isTweening(tweenId))
            LeanTween.cancel(tweenId);

        tweenId = LeanTween.rotateLocal(transform.gameObject, isOpen ? new(0f, maxOpenAngle, 0f) : Vector3.zero, newAnimDuration).id;
    }
}
