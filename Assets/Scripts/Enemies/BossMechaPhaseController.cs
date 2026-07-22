using System.Collections;
using UnityEngine;

public class BossMechaPhaseController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private Animator animator;

    [Header("Phase Speed")]
    [SerializeField] private float normalSpeed = 0.7f;
    [SerializeField] private float enragedSpeed = 1f;

    [Header("Defense Phase")]
    [SerializeField] private float defenseDuration = 2.8f;

    [Header("Sound")]
    [SerializeField] private float bossTimeVolume = 0.65f;
    [SerializeField] private float bossDieVolume = 0.9f;

    private bool hasEnteredEnragedPhase;
    private bool isDefending;

    private void Awake()
    {
        if (enemyHealth == null)
        {
            enemyHealth = GetComponent<EnemyHealth>();
        }

        if (enemyMovement == null)
        {
            enemyMovement = GetComponent<EnemyMovement>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void OnEnable()
    {
        GameAudio.PlayLoop(GameAudio.BossTime, bossTimeVolume);
    }

    private void OnDestroy()
    {
        GameAudio.StopLoop(GameAudio.BossTime);

        if (enemyHealth != null && enemyHealth.currentHealth <= 0f)
        {
            GameAudio.PlaySFX(GameAudio.BossDie, bossDieVolume);
        }
    }

    private void Update()
    {
        if (enemyHealth == null || enemyHealth.maxHealth <= 0f)
        {
            return;
        }

        float healthPercent = enemyHealth.currentHealth / enemyHealth.maxHealth;

        if (isDefending)
        {
            SetMoveSpeed(0f);
            return;
        }

        if (healthPercent <= 0.3f && !hasEnteredEnragedPhase)
        {
            StartCoroutine(EnterEnragedPhaseRoutine());
            return;
        }

        SetMoveSpeed(hasEnteredEnragedPhase ? enragedSpeed : normalSpeed);
    }

    private void SetMoveSpeed(float speed)
    {
        if (enemyMovement != null)
        {
            enemyMovement.moveSpeed = speed;
        }
    }

    private IEnumerator EnterEnragedPhaseRoutine()
    {
        hasEnteredEnragedPhase = true;
        isDefending = true;
        SetMoveSpeed(0f);

        if (enemyHealth != null)
        {
            enemyHealth.SetInvulnerable(true);
        }

        if (animator != null)
        {
            animator.Play("MechaStoneGolem_Defense", 0, 0f);
        }

        yield return new WaitForSeconds(defenseDuration);

        if (enemyHealth != null)
        {
            enemyHealth.SetInvulnerable(false);
        }

        if (animator != null)
        {
            animator.Play("MechaStoneGolem_Walk", 0, 0f);
        }

        isDefending = false;
    }
}
