using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ============================================================================
/// 檔案名稱: TouchSlideButton.cs
/// 所屬模組: UI
/// 責任描述: 行動端專用滑行觸控按鈕。實作 IPointerDownHandler，
///           當玩家觸控按下時通知 Player 執行滑行。
/// 架構分層: UI / Controls
/// ============================================================================
/// </summary>
public class TouchSlideButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Player targetPlayer;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (targetPlayer != null)
        {
            targetPlayer.TriggerSlideInput();
        }
    }
}
