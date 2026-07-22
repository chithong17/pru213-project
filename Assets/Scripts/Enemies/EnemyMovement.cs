using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform[] waypoints;
    public Transform endPoint;
    public float moveSpeed = 2f;
    public float damageToBase = 1f;

    private int currentWaypointIndex;
    private bool hasReachedEnd;
    private BaseHealth baseHealth;

    private void Awake()
    {
        CreatePlaceholderVisualIfNeeded();
    }

    private void Start()
    {
        if (endPoint != null)
        {
            baseHealth = endPoint.GetComponent<BaseHealth>();
        }
    }

    private void Update()
    {
        if (hasReachedEnd)
        {
            return;
        }

        Transform targetPoint = GetCurrentTarget();

        if (targetPoint == null)
        {
            return;
        }


        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPoint.position) <= 0.05f)
        {
            MoveToNextTarget();
        }
    }

    private Transform GetCurrentTarget()
    {
        if (waypoints != null && currentWaypointIndex < waypoints.Length)
        {
            return waypoints[currentWaypointIndex];
        }

        return endPoint;
    }

    private void MoveToNextTarget()
    {
        if (waypoints != null && currentWaypointIndex < waypoints.Length)
        {
            currentWaypointIndex++;
            return;
        }

        hasReachedEnd = true;
        DamageBaseAndDestroy();
    }

    private void DamageBaseAndDestroy()
    {
        if (baseHealth != null)
        {
            baseHealth.TakeDamage(damageToBase);
        }

        Destroy(gameObject);
    }

    private void CreatePlaceholderVisualIfNeeded()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        if (spriteRenderer.sprite != null)
        {
            return;
        }

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.red);
        texture.Apply();

        Sprite placeholderSprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, 1f, 1f),
            new Vector2(0.5f, 0.5f),
            1f
        );

        spriteRenderer.sprite = placeholderSprite;
        transform.localScale = new Vector3(0.5f, 0.5f, 1f);
    }

    public void SetupWaypoints(Transform[] path, Transform end)
    {
        waypoints = path;
        endPoint = end;

        if (endPoint != null)
        {
            baseHealth = endPoint.GetComponent<BaseHealth>();
        }
    }
}
