using UnityEngine;

// ⚠️ 이 오브젝트는 'Is Trigger'가 체크된 Collider2D를 가지고 있어야 합니다.

public class Checkpoint : MonoBehaviour
{
    private GameTimeManager timeManager;
    
    // 이 체크포인트가 이미 활성화(저장)되었는지 확인하는 플래그
    private bool isActivated = false;

    void Start()
    {
        // 씬 관리자 찾기
        timeManager = FindObjectOfType<GameTimeManager>();
        if (timeManager == null)
        {
            Debug.LogError("Checkpoint: GameTimeManager 스크립트가 씬에 없습니다. 체크포인트 기능이 작동하지 않습니다.");
        }
    }
    
    // 플레이어가 충돌했을 때 (트리거)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 오브젝트의 태그가 "Player"인지 확인
        // 이미 활성화된 체크포인트는 다시 저장하지 않도록 합니다. (선택 사항)
        if (other.CompareTag("Player") && !isActivated)
        {
            // 체크포인트 저장 함수 호출
            if (timeManager != null)
            {
                timeManager.SetCheckpoint(transform.position); 
                isActivated = true; // 활성화 완료
                
                // (선택 사항) 체크포인트 활성화 시 시각적 피드백 제공
                // 예: 크리스탈 색깔 변경 또는 파티클 효과 재생
            }
        }
    }
}