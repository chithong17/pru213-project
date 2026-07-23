using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public float damage = 50f;
    public float radius = 1.5f; // Bán kính vụ nổ
    public float destroyTime = 1.0f; // Thời gian vụ nổ tồn tại (trùng với thời gian animation)

    void Start()
    {
        // 1. Gây sát thương ngay khi xuất hiện
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D hit in hitEnemies)
        {
            // Kiểm tra xem đối tượng có script EnemyHealth không
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }

        // 2. Tự hủy sau khi xong animation
        Destroy(gameObject, destroyTime);
    }

    // Vẽ bán kính vùng nổ trong Scene để dễ căn chỉnh (chỉ hiện khi chọn object)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}