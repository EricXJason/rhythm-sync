using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: PlayerDeadState.cs
/// 所屬模組: Player
/// 責任描述: 玩家死亡結算狀態。鎖定操控、凍結位移、播放死亡動畫並發布結算事件。
/// 架構分層: Player / States
/// ============================================================================
/// </summary>
public class PlayerDeadState : IPlayerState
{
    private readonly Player player;
    private readonly PlayerStateMachine stateMachine;

    public PlayerDeadState(Player player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        player.Rb.linearVelocity = Vector2.zero;
        player.Rb.simulated = false; // 關閉物理模擬防止穿模
        player.Anim.SetBool("isDead", true);

        // 發布全域死亡事件供 GameManager 結算分數與 UI 彈出結算視窗
        EventBus.Publish(new PlayerDiedEvent(player.CurrentScore, player.DistanceTraveled));
    }

    public void Update() { }
    public void FixedUpdate() { }
    public void Exit() { }
}
