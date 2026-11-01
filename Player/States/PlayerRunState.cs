using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: PlayerRunState.cs
/// 所屬模組: Player
/// 責任描述: 玩家奔跑狀態。控制角色在地面上的水平加速位移與跳躍/滑行切換判定。
/// 架構分層: Player / States
/// ============================================================================
/// </summary>
public class PlayerRunState : IPlayerState
{
    private readonly Player player;
    private readonly PlayerStateMachine stateMachine;

    public PlayerRunState(Player player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        player.Anim.SetBool("isRunning", true);
    }

    public void Update()
    {
        // 懸空檢測：若不在地面上，進入跳躍/下落狀態
        if (!player.IsGrounded())
        {
            stateMachine.ChangeState(player.JumpState);
            return;
        }

        // 跳躍輸入
        if (player.InputJump)
        {
            player.ConsumeJumpInput();
            stateMachine.ChangeState(player.JumpState);
            return;
        }

        // 滑行輸入
        if (player.InputSlide && player.CanSlide())
        {
            player.ConsumeSlideInput();
            stateMachine.ChangeState(player.SlideState);
            return;
        }
    }

    public void FixedUpdate()
    {
        // 維持水平跑酷向前速度
        player.Rb.linearVelocity = new Vector2(player.CurrentMoveSpeed, player.Rb.linearVelocity.y);
    }

    public void Exit()
    {
        player.Anim.SetBool("isRunning", false);
    }
}
