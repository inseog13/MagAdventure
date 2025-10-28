using UnityEngine;

public class CameraFollowX : MonoBehaviour
{
    // smoothSpeed는 이제 '속도'를 의미하게 되며, 일반적으로 2~5 정도의 값을 사용합니다.
    [SerializeField] private Transform player; 
    [SerializeField] private float smoothSpeed = 5f; // ★ 값을 5.0f 정도로 늘립니다.
    [SerializeField] private Vector3 offset; 

    private void LateUpdate()
    {
        if (player == null) return;

        // 목표 위치 계산 (X축만 플레이어 따라 이동)
        Vector3 targetPosition = new Vector3(
            player.position.x + offset.x,
            transform.position.y, 
            transform.position.z
        );

        // ★ 수정된 보간: Time.deltaTime을 사용하여 프레임 독립적인 이동 속도를 만듭니다.
        // Lerp의 세 번째 인자는 0.0 ~ 1.0 사이의 값이지만, Time.deltaTime을 곱하면
        // 매 프레임마다 남은 거리에 비례하여 부드러운 감속 이동이 됩니다.
        transform.position = Vector3.Lerp(
            transform.position, 
            targetPosition, 
            smoothSpeed * Time.deltaTime
        );
    }
}