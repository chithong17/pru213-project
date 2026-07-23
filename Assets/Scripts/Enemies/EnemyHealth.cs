using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 10f;
    public float currentHealth;
    public int goldReward = 5;

    [SerializeField] private EnemyHealthBar healthBar;

    private CurrencyManager currencyManager;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isInvulnerable;
    private Coroutine flashRoutine;
    private Coroutine poisonRoutine;
    private GameObject poisonBubbles;

    private void Start()
    {
        currentHealth = maxHealth;
        currencyManager = FindAnyObjectByType<CurrencyManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<EnemyHealthBar>();
        }

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }
        else
        {
            Debug.LogWarning("EnemyHealthBar is missing on " + gameObject.name);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable)
        {
            Debug.Log(gameObject.name + " is invulnerable.");
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log("Enemy HP: " + currentHealth + "/" + maxHealth);

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }
        else
        {
            Debug.LogWarning("Cannot update health bar because healthBar is null on " + gameObject.name);
        }

        if (spriteRenderer != null)
        {
            // Stop only the hit-flash routine so poison and boss phase routines keep running.
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }
            flashRoutine = StartCoroutine(FlashOnHit());
        }

        if (currentHealth <= 0f)
        {
            GiveGoldReward();
            Destroy(gameObject);
        }
    }

    public void SetInvulnerable(bool value)
    {
        isInvulnerable = value;
    }

    private void GiveGoldReward()
    {
        if (currencyManager != null)
        {
            currencyManager.AddGold(goldReward);
        }
    }

    private IEnumerator FlashOnHit()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    public void ApplyPoison(float damagePerSecond, float duration, GameObject bubblePrefab)
    {
        if (poisonRoutine != null)
        {
            StopCoroutine(poisonRoutine);
        }

        if (poisonBubbles != null)
        {
            Destroy(poisonBubbles);
        }

        poisonRoutine = StartCoroutine(
            PoisonRoutine(damagePerSecond, duration, bubblePrefab)
        );
    }

    private IEnumerator PoisonRoutine(float damagePerSecond, float duration, GameObject bubblePrefab)
    {
        if (bubblePrefab != null)
        {
            poisonBubbles = Instantiate(
                bubblePrefab,
                transform.position,
                Quaternion.identity,
                transform
            );
        }

        float timePassed = 0f;

        while (timePassed < duration)
        {
            TakeDamage(damagePerSecond);
            yield return new WaitForSeconds(1f);
            timePassed += 1f;
        }

        if (poisonBubbles != null)
        {
            Destroy(poisonBubbles);
            poisonBubbles = null;
        }

        poisonRoutine = null;
    }
}
