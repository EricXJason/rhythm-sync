using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ============================================================================
/// 檔案名稱: GameManager.cs
/// 所屬模組: Gameplay
/// 責任描述: 跑酷遊戲流程仲裁控制器。實作 IGameManager 契約，管理遊戲狀態、
///           Combo 連擊、計分系統與重試流程。
/// 架構分層: Gameplay / Manager
/// 聯動關係:
///   - 介面實作: IGameManager
///   - 事件監聽: EventBus (NoteCollectedEvent, PlayerDiedEvent)
///   - 事件發布: EventBus (ScoreChangedEvent, GameStateChangedEvent)
/// ----------------------------------------------------------------------------
/// 【Unity 場景掛載指引】
/// 1. 掛載物件: 場景根節點下的空物件，命名為「[GameManager]」。
/// ============================================================================
/// </summary>
public class GameManager : MonoBehaviour, IGameManager
{
    private static GameManager instance;

    [Header("Scoring & Combo Settings")]
    [SerializeField] private int baseNoteScore = 100;
    [SerializeField] private float scorePerMeter = 10f;

    public static GameManager Instance => instance;

    public int Score { get; private set; } = 0;
    public int Combo { get; private set; } = 0;
    public bool IsPlaying { get; private set; } = false;

    private float distanceTraveled = 0f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        StartGame();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<NoteCollectedEvent>(OnNoteCollected);
        EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<NoteCollectedEvent>(OnNoteCollected);
        EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    public void StartGame()
    {
        Score = 0;
        Combo = 0;
        IsPlaying = true;
        Time.timeScale = 1f;

        EventBus.Publish(new GameStateChangedEvent(true));
        EventBus.Publish(new ScoreChangedEvent(Score, Combo));
    }

    public void PauseGame()
    {
        IsPlaying = false;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        IsPlaying = true;
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        IsPlaying = false;
        Time.timeScale = 0f;

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.UpdateHighScore(Score, Combo, distanceTraveled);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddScore(int amount)
    {
        if (!IsPlaying) return;

        Score += amount;
        EventBus.Publish(new ScoreChangedEvent(Score, Combo));
    }

    private void OnNoteCollected(NoteCollectedEvent evt)
    {
        Combo++;
        int gainedScore = (baseNoteScore + evt.NoteValue) * Mathf.Max(1, Combo / 10);
        AddScore(gainedScore);
    }

    private void OnPlayerDied(PlayerDiedEvent evt)
    {
        GameOver();
    }

#if UNITY_EDITOR
    private void Update()
    {
        // 編輯器除錯：按下 R 鍵快速重新開始
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }
#endif
}
