/// <summary>
/// ============================================================================
/// 檔案名稱: IPlayerState.cs
/// 所屬模組: Player
/// 責任描述: 純 C# 玩家狀態契約介面。定義狀態進入 (Enter)、幀更新 (Update)、
///           物理更新 (FixedUpdate) 與狀態退出 (Exit) 之生命週期。
/// 架構分層: Player / States
/// 聯動關係:
///   - 下游消費: 供所有具體玩家狀態實作 (Run, Jump, Slide, Ledge, Knockback, Dead)
/// ============================================================================
/// </summary>
public interface IPlayerState
{
    /// <summary>
    /// 進入此狀態時觸發 (啟動動畫、重置計時器、施加初始速度)
    /// </summary>
    void Enter();

    /// <summary>
    /// 每幀執行 (處理輸入與狀態轉換條件)
    /// </summary>
    void Update();

    /// <summary>
    /// 物理固定幀執行 (處理 Rigidbody2D 速度與位移)
    /// </summary>
    void FixedUpdate();

    /// <summary>
    /// 離開此狀態時觸發 (關閉動畫布林值、清理物理限制)
    /// </summary>
    void Exit();
}
