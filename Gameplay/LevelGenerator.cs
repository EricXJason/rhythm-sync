using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: LevelGenerator.cs
/// 所屬模組: Gameplay
/// 責任描述: 無限隨機跑道生成器。自 ObjectPool 取用 Platform 跳台拼裝地形，
///           並動態回收遠離玩家視野後方的跳台，維持恆定記憶體佔用。
/// 架構分層: Gameplay / Spawner
/// 聯動關係:
///   - 依賴: ObjectPool (跳台取用與回收)
/// ============================================================================
/// </summary>
public class LevelGenerator : MonoBehaviour
{
    [Header("Generation Parameters")]
    [SerializeField] private string platformPoolKey = "Platform";
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float spawnAheadDistance = 25f;
    [SerializeField] private float platformWidth = 10f;
    [SerializeField] private float destroyBehindDistance = 15f;

    private float nextSpawnX = 0f;
    private readonly Queue<GameObject> activePlatforms = new Queue<GameObject>();

    private void Start()
    {
        // 初始預先鋪設 3 塊跳台
        for (int i = 0; i < 3; i++)
        {
            SpawnPlatform();
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // 向前生成
        if (playerTransform.position.x + spawnAheadDistance >= nextSpawnX)
        {
            SpawnPlatform();
        }

        // 向後回收
        if (activePlatforms.Count > 0)
        {
            GameObject oldestPlatform = activePlatforms.Peek();
            if (oldestPlatform != null && playerTransform.position.x - oldestPlatform.transform.position.x > destroyBehindDistance)
            {
                activePlatforms.Dequeue();
                if (ObjectPool.Instance != null)
                {
                    ObjectPool.Instance.Despawn(platformPoolKey, oldestPlatform);
                }
                else
                {
                    Destroy(oldestPlatform);
                }
            }
        }
    }

    private void SpawnPlatform()
    {
        Vector3 spawnPos = new Vector3(nextSpawnX, 0f, 0f);
        GameObject platform = null;

        if (ObjectPool.Instance != null)
        {
            platform = ObjectPool.Instance.Spawn(platformPoolKey, spawnPos, Quaternion.identity);
        }

        if (platform != null)
        {
            activePlatforms.Enqueue(platform);
        }

        nextSpawnX += platformWidth;
    }
}
