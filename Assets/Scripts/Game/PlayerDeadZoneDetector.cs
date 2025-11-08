using UnityEngine;

public class PlayerDeadZoneDetector : MonoBehaviour
{
    private GameTimeManager timeManager;

    void Start()
    {
        // 씬 관리자 찾기
        timeManager = FindObjectOfType<GameTimeManager>();
        // ... (생략)
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ... (DeadZone 레이어 확인 로직)
        if (((1 << other.gameObject.layer) & timeManager.deadZoneLayer) != 0)
        {
            // GameTimeManager의 공용 함수 호출
            timeManager.RevertToLastCheckpoint();
        }
    }
}