using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: NoteGenerator.cs
/// 所屬模組: Gameplay
/// 責任描述: 節拍音符生成器。依據節奏頻譜或固定間距，自 ObjectPool 物件池取出
///           Note 實體生成於跑道前方，達成無 GC 消耗的音符生成。
/// 架構分層: Gameplay / Spawner
/// 聯動關係:
///   - 依賴: ObjectPool (物件池取用)
/// ============================================================================
/// </summary>
public class NoteGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    [SerializeField] private string notePoolKey = "Note";
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float spawnAheadDistance = 15f;
    [SerializeField] private float noteSpacing = 3f;
    [SerializeField] private float[] spawnHeightOffsets = new float[] { 1f, 2.5f };

    private float nextSpawnX = 10f;

    private void Update()
    {
        if (playerTransform == null) return;

        // 當玩家接近前方的生成點時，觸發生成
        if (playerTransform.position.x + spawnAheadDistance >= nextSpawnX)
        {
            SpawnNote();
            nextSpawnX += noteSpacing;
        }
    }

    private void SpawnNote()
    {
        if (ObjectPool.Instance == null) return;

        float randomHeight = spawnHeightOffsets[Random.Range(0, spawnHeightOffsets.Length)];
        Vector3 spawnPos = new Vector3(nextSpawnX, randomHeight, 0f);

        ObjectPool.Instance.Spawn(notePoolKey, spawnPos, Quaternion.identity);
    }
}
