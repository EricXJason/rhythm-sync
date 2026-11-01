using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: UIManager.cs
/// 所屬模組: UI
/// 責任描述: 全域 UI 協調控制器。管理遊戲 HUD、主選單與結算面板切換。
/// 架構分層: UI / Coordinator
/// ----------------------------------------------------------------------------
/// 【Unity 場景掛載指引】
/// 1. 掛載物件: 場景畫布根節點「Canvas」或「[UIManager]」。
/// ============================================================================
/// </summary>
public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    [Header("UI Panel Configuration")]
    [SerializeField] private GameObject gameHudPanel;
    [SerializeField] private GameObject endMenuPanel;

    public static UIManager Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void ShowGameHud()
    {
        gameHudPanel?.SetActive(true);
        endMenuPanel?.SetActive(false);
    }

    public void ShowEndMenu()
    {
        endMenuPanel?.SetActive(true);
    }
}
