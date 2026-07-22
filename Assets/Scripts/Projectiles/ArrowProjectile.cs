using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float hitDistance = 0.5f;
    [SerializeField] private float lifeTime = 3f;

    private EnemyHealth target;
    private float damage;

    private void Awake()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 6000;
        }
    }

    public void Setup(EnemyHealth newTarget, float newDamage)
    {
        target = newTarget;
        damage = newDamage;

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPosition = target.transform.position;
        targetPosition.z = 0f;

        Vector3 direction = targetPosition - transform.position;
        direction.z = 0f;

        transform.position += direction.normalized * speed * Time.deltaTime;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        if (Vector3.Distance(transform.position, targetPosition) <= hitDistance)
        {
            Debug.Log("Arrow hit enemy: " + target.name + " damage = " + damage);

            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}