using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: MovingTrap.cs
/// 所屬模組: Gameplay
/// 責任描述: 移動障礙陷阱。在兩個路徑點之間往返移動，觸碰玩家時造成傷害擊退。
/// 架構分層: Gameplay / Entity
/// ============================================================================
/// </summary>
public class MovingTrap : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float moveDistance = 4f;
    [SerializeField] private int damageAmount = 1;

    private Vector3 startPosition;
    private int direction = 1;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.Translate(Vector2.up * direction * moveSpeed * Time.deltaTime);

        if (Vector3.Distance(startPosition, transform.position) >= moveDistance)
        {
            direction *= -1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            player?.TakeDamage(damageAmount);
        }
    }
}
