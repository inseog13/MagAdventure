using UnityEngine;

public class EffectTrigger : MonoBehaviour
{
    [Header("Target Group Settings")]
    [Tooltip("The ID of the EffectObject group this trigger will activate.")]
    public int targetGroupID = 1;

    [Header("Trigger Activation")]
    [Tooltip("If checked, the trigger can be activated multiple times (Multi-Active).")]
    public bool isMultiActive = false;

    [Tooltip("Delay before the effect starts.")]
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

            // 씬에서 모든 EffectObject를 찾아봅니다.
            EffectObject[] effectObjects = FindObjectsByType<EffectObject>(FindObjectsSortMode.None);

            foreach (EffectObject effectObject in effectObjects)
            {
                GroupID groupID = effectObject.GetComponent<GroupID>();
                
                // 그룹 ID가 일치하는지 확인
                if (groupID != null && groupID.groupID == targetGroupID)
                {
                    // 이펙트 실행 명령 전달
                    effectObject.ActivateEffect(delayTime, isMultiActive);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isMultiActive)
        {
            // Multi-Active 모드에서만 리셋
            hasTriggered = false;
        }
    }
}