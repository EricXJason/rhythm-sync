using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: Note.cs
/// 所屬模組: Gameplay
/// 責任描述: 節拍音符物件。實作 IPoolable 契約，被玩家觸碰時廣播收集事件並歸還回物件池。
/// 架構分層: Gameplay / Entity
/// 聯動關係:
///   - 介面實作: IPoolable
///   - 事件發布: EventBus (NoteCollectedEvent)
/// ============================================================================
/// </summary>
public class Note : MonoBehaviour, IPoolable
{
    [SerializeField] private int noteValue = 100;
    [SerializeField] private string poolKey = "Note";

    public void OnSpawn()
    {
        // 從池中取出時重置視覺與碰撞
        gameObject.SetActive(true);
    }

    public void OnDespawn()
    {
        // 歸還池中時清理
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EventBus.Publish(new NoteCollectedEvent(noteValue));

            // 回收回物件池，杜絕 Destroy
            if (ObjectPool.Instance != null)
            {
                ObjectPool.Instance.Despawn(poolKey, gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
