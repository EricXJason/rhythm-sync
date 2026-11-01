using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: SpectrumScaleEffect.cs
/// 所屬模組: Visuals
/// 責任描述: 頻譜動態縮放效果器。讀取 AudioSpectrumProvider 頻譜特定頻段數據，
///           驅動 GameObject 物件進行平滑縮放律動 (音波柱狀效果)。
/// 架構分層: Visuals / Effects
/// ============================================================================
/// </summary>
public class SpectrumScaleEffect : MonoBehaviour
{
    [SerializeField] private int spectrumBandIndex = 0;
    [SerializeField] private float scaleMultiplier = 5f;
    [SerializeField] private float minScale = 1f;
    [SerializeField] private float maxScale = 3f;
    [SerializeField] private float smoothSpeed = 15f;

    private Vector3 initialScale;

    private void Start()
    {
        initialScale = transform.localScale;
    }

    private void Update()
    {
        if (AudioSpectrumProvider.Instance == null) return;

        float[] samples = AudioSpectrumProvider.Instance.SpectrumSamples;
        if (samples != null && spectrumBandIndex < samples.Length)
        {
            float intensity = samples[spectrumBandIndex] * scaleMultiplier;
            float targetY = Mathf.Clamp(initialScale.y + intensity, minScale, maxScale);

            Vector3 targetScale = new Vector3(initialScale.x, targetY, initialScale.z);
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * smoothSpeed);
        }
    }
}
