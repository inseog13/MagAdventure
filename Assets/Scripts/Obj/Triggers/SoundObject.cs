using UnityEngine;

[RequireComponent(typeof(GroupID))]
public class SoundObject : MonoBehaviour
{
    [Header("Sound Settings")]
    [Tooltip("The sound clip to be played when activated.")]
    public AudioClip soundClip;

    [Tooltip("If checked, the sound object will not play if the sound is already playing (optional).")]
    public bool preventOverlap = true;

    private AudioSource audioSource;
    private bool hasTriggered = false; // Single-Active mode tracking

    void Awake()
    {
        // SoundObject가 자체 AudioSource를 가지고 소리를 재생하도록 설정합니다.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Output을 SFX 믹서 그룹으로 설정하는 것이 좋습니다.
        // audioSource.outputAudioMixerGroup = SoundManager.Instance.GetSFXMixerGroup(); 
        // (SoundManager에 GetSFXMixerGroup 같은 함수를 추가했다면 사용 가능)
    }

    // SoundTrigger로부터 명령을 받아 소리를 재생합니다.
    public void ActivateSound(float delayTime, bool isMultiActive)
    {
        // Single-Active 모드 체크
        if (!isMultiActive && hasTriggered) return;

        if (!isMultiActive)
        {
            hasTriggered = true;
        }

        // 딜레이 시간 후 재생 함수 호출
        Invoke("PerformSound", delayTime);
    }

    private void PerformSound()
    {
        if (soundClip == null)
        {
            Debug.LogWarning($"SoundObject on {gameObject.name} is missing an AudioClip.");
            return;
        }

        if (preventOverlap && audioSource.isPlaying)
        {
            // 중복 재생 방지 설정 시 이미 재생 중이면 종료
            return;
        }
        
        // PlayOneShot을 사용하여 현재 재생 중인 다른 소리에 영향을 주지 않고 재생합니다.
        audioSource.PlayOneShot(soundClip);
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}