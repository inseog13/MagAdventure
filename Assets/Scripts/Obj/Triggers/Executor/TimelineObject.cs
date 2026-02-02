using UnityEngine;
using UnityEngine.Playables; // PlayableDirector를 사용하기 위해 필수

[RequireComponent(typeof(GroupID))]
[RequireComponent(typeof(PlayableDirector))]
public class TimelineObject : MonoBehaviour
{
    [Header("Timeline Settings")]
    [Tooltip("The PlayableDirector component that controls the Timeline Asset.")]
    public PlayableDirector playableDirector;

    private bool hasTriggered = false; // Single-Active mode tracking

    void Awake()
    {
        // PlayableDirector가 할당되지 않은 경우, 자동으로 GetComponent로 가져옵니다.
        if (playableDirector == null)
        {
            playableDirector = GetComponent<PlayableDirector>();
        }

        if (playableDirector == null)
        {
            Debug.LogError($"TimelineObject on {gameObject.name} requires a PlayableDirector component.");
        }
    }

    // TimelineTrigger로부터 명령을 받아 타임라인을 실행합니다.
    public void ActivateTimeline(float delayTime, bool isMultiActive)
    {
        if (playableDirector == null) return;

        // Single-Active 모드 체크: 이미 발동했으면 무시
        if (!isMultiActive && hasTriggered) return;

        if (!isMultiActive)
        {
            hasTriggered = true;
        }

        // 딜레이 시간 후 재생 함수 호출
        Invoke("PerformTimelinePlay", delayTime);
    }

    private void PerformTimelinePlay()
    {
        // 타임라인을 멈추고 재생합니다. (멈추지 않으면 현재 위치부터 재생될 수 있습니다.)
        if (playableDirector.state == PlayState.Paused || playableDirector.state == PlayState.Playing)
        {
            playableDirector.Stop();
        }
        
        playableDirector.Play();
    }
    
    // Multi-Active 모드를 위한 리셋 기능 (OnTriggerExit2D에서 사용 가능)
    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}