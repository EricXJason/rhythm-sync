using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: PlayerSlideState.cs
/// 所屬模組: Player
/// 責任描述: 玩家滑行狀態。縮小碰撞體以穿過低矮障礙物，具備計時器與冷卻控制。
/// 架構分層: Player / States
/// ============================================================================
/// </summary>
public class PlayerSlideState : IPlayerState
{
    private readonly Player player;
    private readonly PlayerStateMachine stateMachine;
    private float slideTimer = 0f;

    public PlayerSlideState(Player player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        slideTimer = player.SlideDuration;
        player.SetColliderHeight(player.SlideColliderHeight);
        player.Anim.SetBool("isSliding", true);
    }

    public void Update()
    {
        slideTimer -= Time.deltaTime;

        // 滑行時間到或玩家手動跳躍時解除滑行
        if (slideTimer <= 0f || player.InputJump)
        {
            stateMachine.ChangeState(player.RunState);
            return;
        }
    }

    public void FixedUpdate()
    {
        // 滑行時以專屬滑行速度向前突進
        player.Rb.linearVelocity = new Vector2(player.SlideSpeed, player.Rb.linearVelocity.y);
    }

    public void Exit()
    {
        // 恢復正常碰撞體高度
        player.SetColliderHeight(player.NormalColliderHeight);
        player.Anim.SetBool("isSliding", false);
        player.ResetSlideCooldown();
    }
}
