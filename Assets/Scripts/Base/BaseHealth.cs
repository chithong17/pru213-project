using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 20f;
    public float currentHealth;

    [Header("World Health Bar")]
    [SerializeField] private Transform worldHealthBarFill;

    [Header("HUD Health Bar")]
    [SerializeField] private Image hudHealthBarFill;
    [SerializeField] private TMP_Text hudHealthText;
    [SerializeField] private string gameOverSceneName = "GameOver";

    private Vector3 originalWorldFillScale;
    private Vector3 originalWorldFillPosition;
    private float originalWorldFillWidth;
    private bool isDead;

    private void Awake()
    {
        if (worldHealthBarFill != null)
        {
            originalWorldFillScale = worldHealthBarFill.localScale;
            originalWorldFillPosition = worldHealthBarFill.localPosition;

            SpriteRenderer fillRenderer =
                worldHealthBarFill.GetComponent<SpriteRenderer>();

            if (fillRenderer != null)
            {
                originalWorldFillWidth =
                    fillRenderer.sprite.bounds.size.x *
                    originalWorldFillScale.x;
            }
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }

        GameAudio.PlaySFX(GameAudio.BaseHit, 0.8f);
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        UpdateHealthUI();

        Debug.Log(
            "Base Health: " +
            currentHealth +
            " / " +
            maxHealth
        );

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (maxHealth <= 0f)
        {
            return;
        }

        float healthPercent =
            Mathf.Clamp01(currentHealth / maxHealth);

        UpdateWorldHealthBar(healthPercent);
        UpdateHUDHealthBar(healthPercent);
    }

    private void UpdateWorldHealthBar(float healthPercent)
    {
        if (worldHealthBarFill == null)
        {
            return;
        }

        float newScaleX =
            originalWorldFillScale.x * healthPercent;

        worldHealthBarFill.localScale = new Vector3(
            newScaleX,
            originalWorldFillScale.y,
            originalWorldFillScale.z
        );

        if (originalWorldFillWidth > 0f)
        {
            float newWidth =
                originalWorldFillWidth * healthPercent;

            float offsetX =
                (originalWorldFillWidth - newWidth) * 0.5f;

            worldHealthBarFill.localPosition = new Vector3(
                originalWorldFillPosition.x - offsetX,
                originalWorldFillPosition.y,
                originalWorldFillPosition.z
            );
        }
    }

    private void UpdateHUDHealthBar(float healthPercent)
    {
        if (hudHealthBarFill != null)
        {
            hudHealthBarFill.fillAmount = healthPercent;
        }

        if (hudHealthText != null)
        {
            hudHealthText.text =
                $"{currentHealth:0} / {maxHealth:0}";
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        Debug.Log("Base destroyed!");

        if (!string.IsNullOrEmpty(gameOverSceneName))
        {
            SceneManager.LoadScene(gameOverSceneName);
        }
    }
}

