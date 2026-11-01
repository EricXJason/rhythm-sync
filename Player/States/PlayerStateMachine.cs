using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: PlayerStateMachine.cs
/// 所屬模組: Player
/// 責任描述: 純 C# 玩家有限狀態機 (FSM) 控制器。管理當前狀態流轉，
///           確保狀態切換時安全執行 Exit 與 Enter，並在編輯器下輸出狀態流轉除錯追蹤。
/// 架構分層: Player / States
/// 聯動關係:
///   - 狀態相依: IPlayerState
///   - 下游消費: 由 Player 組件持有並驅動
/// ============================================================================
/// </summary>
public class PlayerStateMachine
{
    public IPlayerState CurrentState { get; private set; }

    /// <summary>
    /// 初始化狀態機至起始狀態
    /// </summary>
    public void Initialize(IPlayerState startingState)
    {
        CurrentState = startingState;
        CurrentState?.Enter();

#if UNITY_EDITOR
        Debug.Log($"[PlayerFSM] State machine initialized, initial state: {startingState?.GetType().Name}");
#endif
    }

    /// <summary>
    /// 切換至新狀態
    /// </summary>
    public void ChangeState(IPlayerState newState)
    {
        if (newState == null || newState == CurrentState) return;

#if UNITY_EDITOR
        string fromState = CurrentState != null ? CurrentState.GetType().Name : "None";
        string toState = newState.GetType().Name;
        Debug.Log($"[PlayerFSM] State transition: {fromState} -> {toState}");
#endif

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    /// <summary>
    /// 驅動當前狀態之邏輯更新
    /// </summary>
    public void Update()
    {
        CurrentState?.Update();
    }

    /// <summary>
    /// 驅動當前狀態之物理更新
    /// </summary>
    public void FixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }
}
