using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: ObjectPool.cs
/// 所屬模組: Infrastructure
/// 責任描述: 簡易輕量 GameObject 物件池。消除跑酷遊戲中跳台與音符頻繁生成銷毀
///           所造成的 Instantiate / Destroy 記憶體垃圾回收開銷 (GC Alloc)。
/// 架構分層: Infrastructure / Pooling
/// 聯動關係:
///   - 介面依賴: IPoolable (物件出池與入池重置契約)
///   - 下游消費: 供 LevelGenerator, NoteGenerator 調用 Spawn 與 Despawn
/// 邊界處理: 具備字典防呆、擴容防護與 Editor 狀態檢查日誌。
/// ============================================================================
/// </summary>
public class ObjectPool : MonoBehaviour
{
    private static ObjectPool instance;

    [System.Serializable]
    public class PoolItem
    {
        public string poolKey;
        public GameObject prefab;
        public int initialCount = 10;
    }

    [Header("Preload Pool Items")]
    [SerializeField] private List<PoolItem> poolItems = new List<PoolItem>();

    // 物件佇列字典：key -> 待使用的物件隊列
    private readonly Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
    // Prefab 字典：key -> 原始預製體（供動態擴容時生成）
    private readonly Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    public static ObjectPool Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        InitializePool();
    }

    /// <summary>
    /// 初始化預設物件池，預先 Instantiate 物件並隱藏
    /// </summary>
    private void InitializePool()
    {
        foreach (var item in poolItems)
        {
            if (item.prefab == null || string.IsNullOrEmpty(item.poolKey))
            {
#if UNITY_EDITOR
                Debug.LogWarning("[ObjectPool] Found incomplete or null prefab pool item, skipped.");
#endif
                continue;
            }

            prefabDictionary[item.poolKey] = item.prefab;
            Queue<GameObject> queue = new Queue<GameObject>();

            for (int i = 0; i < item.initialCount; i++)
            {
                GameObject obj = CreateNewObject(item.prefab);
                queue.Enqueue(obj);
            }

            poolDictionary[item.poolKey] = queue;
        }

#if UNITY_EDITOR
        Debug.Log($"[ObjectPool] Pool initialized successfully with {poolDictionary.Count} categories.");
#endif
    }

    /// <summary>
    /// 從物件池取出物件 (若池空則自動補生成擴容)
    /// </summary>
    public GameObject Spawn(string poolKey, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(poolKey))
        {
#if UNITY_EDITOR
            Debug.LogError($"[ObjectPool] Spawn failed: pool key '{poolKey}' is not defined!");
#endif
            return null;
        }

        GameObject obj;
        Queue<GameObject> queue = poolDictionary[poolKey];

        if (queue.Count > 0)
        {
            obj = queue.Dequeue();
        }
        else
        {
            // 池空時動態擴容一個
            obj = CreateNewObject(prefabDictionary[poolKey]);
#if UNITY_EDITOR
            Debug.Log($"[ObjectPool] Pool '{poolKey}' exhausted, dynamically instantiated new instance.");
#endif
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        IPoolable poolable = obj.GetComponent<IPoolable>();
        poolable?.OnSpawn();

        return obj;
    }

    /// <summary>
    /// 將使用完畢之物件歸還回池
    /// </summary>
    public void Despawn(string poolKey, GameObject obj)
    {
        if (obj == null) return;

        IPoolable poolable = obj.GetComponent<IPoolable>();
        poolable?.OnDespawn();

        obj.SetActive(false);
        obj.transform.SetParent(transform);

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary[poolKey] = new Queue<GameObject>();
        }

        poolDictionary[poolKey].Enqueue(obj);
    }

    private GameObject CreateNewObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        return obj;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 編輯器驗證：提醒檢查重複 Key
        HashSet<string> keys = new HashSet<string>();
        foreach (var item in poolItems)
        {
            if (!string.IsNullOrEmpty(item.poolKey) && !keys.Add(item.poolKey))
            {
                Debug.LogWarning($"[ObjectPool] Duplicate pool key detected in Inspector: {item.poolKey}");
            }
        }
    }
#endif
}
