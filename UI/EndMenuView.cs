using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ============================================================================
/// 檔案名稱: EndMenuView.cs
/// 所屬模組: UI
/// 責任描述: 遊戲結束結算視圖。訂閱 PlayerDiedEvent 顯示最終分數與前進距離，
///           並提供重新開始按鈕互動。
/// 架構分層: UI / Views
/// ============================================================================
/// </summary>
public class EndMenuView : MonoBehaviour
{
    [SerializeField] private GameObject contentPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        if (contentPanel != null)
        {
            contentPanel.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }
    }

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    private void OnPlayerDied(PlayerDiedEvent evt)
    {
        if (contentPanel != null)
        {
            contentPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = $"最終分數: {evt.FinalScore:N0}";
        }

        if (highScoreText != null && SaveManager.Instance != null && SaveManager.Instance.CurrentSaveData != null)
        {
            highScoreText.text = $"歷史最高: {SaveManager.Instance.CurrentSaveData.highScore:N0}";
        }

        if (distanceText != null)
        {
            distanceText.text = $"奔馳距離: {evt.DistanceTraveled:F1} m";
        }
    }

    private void OnRestartClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
}
