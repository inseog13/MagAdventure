using UnityEngine;

public class TimelineTrigger : MonoBehaviour
{
    [Header("Target Group Settings")]
    [Tooltip("The ID of the TimelineObject group this trigger will activate.")]
    public int targetGroupID = 1;

    [Header("Trigger Activation")]
    [Tooltip("If checked, the trigger can be activated multiple times (Multi-Active).")]
    public bool isMultiActive = false;

    [Tooltip("Delay before the timeline starts.")]
    public float delayTime = 0f;

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

            // 씬에서 모든 TimelineObject를 찾아봅니다.
            TimelineObject[] timelineObjects = FindObjectsByType<TimelineObject>(FindObjectsSortMode.None);

            foreach (TimelineObject timelineObject in timelineObjects)
            {
                GroupID groupID = timelineObject.GetComponent<GroupID>();
                
                // 그룹 ID가 일치하는지 확인
                if (groupID != null && groupID.groupID == targetGroupID)
                {
                    // 타임라인 재생 명령 전달
                    timelineObject.ActivateTimeline(delayTime, isMultiActive);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isMultiActive)
        {
            // Multi-Active 모드에서만 나갔을 때 리셋
            hasTriggered = false;
        }
    }
}