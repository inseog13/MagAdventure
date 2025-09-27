using UnityEngine;

public class CameraFollowX : MonoBehaviour
{
    [SerializeField] private Transform player; // 플레이어 Transform
    [SerializeField] private float smoothSpeed = 0.125f; // 카메라 이동 부드러움
    [SerializeField] private Vector3 offset; // 플레이어와 카메라 간의 거리

    private void LateUpdate()
    {
        if (player == null) return;

        // 목표 위치: X는 플레이어 위치 + offset, Y/Z는 현재 카메라 유지
        Vector3 targetPosition = new Vector3(
            player.position.x + offset.x,
            transform.position.y, 
            transform.position.z
        );

        // 부드럽게 카메라 이동
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }
}
