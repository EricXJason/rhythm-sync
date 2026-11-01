using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: DeadZone.cs
/// 所屬模組: Gameplay
/// 責任描述: 墜落深淵死亡邊界。跟隨玩家 X 軸水平移動，當玩家墜入深淵時直接判定死亡。
/// 架構分層: Gameplay / Trigger
/// ============================================================================
/// </summary>
public class DeadZone : MonoBehaviour
{
    [SerializeField] private Transform targetPlayer;

    private void Update()
    {
        if (targetPlayer != null)
        {
            // 保持 X 軸跟隨玩家，Y 軸固定在深淵底部
            Vector3 pos = transform.position;
            pos.x = targetPlayer.position.x;
            transform.position = pos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            player?.TakeDamage(999); // 墜崖直接致命傷害
        }
    }
}
