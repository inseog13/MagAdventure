using UnityEngine;

[RequireComponent(typeof(GroupID))]
public class MoveObject : MonoBehaviour
{
    // EasingMode는 MoveObject가 아닌 ActivationTrigger에서 정의되어야 합니다.
    public enum EasingMode { None, EaseInOut, ExponentialOut }

    private bool isMoving = false;
    private bool isRotating = false; // 👈 회전 상태 추적

    // 외부(ActivationTrigger)로부터 이동 정보를 받아 실행합니다.
    public void ActivateMovement(Vector3 moveVector, float duration, float delayTime, EasingMode mode)
    {
        if (isMoving) return; 
        StartCoroutine(PerformMovement(moveVector, duration, delayTime, mode));
    }
    
    // 👈 외부(ActivationTrigger)로부터 회전 정보를 받아 실행합니다.
    public void ActivateRotation(Vector3 rotationVector, float duration, float delayTime, EasingMode mode)
    {
        if (isRotating) return;
        StartCoroutine(PerformRotation(rotationVector, duration, delayTime, mode));
    }

    // --- 코루틴 구현 ---

    // 이동 코루틴 (기존과 동일)
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

    // 👈 회전 코루틴 (새로 추가)
    private System.Collections.IEnumerator PerformRotation(Vector3 rotationVector, float dur, float delay, EasingMode mode)
    {
        isRotating = true;
        yield return new WaitForSeconds(delay);

        float timer = 0f;
        Quaternion startRot = transform.rotation;
        // 오일러 각(Vector3)을 쿼터니언으로 변환하여 목표 회전을 설정합니다.
        Quaternion endRot = transform.rotation * Quaternion.Euler(rotationVector); 

        while (timer < dur)
        {
            timer += Time.deltaTime;
            float t = timer / dur;
            float easedT = ApplyEasing(t, mode); 
            
            // Quaternion.Slerp를 사용하여 구면 선형 보간으로 부드럽게 회전합니다.
            transform.rotation = Quaternion.Slerp(startRot, endRot, easedT);
            yield return null;
        }
        transform.rotation = endRot;
        isRotating = false;
    }

    // Easing Logic (기존과 동일)
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