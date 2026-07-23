using UnityEngine;

public class PoisonEffect : MonoBehaviour
{
    [Header("Poison Settings")]
    public float damagePerSecond = 2f; 
    public float poisonDuration = 5f; 
    public GameObject bubblePrefab;    

    [Header("Explosion Settings")]
    public float radius = 2f;          
    public float destroyTime = 1f;     

    void Start()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D hit in colliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.ApplyPoison(damagePerSecond, poisonDuration, bubblePrefab);
                }
            }
        }

        Destroy(gameObject, destroyTime);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}