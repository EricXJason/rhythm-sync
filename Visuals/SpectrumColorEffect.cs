using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: SpectrumColorEffect.cs
/// 所屬模組: Visuals
/// 責任描述: 灰階色彩律動效果器。依據音樂能量在黑白與灰階之間動態插值 SpriteRenderer 顏色。
/// 架構分層: Visuals / Effects
/// ============================================================================
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpectrumColorEffect : MonoBehaviour
{
    [SerializeField] private Color baseColor = Color.gray;
    [SerializeField] private Color pulseColor = Color.white;
    [SerializeField] private float colorLerpSpeed = 10f;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (AudioSpectrumProvider.Instance == null || sr == null) return;

        float energy = AudioSpectrumProvider.Instance.CurrentAverageEnergy;
        Color targetColor = Color.Lerp(baseColor, pulseColor, energy * 3f);
        sr.color = Color.Lerp(sr.color, targetColor, Time.deltaTime * colorLerpSpeed);
    }
}
