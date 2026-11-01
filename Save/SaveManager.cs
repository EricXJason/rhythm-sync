using System;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: SaveManager.cs
/// 所屬模組: Save
/// 責任描述: 全域存檔狀態管理器。實作 ISaveManager 契約，快取玩家最高紀錄與音量，
///           採用髒標記 (Dirty Flag) 機制排程自動落盤，防範手機端 I/O 掉幀。
/// 架構分層: Save / Manager
/// 聯動關係:
///   - 介面實作: ISaveManager
///   - 依賴: ISaveService (JsonSaveService 實作)
/// ----------------------------------------------------------------------------
/// 【Unity 場景掛載指引】
/// 1. 掛載物件: 場景根節點下的空物件，命名為「[SaveManager]」。
/// 2. 生命週期: 腳本內建 DontDestroyOnLoad，保持全域常駐。
/// ============================================================================
/// </summary>
public class SaveManager : MonoBehaviour, ISaveManager
{
    private static SaveManager instance;

    private const string SaveKey = "RhythmSync_SaveData";

    [Header("Auto Save Settings")]
    [SerializeField] private float autoSaveInterval = 30f;

    private ISaveService saveService;
    private RhythmSaveData currentSaveData;
    private bool isDirty = false;
    private float autoSaveTimer = 0f;

    public static SaveManager Instance => instance;

    public RhythmSaveData CurrentSaveData => currentSaveData;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        saveService = new JsonSaveService();
        LoadGame();
    }

    private void Update()
    {
        if (!isDirty) return;

        autoSaveTimer += Time.deltaTime;
        if (autoSaveTimer >= autoSaveInterval)
        {
            autoSaveTimer = 0f;
            SaveGame();
        }
    }

    public void LoadGame()
    {
        if (saveService.Exists(SaveKey))
        {
            currentSaveData = saveService.Load<RhythmSaveData>(SaveKey);
        }

        if (currentSaveData == null)
        {
            currentSaveData = new RhythmSaveData();
            isDirty = true;
        }

#if UNITY_EDITOR
        Debug.Log($"[SaveManager] Save data loaded, high score: {currentSaveData.highScore}, max combo: {currentSaveData.maxCombo}");
#endif
    }

    public void SaveGame()
    {
        if (currentSaveData == null) return;

        currentSaveData.lastSaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        saveService.Save(SaveKey, currentSaveData);
        isDirty = false;

#if UNITY_EDITOR
        Debug.Log("[SaveManager] Save data synced to disk (dirty flag cleared).");
#endif
    }

    /// <summary>
    /// 結算時更新最高分數、連擊與累積距離
    /// </summary>
    public void UpdateHighScore(int score, int combo, float distance)
    {
        if (currentSaveData == null) return;

        bool updated = false;
        if (score > currentSaveData.highScore)
        {
            currentSaveData.highScore = score;
            updated = true;
        }

        if (combo > currentSaveData.maxCombo)
        {
            currentSaveData.maxCombo = combo;
            updated = true;
        }

        currentSaveData.totalDistance += distance;
        isDirty = true;

#if UNITY_EDITOR
        if (updated)
        {
            Debug.Log($"[SaveManager] New high score achieved! Score: {currentSaveData.highScore}, Combo: {currentSaveData.maxCombo}");
        }
#endif
    }

    private void OnApplicationQuit()
    {
        if (isDirty)
        {
            SaveGame();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && isDirty)
        {
            SaveGame();
        }
    }
}
