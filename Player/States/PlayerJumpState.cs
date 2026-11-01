using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: PlayerJumpState.cs
/// 所屬模組: Player
/// 責任描述: 玩家跳躍與滯空狀態。支援一段跳與二段跳，以及懸崖抓牆 (Ledge) 檢測。
/// 架構分層: Player / States
/// ============================================================================
/// </summary>
public class PlayerJumpState : IPlayerState
{
    private readonly Player player;
    private readonly PlayerStateMachine stateMachine;
    private bool canDoubleJump = true;

    public PlayerJumpState(Player player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        player.Anim.SetBool("isJumping", true);

        // 若從地面起跳，施加一段跳向上的力
        if (player.IsGrounded())
        {
            player.Rb.linearVelocity = new Vector2(player.CurrentMoveSpeed, player.JumpForce);
            canDoubleJump = true;
        }
    }

    public void Update()
    {
        // 抓牆懸崖檢測
        if (player.CheckLedge())
        {
            stateMachine.ChangeState(player.LedgeState);
            return;
        }

        // 二段跳輸入
        if (player.InputJump && canDoubleJump)
        {
            player.ConsumeJumpInput();
            canDoubleJump = false;
            player.Rb.linearVelocity = new Vector2(player.CurrentMoveSpeed, player.DoubleJumpForce);
            player.Anim.SetTrigger("doubleJump");
            return;
        }

        // 落地切回奔跑狀態
        if (player.IsGrounded() && player.Rb.linearVelocity.y <= 0.1f)
        {
            stateMachine.ChangeState(player.RunState);
            return;
        }
    }

    public void FixedUpdate()
    {
        // 滯空期間維持前進水平速度
        player.Rb.linearVelocity = new Vector2(player.CurrentMoveSpeed, player.Rb.linearVelocity.y);
    }

    public void Exit()
    {
        player.Anim.SetBool("isJumping", false);
    }
}
