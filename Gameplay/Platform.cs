using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: Platform.cs
/// 所屬模組: Gameplay
/// 責任描述: 跑酷跳台物件。實作 IPoolable 契約，支援被物件池回收重用。
/// 架構分層: Gameplay / Entity
/// ============================================================================
/// </summary>
public class Platform : MonoBehaviour, IPoolable
{
    [SerializeField] private string poolKey = "Platform";

    public void OnSpawn()
    {
        gameObject.SetActive(true);
    }

    public void OnDespawn()
    {
    }

    /// <summary>
    /// 當玩家遠離跳台時主動回收
    /// </summary>
    public void Recycle()
    {
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.Despawn(poolKey, gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
