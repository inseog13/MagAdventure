using UnityEngine;

[RequireComponent(typeof(GroupID))]
public class EffectObject : MonoBehaviour
{
    [Header("Effect Settings")]
    [Tooltip("The ParticleSystem component to play when activated.")]
    public ParticleSystem particleSystem;

    private bool hasTriggered = false; // Single-Active mode tracking

    void Awake()
    {
        // ParticleSystem을 인스펙터에서 할당하지 않았을 경우, 자동으로 찾습니다.
        if (particleSystem == null)
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        if (particleSystem == null)
        {
            Debug.LogError($"EffectObject on {gameObject.name} requires a ParticleSystem component.");
        }
    }

    // EffectTrigger로부터 명령을 받아 이펙트를 실행합니다.
    public void ActivateEffect(float delayTime, bool isMultiActive)
    {
        if (particleSystem == null) return;

        // Single-Active 모드 체크: 이미 발동했으면 무시
        if (!isMultiActive && hasTriggered) return;

        if (!isMultiActive)
        {
            hasTriggered = true;
        }

        // 딜레이 시간 후 재생 함수 호출
        Invoke("PerformEffect", delayTime);
    }

    private void PerformEffect()
    {
        // 파티클 시스템을 재생합니다. (자식 파티클 시스템도 함께 재생)
        particleSystem.Play(true);
    }
    
    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}