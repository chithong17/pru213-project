using UnityEngine;

public class BossMechaProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float damage = 2f;
    [SerializeField] private float hitDistance = 0.25f;
    [SerializeField] private float lifeTime = 5f;

    private BaseHealth targetBase;
    private Vector3 targetPosition;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Setup(BaseHealth newTargetBase, float newDamage)
    {
        targetBase = newTargetBase;
        damage = newDamage;

        if (targetBase != null)
        {
            targetPosition = targetBase.transform.position;
        }
    }

    private void Update()
    {
        if (targetBase != null)
        {
            targetPosition = targetBase.transform.position;
        }

        Vector3 direction = targetPosition - transform.position;
        direction.z = 0f;

        if (direction.sqrMagnitude <= hitDistance * hitDistance)
        {
            HitTarget();
            return;
        }

        transform.position += direction.normalized * speed * Time.deltaTime;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void HitTarget()
    {
        if (targetBase != null)
        {
            targetBase.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
