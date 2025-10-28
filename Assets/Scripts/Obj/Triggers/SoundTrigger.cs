using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    [Header("Target Group Settings")]
    [Tooltip("The ID of the SoundObject group this trigger will activate.")]
    public int targetGroupID = 1;

    [Header("Trigger Activation")]
    [Tooltip("If checked, the trigger can be activated multiple times (Multi-Active).")]
    public bool isMultiActive = false;

    [Tooltip("Delay before the sound starts playing.")]
    public float delayTime = 0f;

    // Single-Active mode tracking
    private bool hasTriggered = false; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Single-Active 모드 체크
            if (!isMultiActive && hasTriggered) return;

            if (!isMultiActive)
            {
                hasTriggered = true;
            }

            // 씬에서 모든 SoundObject를 찾아봅니다.
            SoundObject[] soundObjects = FindObjectsByType<SoundObject>(FindObjectsSortMode.None);

            foreach (SoundObject soundObject in soundObjects)
            {
                GroupID groupID = soundObject.GetComponent<GroupID>();
                
                // 그룹 ID가 일치하는지 확인
                if (groupID != null && groupID.groupID == targetGroupID)
                {
                    // 소리 재생 명령 전달
                    soundObject.ActivateSound(delayTime, isMultiActive);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isMultiActive)
        {
            // Multi-Active 모드에서만 나갔을 때 리셋하여 재진입 시 다시 발동 가능
            hasTriggered = false;
            
            // 만약 나갔을 때 소리 오브젝트 자체도 리셋해야 한다면 (예: 루프를 멈추고 리셋)
            // 아래 코드를 사용할 수 있습니다.
            /*
            SoundObject[] soundObjects = FindObjectsByType<SoundObject>(FindObjectsSortMode.None);
            foreach (SoundObject soundObject in soundObjects)
            {
                GroupID groupID = soundObject.GetComponent<GroupID>();
                if (groupID != null && groupID.groupID == targetGroupID)
                {
                    soundObject.ResetTrigger();
                }
            }
            */
        }
    }
}