using UnityEngine;

public class TowerArcherVisual : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject arrowPrefab;

    private EnemyHealth currentTarget;
    private float currentDamage;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void PlayShoot(EnemyHealth target, float damage)
    {
        currentTarget = target;
        currentDamage = damage;

        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(
                AudioManager.Instance.TowerShootSFX,
                AudioManager.Instance.TowerShootVolume
            );
        }

        // Test tạm: bắn tên ngay
        SpawnArrow();
    }

    public void SpawnArrow()
    {
        Debug.Log("SpawnArrow called");

        if (arrowPrefab == null)
        {
            Debug.LogWarning("Arrow prefab is missing");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning("FirePoint is missing");
            return;
        }

        if (currentTarget == null)
        {
            Debug.LogWarning("Current target is missing");
            return;
        }

        GameObject arrowObject = Instantiate(
            arrowPrefab,
            firePoint.position,
            Quaternion.identity
        );

        Debug.Log("Arrow spawned: " + arrowObject.name);

        ArrowProjectile arrowProjectile = arrowObject.GetComponent<ArrowProjectile>();

        if (arrowProjectile != null)
        {
            arrowProjectile.Setup(currentTarget, currentDamage);
        }
        else
        {
            Debug.LogWarning("ArrowProjectile script is missing on arrow prefab");
        }
    }
}