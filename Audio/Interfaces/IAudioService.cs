using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: IAudioService.cs
/// 所屬模組: Audio
/// 責任描述: 節奏遊戲音訊管線契約介面。定義音樂播放、節拍偵測、音量調整之行為。
/// 架構分層: Audio / Interfaces
/// ============================================================================
/// </summary>
public interface IAudioService
{
    void PlayMusic(AudioClip clip);
    void PauseMusic();
    void ResumeMusic();
    void PlaySfx(string sfxName);
    void SetMusicVolume(float volume);
    void SetSfxVolume(float volume);
    float[] GetSpectrumData();
}
