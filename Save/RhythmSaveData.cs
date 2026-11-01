using System;

/// <summary>
/// ============================================================================
/// 檔案名稱: RhythmSaveData.cs
/// 所屬模組: Save
/// 責任描述: 跑酷遊戲玩家存檔資料結構 (POCO DTO)。儲存最高分數、最高連擊、
///           累計奔跑距離與音量設定。
/// 架構分層: Save / Data
/// ============================================================================
/// </summary>
[Serializable]
public class RhythmSaveData
{
    public int highScore = 0;
    public int maxCombo = 0;
    public float totalDistance = 0f;
    public float bgmVolume = 0.8f;
    public float sfxVolume = 1.0f;
    public string lastSaveTime = string.Empty;

    public RhythmSaveData()
    {
        lastSaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
