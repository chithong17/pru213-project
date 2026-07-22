using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (playerMovement == null)
        {
            return;
        }

        UpdateAnimation();
        UpdateFacingDirection();
    }

    private void UpdateAnimation()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetFloat("Speed", playerMovement.MoveInput.magnitude);
    }

    private void UpdateFacingDirection()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (playerMovement.MoveInput.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (playerMovement.MoveInput.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }
}
