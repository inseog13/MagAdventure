using UnityEngine;

// ⚠️ 가장 중요한 설정:
// 'DeadZone' 레이어와 'Player' Transform을 인스펙터에 할당해야 합니다.

public class GameTimeManager : MonoBehaviour
{
    // 씬 시작 후 경과된 시간을 추적하는 커스텀 시간 변수
    public static float ElapsedTime = 0f;

    // 마지막 체크포인트 시간과 위치를 저장하는 변수
    private float lastCheckpointTime = 0f;
    private Vector3 lastCheckpointPosition;

    // 인스펙터에서 플레이어 오브젝트를 할당하세요.
    public Transform playerTransform; 
    
    // 인스펙터에서 DeadZone 레이어를 설정합니다.
    [Tooltip("DeadZone 레이어를 선택해주세요.")]
    public LayerMask deadZoneLayer; 

    void Awake()
    {
        // 씬 시작 시 플레이어 Transform이 할당되지 않았다면 'Player' 태그로 찾습니다.
        if (playerTransform == null)
        {
            GameObject playerGO = GameObject.FindWithTag("Player");
            if (playerGO != null)
            {
                playerTransform = playerGO.transform;
            }
        }
        
        // 플레이어 위치를 초기 스폰 지점으로 설정합니다.
        if (playerTransform != null)
        {
            lastCheckpointPosition = playerTransform.position;
        }
        
        // 씬 로드 시 시간을 0으로 초기화합니다.
        ElapsedTime = 0f;
        lastCheckpointTime = 0f;
    }

    // 매 프레임마다 커스텀 시간을 증가시킵니다.
    void Update()
    {
        ElapsedTime += Time.deltaTime; 
        // Debug.Log($"현재 시간: {ElapsedTime:F2}"); // 디버깅용
    }

    // 🚩 크리스탈 충돌 시 호출되어 상태를 저장합니다.
    public void SetCheckpoint(Vector3 crystalPosition)
    {
        lastCheckpointTime = ElapsedTime;
        
        // 크리스탈이 아닌, 그 순간의 플레이어 위치를 저장하는 것이 일반적입니다.
        if (playerTransform != null)
        {
            lastCheckpointPosition = playerTransform.position; 
        }
        
        Debug.Log($"체크포인트 저장 완료! (시간: {lastCheckpointTime:F2}초)");
    }

    // 💀 DeadZone 충돌 시 호출되어 시간을 되돌립니다.
    public void RevertToLastCheckpoint()
    {
        Debug.Log($"DeadZone 충돌 감지! 시간을 {lastCheckpointTime:F2}초로 되돌립니다.");

        // 1. 플레이어 위치 되돌리기
        if (playerTransform != null)
        {
            // Rigidbody가 있다면 속도를 리셋하여 텔레포트 후 잔여 움직임을 방지합니다.
            Rigidbody2D rb = playerTransform.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
            playerTransform.position = lastCheckpointPosition;
        }
        
        // 2. 커스텀 시간 되돌리기
        ElapsedTime = lastCheckpointTime;

        // 추가적인 사망 효과(애니메이션, 사운드 등)는 여기에 구현할 수 있습니다.
    }
}