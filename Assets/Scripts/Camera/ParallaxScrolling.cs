using UnityEngine;

public class ParallaxScrolling : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Tooltip("카메라 이동에 대한 배경의 상대 속도 (0에 가까울수록 느림)")]
    [Range(0f, 2f)]
    public float parallaxEffect = 0.5f;

    [Tooltip("배경 스프라이트의 실제 월드 유닛 너비 (Width 256, PPU 64이므로 4.0)")]
    public float backgroundWidth = 4.0f; // ★ 4.0으로 설정 확인

    [Header("References")]
    public Transform cameraTransform;

    // 카메라에 대한 배경의 초기 상대 위치 오프셋
    private float initialOffsetFromCamera;

    void Start()
    {
        if (cameraTransform == null || backgroundWidth <= 0)
        {
            Debug.LogError("필수 설정이 누락되었습니다.");
            enabled = false;
            return;
        }

        // 1. 배경과 카메라의 초기 상대 거리를 저장합니다.
        // 이 거리가 패럴랙스 효과를 통해 유지되어야 합니다.
        initialOffsetFromCamera = transform.position.x - cameraTransform.position.x;
    }

    void LateUpdate()
    {
        // 1. 패럴랙스 기준점 계산
        // 카메라가 이동한 거리(Camera.x - Camera.initial.x)를
        // 패럴랙스 효과를 적용하여 배경이 얼마나 '밀려나야' 하는지 계산합니다.
        // (1 - parallaxEffect)를 곱하는 것이 핵심입니다.
        // parallaxEffect가 0.5이면, 카메라는 10 이동할 때 배경은 5만큼 덜 움직여야 합니다.
        float parallaxMovementX = cameraTransform.position.x * (1 - parallaxEffect);

        // 2. 무한 루프 오프셋 계산 (Modulo 연산으로 루프 사이클을 만듭니다)
        // C#의 % 연산은 음수 결과가 나올 수 있으므로, 정확한 루프를 위해 양수 결과를 보장합니다.
        // 배경이 루프 너비(4.0)를 넘어갔을 때, 얼마나 되돌려야 하는지 계산합니다.
        float loopOffset = parallaxMovementX % backgroundWidth;
        
        // C# Modulo 문제 해결:
        if (loopOffset < 0)
        {
            loopOffset += backgroundWidth;
        }

        // 3. 최종 위치 설정
        // New X = (카메라 X) + (카메라와의 초기 오프셋) - (패럴랙스 이동량)
        // 위에서 계산한 loopOffset을 카메라 X 위치에 더하거나 빼서 배경을 고정시킵니다.
        
        // [수정된 로직] 카메라의 X 위치에 루프 오프셋을 적용하고, 
        // 패럴랙스 효과를 적용하여 최종 위치를 설정합니다.
        transform.position = new Vector3(
            cameraTransform.position.x + initialOffsetFromCamera - loopOffset,
            transform.position.y,
            transform.position.z
        );
    }
}