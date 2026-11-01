using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ============================================================================
/// 檔案名稱: TouchJumpButton.cs
/// 所屬模組: UI
/// 責任描述: 行動端專用跳躍觸控按鈕。實作 IPointerDownHandler，
///           當玩家觸控按下時通知 Player 執行跳躍。
/// 架構分層: UI / Controls
/// ============================================================================
/// </summary>
public class TouchJumpButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Player targetPlayer;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (targetPlayer != null)
        {
            targetPlayer.TriggerJumpInput();
        }
    }
}
