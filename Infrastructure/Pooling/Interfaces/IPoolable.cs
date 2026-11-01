/// <summary>
/// ============================================================================
/// 檔案名稱: IPoolable.cs
/// 所屬模組: Infrastructure
/// 責任描述: 可回收到物件池之物件行為契約。定義生成出池 (OnSpawn) 與
///           回收回池 (OnDespawn) 之生命週期回呼。
/// 架構分層: Infrastructure / Pooling
/// 聯動關係:
///   - 下游消費: 供 Note, Platform, MovingTrap 等高頻物件實作
/// ============================================================================
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// 物件從池中取出重用時觸發 (重置血量、位置、動畫狀態)
    /// </summary>
    void OnSpawn();

    /// <summary>
    /// 物件歸還回池中時觸發 (停止粒子、清除速度)
    /// </summary>
    void OnDespawn();
}
