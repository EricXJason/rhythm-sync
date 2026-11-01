using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: PlayerKnockbackState.cs
/// 所屬模組: Player
/// 責任描述: 玩家碰撞受傷擊退狀態。施加後退力道並暫時鎖定操控，結束後返回奔跑。
/// 架構分層: Player / States
/// ============================================================================
/// </summary>
public class PlayerKnockbackState : IPlayerState
{
    private readonly Player player;
    private readonly PlayerStateMachine stateMachine;
    private float knockbackTimer = 0f;

    public PlayerKnockbackState(Player player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        knockbackTimer = player.KnockbackDuration;
        player.Anim.SetTrigger("knockback");

        // 施加受擊擊退物理衝量
        player.Rb.linearVelocity = player.KnockbackForce;
        EventBus.Publish(new PlayerDamagedEvent(player.CurrentHealth));
    }

    public void Update()
    {
        knockbackTimer -= Time.deltaTime;

        if (knockbackTimer <= 0f)
        {
            stateMachine.ChangeState(player.RunState);
        }
    }

    public void FixedUpdate() { }

    public void Exit()
    {
        player.ResetKnockbackInvincibility();
    }
}
