using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: PlayerLedgeState.cs
/// 所屬模組: Player
/// 責任描述: 玩家懸崖攀爬狀態。懸掛於跳台邊緣，播放攀爬動畫後登頂並切回奔跑。
/// 架構分層: Player / States
/// ============================================================================
/// </summary>
public class PlayerLedgeState : IPlayerState
{
    private readonly Player player;
    private readonly PlayerStateMachine stateMachine;
    private bool isClimbing = false;

    public PlayerLedgeState(Player player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        isClimbing = false;
        player.Rb.linearVelocity = Vector2.zero;
        player.Rb.gravityScale = 0f; // 懸掛時凍結重力

        player.Anim.SetBool("isLedgeClimbing", true);
        TriggerClimbUp();
    }

    private void TriggerClimbUp()
    {
        if (isClimbing) return;
        isClimbing = true;

        // 登頂位移
        player.StartCoroutine(PerformClimbSequence());
    }

    private System.Collections.IEnumerator PerformClimbSequence()
    {
        yield return new WaitForSeconds(0.3f);

        // 將玩家傳送至跳台上方平台
        player.transform.position = player.LedgeTargetPosition;

        player.Rb.gravityScale = player.NormalGravityScale;
        stateMachine.ChangeState(player.RunState);
    }

    public void Update() { }
    public void FixedUpdate() { }

    public void Exit()
    {
        player.Rb.gravityScale = player.NormalGravityScale;
        player.Anim.SetBool("isLedgeClimbing", false);
    }
}
