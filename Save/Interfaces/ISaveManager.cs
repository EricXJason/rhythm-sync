/// <summary>
/// ============================================================================
/// 檔案名稱: ISaveManager.cs
/// 所屬模組: Save
/// 責任描述: 全域存檔狀態管理契約。提供讀取記憶體快取與手動落盤介面。
/// 架構分層: Save / Interfaces
/// ============================================================================
/// </summary>
public interface ISaveManager
{
    RhythmSaveData CurrentSaveData { get; }
    void SaveGame();
    void LoadGame();
    void UpdateHighScore(int score, int combo, float distance);
}
