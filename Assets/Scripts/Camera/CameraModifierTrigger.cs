using UnityEngine;
using System;
using System.Reflection;

// Inspector에서 설정값을 그룹화하여 쉽게 편집할 수 있도록 합니다.
[Serializable]
public class CameraSettings
{
    [Tooltip("변경할 Orthographic Size (줌 레벨)")]
    public float orthographicSize = 6f;

    [Tooltip("변경할 Follow Offset (X, Y, Z)")]
    public Vector3 followOffset = new Vector3(0f, 0f, -10f);

    [Tooltip("Cinemachine Position Composer의 X Damping 값")]
    public float xDamping = 1f;

    [Tooltip("Cinemachine Position Composer의 Y Damping 값")]
    public float yDamping = 1f;

    // 참고: 화면 흔들림(Noise)은 보통 Cinemachine Impulse Source 컴포넌트를 분리하여 사용하는 것이 효율적입니다.
}

public class CameraModifierTrigger : MonoBehaviour
{
    [Header("Cinemachine Target & Settings")]
    [Tooltip("Cinemachine Camera 컴포넌트가 붙어있는 '2D camera' 오브젝트를 할당하세요.")]
    [SerializeField] private GameObject targetCameraObject;

    [Tooltip("이 트리거에 진입했을 때 적용될 고유 카메라 설정")]
    [SerializeField] private CameraSettings newSettings;

    // Cinemachine 컴포넌트 참조 변수
    private Component cmCamera;         // Cinemachine Camera
    private Component cmComposer;       // Cinemachine Position Composer (Framing Transposer 역할)
    
    // 기본 설정값 저장용 변수
    private CameraSettings defaultSettings = new CameraSettings();

    private void Start()
    {
        if (targetCameraObject == null)
        {
            Debug.LogError("타겟 카메라 오브젝트가 할당되지 않았습니다. 스크립트 비활성화.");
            enabled = false;
            return;
        }

        // 컴포넌트 이름을 string으로 참조합니다. (사용자 환경의 커스텀 네이밍 가정)
        cmCamera = targetCameraObject.GetComponent("CinemachineCamera");
        cmComposer = targetCameraObject.GetComponent("CinemachinePositionComposer");

        if (cmCamera == null || cmComposer == null)
        {
            Debug.LogError("필요한 Cinemachine 컴포넌트(CinemachineCamera 또는 CinemachinePositionComposer)를 찾을 수 없습니다. 스크립트 비활성화.");
            enabled = false;
            return;
        }
        
        // **[Multi-Active 핵심]** 시작 시점의 카메라 기본값을 저장합니다.
        // 플레이어가 모든 트리거 밖으로 나갔을 때 돌아갈 '원래 상태'입니다.
        CaptureDefaultSettings();
    }

    // 현재 카메라 설정을 defaultSettings에 저장합니다.
    // --- Start() 메서드 안에 있는 CaptureDefaultSettings() 메서드를 아래와 같이 수정해야 합니다. ---

    private void CaptureDefaultSettings()
    {
        // **주의: 이 부분은 GetValue를 사용하여 현재 값을 읽어오도록 수정해야 합니다.**
        try
        {
            // 1. Orthographic Size (줌) 저장 (SetOrthoSize의 코드를 역으로 사용)
            FieldInfo lensField = cmCamera.GetType().GetField("m_Lens", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            object lens = lensField.GetValue(cmCamera);
            FieldInfo sizeField = lens.GetType().GetField("m_OrthographicSize", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            defaultSettings.orthographicSize = (float)sizeField.GetValue(lens);

            // 2. Follow Offset 저장
            FieldInfo offsetField = cmComposer.GetType().GetField("m_FollowOffset", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            defaultSettings.followOffset = (Vector3)offsetField.GetValue(cmComposer);

            // 3. Damping 값 저장
            FieldInfo xDampField = cmComposer.GetType().GetField("m_XDamping", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            FieldInfo yDampField = cmComposer.GetType().GetField("m_YDamping", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            defaultSettings.xDamping = (float)xDampField.GetValue(cmComposer);
            defaultSettings.yDamping = (float)yDampField.GetValue(cmComposer);

            Debug.Log("카메라 기본 설정 저장 완료.");
        }
        catch (Exception e)
        {
            Debug.LogError($"[CaptureDefaultSettings] 기본값 읽기 오류: {e.Message}. 기본값 복원이 작동하지 않을 수 있습니다.");
        }
    }

    // 플레이어가 트리거 박스에 진입했을 때 호출
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && cmCamera != null)
        {
            // 이 트리거만의 고유 설정 적용
            ApplySettings(newSettings);
            
            // 화면 흔들림 효과가 필요하다면 이 지점에서 Cinemachine Impulse Source를 활성화하거나 Trigger합니다.
            
            Debug.Log($"카메라 설정 변경 적용: {gameObject.name}의 고유 설정");
        }
    }

    // 플레이어가 트리거 박스에서 벗어났을 때 호출
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && cmCamera != null)
        {
            // **[Multi-Active 핵심]** 이전에 저장된 기본 설정으로 복원
            ApplySettings(defaultSettings);

            Debug.Log("카메라 설정 기본값으로 복원 완료.");
        }
    }
    
    // 설정 적용을 위한 래퍼(Wrapper) 메서드
    private void ApplySettings(CameraSettings settings)
    {
        // 1. Orthographic Size (줌) 변경
        SetOrthoSize(settings.orthographicSize);
        
        // 2. Follow Offset 변경
        SetFollowOffset(settings.followOffset);
        
        // 3. Damping 값 변경
        SetDamping(settings.xDamping, settings.yDamping);

        // 4. Noise 변경 (선택 사항 - 별도 컴포넌트로 구현)
    }

    // --- 카메라 컴포넌트 속성 접근 Placeholder ---

    private void SetOrthoSize(float size)
    {
        // 'Cinemachine Camera' 컴포넌트 내의 'm_Lens' 구조체의 'm_OrthographicSize' 필드를 찾습니다.
        try
        {
            // 1. m_Lens 필드를 가져옵니다.
            FieldInfo lensField = cmCamera.GetType().GetField("m_Lens", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (lensField == null) throw new MissingFieldException("m_Lens 필드를 찾을 수 없습니다.");

            object lens = lensField.GetValue(cmCamera);
            if (lens == null) throw new NullReferenceException("m_Lens 값이 null입니다.");

            // 2. m_OrthographicSize 필드를 가져옵니다.
            FieldInfo sizeField = lens.GetType().GetField("m_OrthographicSize", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (sizeField == null) throw new MissingFieldException("m_OrthographicSize 필드를 찾을 수 없습니다.");

            // 3. 값 변경
            sizeField.SetValue(lens, size);
            lensField.SetValue(cmCamera, lens); // Lens 구조체가 값 타입일 경우 변경 사항을 다시 할당해야 합니다.

            // Debug.Log($"[SetOrthoSize] 줌을 {size}로 변경 성공.");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SetOrthoSize] CinemachineCamera 내부 접근 오류: {e.Message}");
        }
    }
    
    private void SetFollowOffset(Vector3 offset)
    {
        // 'Cinemachine Position Composer' 컴포넌트 내의 'm_FollowOffset' 필드를 찾습니다.
        try
        {
            FieldInfo offsetField = cmComposer.GetType().GetField("m_FollowOffset", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (offsetField == null) throw new MissingFieldException("m_FollowOffset 필드를 찾을 수 없습니다. 이름이 다를 수 있습니다.");

            // 값 변경
            offsetField.SetValue(cmComposer, offset);
            // Debug.Log($"[SetFollowOffset] 오프셋을 {offset}로 변경 성공.");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SetFollowOffset] CinemachinePositionComposer 내부 접근 오류: {e.Message}");
        }
    }
    
    private void SetDamping(float xDamp, float yDamp)
    {
        // 'Cinemachine Position Composer' 컴포넌트 내의 'm_XDamping', 'm_YDamping' 필드를 찾습니다.
        try
        {
            FieldInfo xDampField = cmComposer.GetType().GetField("m_XDamping", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            FieldInfo yDampField = cmComposer.GetType().GetField("m_YDamping", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (xDampField == null || yDampField == null) throw new MissingFieldException("X/Y Damping 필드를 찾을 수 없습니다. 이름이 다를 수 있습니다.");

            // 값 변경
            xDampField.SetValue(cmComposer, xDamp);
            yDampField.SetValue(cmComposer, yDamp);
            // Debug.Log($"[SetDamping] X/Y 댐핑을 {xDamp}/{yDamp}로 변경 성공.");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SetDamping] CinemachinePositionComposer 내부 접근 오류: {e.Message}");
        }
    }
}