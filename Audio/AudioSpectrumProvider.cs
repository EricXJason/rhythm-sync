using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: AudioSpectrumProvider.cs
/// 所屬模組: Audio
/// 責任描述: 音訊快速傅立葉轉換 (FFT) 頻譜分析器。即時自 AudioSource 取樣 64/512
///           頻段資料，計算平均能量與重音脈衝，並對 EventBus 廣播 BeatPulseEvent。
/// 架構分層: Audio / Analysis
/// 聯動關係:
///   - 下游消費: 驅動 Visuals 領域中的動態縮放、鏡頭縮放與灰階變換組件
/// ============================================================================
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioSpectrumProvider : MonoBehaviour
{
    private static AudioSpectrumProvider instance;

    [Header("FFT Spectrum Configuration")]
    [SerializeField] private FFTWindow fftWindow = FFTWindow.BlackmanHarris;
    [SerializeField] private int sampleSize = 64;
    [SerializeField] private float beatThreshold = 0.35f;
    [SerializeField] private float beatCooldown = 0.15f;

    private AudioSource audioSource;
    private float[] spectrumSamples;
    private float beatTimer = 0f;

    public static AudioSpectrumProvider Instance => instance;

    /// <summary>
    /// 當前幀音訊頻譜平均能量強度 (0.0 ~ 1.0)
    /// </summary>
    public float CurrentAverageEnergy { get; private set; }

    /// <summary>
    /// 取得完整頻譜取樣陣列
    /// </summary>
    public float[] SpectrumSamples => spectrumSamples;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        audioSource = GetComponent<AudioSource>();
        spectrumSamples = new float[sampleSize];
    }

    private void Update()
    {
        if (audioSource == null || !audioSource.isPlaying) return;

        beatTimer -= Time.deltaTime;

        // 執行音訊頻譜取樣
        audioSource.GetSpectrumData(spectrumSamples, 0, fftWindow);

        // 計算低頻區間 (通常為 Bass / 重音區) 平均能量
        float energySum = 0f;
        int checkCount = Mathf.Min(8, sampleSize);
        for (int i = 0; i < checkCount; i++)
        {
            energySum += spectrumSamples[i];
        }

        CurrentAverageEnergy = energySum / checkCount;

        // 偵測重音脈衝
        if (CurrentAverageEnergy > beatThreshold && beatTimer <= 0f)
        {
            beatTimer = beatCooldown;
            EventBus.Publish(new BeatPulseEvent(CurrentAverageEnergy));
        }
    }
}
