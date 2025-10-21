using UnityEngine;
using UnityEngine.Audio; // AudioMixer 사용을 위해 필요

public class SoundManager : MonoBehaviour
{
    // 씬이 바뀌어도 유지되는 싱글톤 패턴
    public static SoundManager Instance;

    [Header("Audio Mixer Setting")]
    [SerializeField] private AudioMixer audioMixer;
    // BGM과 SFX 재생을 위한 AudioSource
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    // --- Audio Mixer의 노출된 파라미터 이름 ---
    private const string MASTER_VOLUME_PARAM = "MasterVolume";
    private const string BGM_VOLUME_PARAM = "BGMVolume";
    private const string SFX_VOLUME_PARAM = "SFXVolume";

    // --- PlayerPrefs에 저장할 키 이름 ---
    private const string MASTER_VOL_KEY = "MasterVol";
    private const string BGM_VOL_KEY = "BGMVol";
    private const string SFX_VOL_KEY = "SFXVol";

    void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴 방지
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // BGM 및 SFX AudioSource가 없다면 생성 (Inspector에서 설정하는 것을 권장)
        if (bgmAudioSource == null)
        {
            bgmAudioSource = gameObject.AddComponent<AudioSource>();
            bgmAudioSource.loop = true;
        }
        if (sfxAudioSource == null)
        {
            sfxAudioSource = gameObject.AddComponent<AudioSource>();
            sfxAudioSource.loop = false;
        }

        LoadVolume(); // 저장된 볼륨 설정 불러오기 및 적용
    }

    // --- 볼륨 설정 메서드 (UI 슬라이더와 연결될 함수) ---

    // UI 슬라이더의 On Value Changed 이벤트에 연결합니다. (값 범위: 0.0f ~ 1.0f)
    public void SetMasterVolume(float volume)
    {
        SetVolume(MASTER_VOLUME_PARAM, MASTER_VOL_KEY, volume);
    }

    public void SetBGMVolume(float volume)
    {
        SetVolume(BGM_VOLUME_PARAM, BGM_VOL_KEY, volume);
    }

    public void SetSFXVolume(float volume)
    {
        SetVolume(SFX_VOLUME_PARAM, SFX_VOL_KEY, volume);
    }

    // 실제 볼륨 조절 로직 (선형 슬라이더 값을 로그 스케일 dB로 변환)
    private void SetVolume(string mixerParam, string prefKey, float volume)
    {
        // 0 ~ 1 범위의 선형 볼륨 값을 AudioMixer의 로그 스케일(dB) 값으로 변환
        // 0.0001f는 volume이 0일 때 무음(-80dB)으로 설정하기 위한 안전 장치
        float mixerValue = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;
        
        // Audio Mixer에 볼륨 적용
        audioMixer.SetFloat(mixerParam, mixerValue);
        
        // 설정 저장
        PlayerPrefs.SetFloat(prefKey, volume);
        PlayerPrefs.Save();
    }


    // --- 사운드 재생 메서드 ---

    // BGM 재생
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        
        // 이미 같은 BGM이 재생 중이면 중복 재생 방지
        if (bgmAudioSource.clip == clip && bgmAudioSource.isPlaying)
            return;

        bgmAudioSource.clip = clip;
        bgmAudioSource.Play();
    }
    
    // SFX 재생
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        
        // PlayOneShot을 사용하여 다른 효과음이 재생 중이어도 해당 클립을 한 번 재생
        sfxAudioSource.PlayOneShot(clip); 
    }


    // --- 볼륨 불러오기 ---

    private void LoadVolume()
    {
        // 저장된 값이 없으면 기본값 (1.0f)으로 설정
        float masterVol = PlayerPrefs.GetFloat(MASTER_VOL_KEY, 1.0f);
        float bgmVol = PlayerPrefs.GetFloat(BGM_VOL_KEY, 1.0f);
        float sfxVol = PlayerPrefs.GetFloat(SFX_VOL_KEY, 1.0f);
        
        // 불러온 값으로 볼륨 적용
        SetVolume(MASTER_VOLUME_PARAM, MASTER_VOL_KEY, masterVol);
        SetVolume(BGM_VOLUME_PARAM, BGM_VOL_KEY, bgmVol);
        SetVolume(SFX_VOLUME_PARAM, SFX_VOL_KEY, sfxVol);
    }
}