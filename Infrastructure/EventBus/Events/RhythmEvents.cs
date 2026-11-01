/// <summary>
/// ============================================================================
/// 檔案名稱: RhythmEvents.cs
/// 所屬模組: Infrastructure
/// 責任描述: 集中宣告《節奏同奏》全域不可變事件資料結構 (readonly struct)。
/// 架構分層: Infrastructure / EventBus / Events
/// 聯動關係:
///   - 下游消費: 供 Gameplay, Player, UI, Audio, Visuals 等模組發布與訂閱
/// 邊界處理: 採用 readonly struct 達成 Zero-GC 零堆疊記憶體垃圾開銷
/// ============================================================================
/// </summary>

/// <summary>
/// 分數變更事件 (拾取音符或前進距離時觸發)
/// </summary>
public readonly struct ScoreChangedEvent
{
    public int CurrentScore { get; }
    public int ComboCount { get; }

    public ScoreChangedEvent(int currentScore, int comboCount)
    {
        CurrentScore = currentScore;
        ComboCount = comboCount;
    }
}

/// <summary>
/// 音符成功拾取事件 (觸發音效與視覺特效)
/// </summary>
public readonly struct NoteCollectedEvent
{
    public int NoteValue { get; }

    public NoteCollectedEvent(int noteValue)
    {
        NoteValue = noteValue;
    }
}

/// <summary>
/// 玩家受擊事件 (觸發擊退動畫與鏡頭震動)
/// </summary>
public readonly struct PlayerDamagedEvent
{
    public int RemainingHealth { get; }

    public PlayerDamagedEvent(int remainingHealth)
    {
        RemainingHealth = remainingHealth;
    }
}

/// <summary>
/// 玩家死亡事件 (觸發結算面板)
/// </summary>
public readonly struct PlayerDiedEvent
{
    public int FinalScore { get; }
    public float DistanceTraveled { get; }

    public PlayerDiedEvent(int finalScore, float distanceTraveled)
    {
        FinalScore = finalScore;
        DistanceTraveled = distanceTraveled;
    }
}

/// <summary>
/// 音樂節拍重音脈衝事件 (由頻譜分析器定時或重音時觸發，驅動相機與背景律動)
/// </summary>
public readonly struct BeatPulseEvent
{
    public float Intensity { get; }

    public BeatPulseEvent(float intensity)
    {
        Intensity = intensity;
    }
}

/// <summary>
/// 遊戲狀態切換事件
/// </summary>
public readonly struct GameStateChangedEvent
{
    public bool IsPlaying { get; }

    public GameStateChangedEvent(bool isPlaying)
    {
        IsPlaying = isPlaying;
    }
}
