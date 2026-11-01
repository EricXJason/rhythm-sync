/// <summary>
/// ============================================================================
/// 檔案名稱: IGameManager.cs
/// 所屬模組: Gameplay
/// 責任描述: 遊戲流程與計分仲裁契約介面。
/// 架構分層: Gameplay / Interfaces
/// ============================================================================
/// </summary>
public interface IGameManager
{
    int Score { get; }
    int Combo { get; }
    bool IsPlaying { get; }

    void StartGame();
    void PauseGame();
    void ResumeGame();
    void GameOver();
    void RestartGame();
    void AddScore(int amount);
}
