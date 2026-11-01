using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// ============================================================================
/// 檔案名稱: AudioManager.cs
/// 所屬模組: Audio
/// 責任描述: 全域音效與音樂播放管線。實作 IAudioService 契約，管理 AudioMixer、
///           BGM 播放與音效事件響應。
/// 架構分層: Audio / Service
/// 聯動關係:
///   - 介面實作: IAudioService
///   - 事件監聽: EventBus (NoteCollectedEvent, PlayerDamagedEvent, PlayerDiedEvent)
/// ----------------------------------------------------------------------------
/// 【Unity 場景掛載指引】
/// 1. 掛載物件: 場景根節點下的空物件，命名為「[AudioManager]」。
/// 2. Inspector 參數配置: 拖入 AudioMixer、BGM AudioSource 與 SFX AudioSource。
/// ============================================================================
/// </summary>
public class AudioManager : MonoBehaviour, IAudioService
{
    private static AudioManager instance;

    [Header("Mixer & Track Configuration")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("SFX Asset List")]
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip slideClip;
    [SerializeField] private AudioClip noteCollectClip;
    [SerializeField] private AudioClip damageClip;
    [SerializeField] private AudioClip deathClip;

    public static AudioManager Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        EventBus.Subscribe<NoteCollectedEvent>(OnNoteCollected);
        EventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);
        EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<NoteCollectedEvent>(OnNoteCollected);
        EventBus.Unsubscribe<PlayerDamagedEvent>(OnPlayerDamaged);
        EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (bgmSource == null || clip == null) return;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PauseMusic() => bgmSource?.Pause();
    public void ResumeMusic() => bgmSource?.UnPause();

    public void PlaySfx(string sfxName)
    {
        if (sfxSource == null) return;

        AudioClip clipToPlay = sfxName.ToLower() switch
        {
            "jump" => jumpClip,
            "slide" => slideClip,
            "note" => noteCollectClip,
            "damage" => damageClip,
            "death" => deathClip,
            _ => null
        };

        if (clipToPlay != null)
        {
            sfxSource.PlayOneShot(clipToPlay);
        }
    }

    public void SetMusicVolume(float volume) => SetMixerVolume("BGM", volume);
    public void SetSfxVolume(float volume) => SetMixerVolume("SFX", volume);

    public void SetMixerVolume(string parameterName, float normalizedVolume)
    {
        if (audioMixer == null || string.IsNullOrEmpty(parameterName)) return;

        float clamped = Mathf.Clamp(normalizedVolume, 0.0001f, 1f);
        float dB = Mathf.Log10(clamped) * 20f;
        audioMixer.SetFloat(parameterName, dB);
    }

    public float[] GetSpectrumData()
    {
        return AudioSpectrumProvider.Instance != null ? AudioSpectrumProvider.Instance.SpectrumSamples : new float[64];
    }

    private void OnNoteCollected(NoteCollectedEvent evt) => PlaySfx("note");
    private void OnPlayerDamaged(PlayerDamagedEvent evt) => PlaySfx("damage");
    private void OnPlayerDied(PlayerDiedEvent evt) => PlaySfx("death");
}
