using UnityEngine;

public class ParallaxScrolling : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Tooltip("카메라 이동에 대한 배경의 상대 속도 (0에 가까울수록 느림)")]
    [Range(0f, 1f)] // 1을 넘어가면 배경이 카메라보다 빠르게 움직임
    public float parallaxEffect = 0.5f;

    [Tooltip("배경 스프라이트의 실제 월드 유닛 너비")]
    public float backgroundWidth = 8.0f; // ★ 사용자 설정값 8.0

    [Header("References")]
    public Transform cameraTransform;

    // 카메라의 '초기 X 위치'
    private float cameraStartX;

    // 배경의 '초기 X 위치'
    private float backgroundStartX;

    void Start()
    {
        if (cameraTransform == null || backgroundWidth <= 0)
        {
            Debug.LogError("필수 설정이 누락되었습니다. 카메라 Transform과 배경 너비를 확인하세요.");
            enabled = false;
            return;
        }

        // 1. 초기 기준점 저장
        // 카메라가 움직이기 시작하는 기준 위치를 저장합니다.
        cameraStartX = cameraTransform.position.x;
        
        // 배경의 시작 위치를 저장합니다. 이 위치를 기준으로 배경이 루프됩니다.
        backgroundStartX = transform.position.x;
    }

    void LateUpdate()
    {
        // 1. 카메라 이동 거리 계산
        // 카메라가 시작점으로부터 얼마나 멀리 이동했는지 계산합니다.
        float cameraDisplacement = cameraTransform.position.x - cameraStartX;

        // 2. 패럴랙스 이동량 계산 (배경이 움직여야 하는 거리)
        // 배경은 카메라 이동량에 'parallaxEffect'를 곱한 만큼만 움직입니다.
        // 이 거리를 기준으로 루프를 계산합니다.
        float parallaxMovement = cameraDisplacement * parallaxEffect;

        // 3. 무한 루프 오프셋 계산 (Modulo 연산)
        // 배경의 총 이동량(패럴랙스 이동량)이 배경 너비를 몇 번 넘었는지 계산하여 오프셋을 만듭니다.
        // % 연산은 C#에서 음수가 될 수 있으므로, 루프 위치를 backgroundWidth 내로 제한합니다.
        float loopOffset = parallaxMovement % backgroundWidth;
        
        // C# Modulo 문제 해결:
        if (loopOffset < 0)
        {
            loopOffset += backgroundWidth;
        }

        // 4. 최종 위치 설정
        // New X = (배경 시작 X) + (카메라 이동량) - (패럴랙스 이동량) - (루프 오프셋)
        // 최종 X = (배경 시작 X) + (카메라가 움직인 거리) - (루프 오프셋)
        
        // [핵심 공식] 최종 위치 = (카메라가 이동한 만큼의 현재 위치) - (루프 오프셋)
        transform.position = new Vector3(
            backgroundStartX + cameraDisplacement - loopOffset, // ★ 배경의 최종 X 위치
            transform.position.y,
            transform.position.z
        );
    }
}