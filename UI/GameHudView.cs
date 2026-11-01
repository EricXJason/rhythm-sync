using TMPro;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: GameHudView.cs
/// 所屬模組: UI
/// 責任描述: 遊戲進行中即時 HUD 視圖。訂閱 ScoreChangedEvent 刷新分數與連擊數顯示。
/// 架構分層: UI / Views
/// 聯動關係:
///   - 事件監聽: EventBus (ScoreChangedEvent)
/// ============================================================================
/// </summary>
public class GameHudView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;

    private void OnEnable()
    {
        EventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
    }

    private void OnScoreChanged(ScoreChangedEvent evt)
    {
        if (scoreText != null)
        {
            scoreText.text = evt.CurrentScore.ToString("N0");
        }

        if (comboText != null)
        {
            comboText.text = evt.ComboCount > 1 ? $"{evt.ComboCount} COMBO!" : string.Empty;
        }
    }
}
