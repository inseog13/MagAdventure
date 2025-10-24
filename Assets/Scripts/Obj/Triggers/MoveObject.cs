using UnityEngine;

[RequireComponent(typeof(GroupID))]
public class MoveObject : MonoBehaviour
{
    // EasingMode is defined here but controlled by ActivationTrigger
    public enum EasingMode { None, EaseInOut, ExponentialOut }

    // No need for EasingMode variable here anymore; it comes from the trigger.

    // Internal state variables
    private bool isMoving = false;

    // External activation method
    public void ActivateMovement(Vector3 moveVector, float duration, float delayTime, EasingMode mode)
    {
        // Safety check to prevent immediate re-activation if already moving
        if (isMoving) return; 

        // Start movement logic as a coroutine
        StartCoroutine(PerformMovement(moveVector, duration, delayTime, mode));
    }

    // Coroutine to handle the movement with easing
    private System.Collections.IEnumerator PerformMovement(Vector3 moveVector, float dur, float delay, EasingMode mode)
    {
        isMoving = true;
        yield return new WaitForSeconds(delay);

        float timer = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + moveVector;

        while (timer < dur)
        {
            timer += Time.deltaTime;
            float t = timer / dur;
            
            float easedT = ApplyEasing(t, mode); 
            
            transform.position = Vector3.Lerp(startPos, endPos, easedT);
            yield return null;
        }
        transform.position = endPos;
        isMoving = false;
    }

    // Easing Logic
    private float ApplyEasing(float t, EasingMode mode)
    {
        t = Mathf.Clamp01(t);

        switch (mode)
        {
            case EasingMode.None: return t;
            case EasingMode.EaseInOut: return Mathf.SmoothStep(0f, 1f, t); 
            case EasingMode.ExponentialOut: return (t == 1f) ? 1f : 1f - Mathf.Pow(2f, -10f * t);

            default: return t;
        }
    }
}