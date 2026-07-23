using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Transform fillBar;

    private Vector3 originalScale;
    private Vector3 originalPosition;

    [SerializeField] private float positionOffset = 0.25f;
    private float autoOffset;

    private void Awake()
    {
        if (fillBar != null)
        {
            originalScale = fillBar.localScale;
            originalPosition = fillBar.localPosition;

            SpriteRenderer sr = fillBar.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                autoOffset = sr.sprite.bounds.extents.x * originalScale.x;
            }
            else
            {
                autoOffset = positionOffset;
            }
        }
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (fillBar == null || maxHealth <= 0f)
        {
            return;
        }

        float percent = Mathf.Clamp01(currentHealth / maxHealth);

        fillBar.localScale = new Vector3(
            originalScale.x * percent,
            originalScale.y,
            originalScale.z
        );

        fillBar.localPosition = new Vector3(
            originalPosition.x - autoOffset * (1f - percent),
            originalPosition.y,
            originalPosition.z
        );
    }
}