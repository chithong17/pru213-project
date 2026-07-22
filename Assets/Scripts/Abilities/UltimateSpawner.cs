using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UltimateSpawner : MonoBehaviour
{
    [Header("Skill E Settings")]
    public GameObject explosionPrefabE;
    public float cooldownE = 10f;
    private float lastCastTimeE = -99f;
    public Image cooldownOverlayE;

    [Header("Skill R Settings")]
    public GameObject explosionPrefabR;
    public float cooldownR = 15f;
    private float lastCastTimeR = -99f;
    public Image cooldownOverlayR;

    [Header("Skill Space Settings")]
    public GameObject explosionPrefabSpace;
    public float cooldownSpace = 12f;
    private float lastCastTimeSpace = -99f;
    public Image cooldownOverlaySpace;

    [Header("Visuals")]
    public GameObject previewIndicator;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip castSoundE;
    [SerializeField] private AudioClip castSoundR;
    [SerializeField] private AudioClip castSoundSpace;

    [SerializeField] private AudioClip poisonBubblingSound;
    [SerializeField] private float poisonBubblingDuration = 4f;

    [SerializeField, Range(0f, 1f)] private float castVolume = 0.8f;
    [SerializeField, Range(0f, 1f)] private float poisonBubblingVolume = 0.25f;

    private AudioSource poisonLoopSource;
    private Coroutine poisonSoundCoroutine;

    private enum AimingState
    {
        None,
        AimingE,
        AimingR,
        AimingSpace
    }

    private AimingState currentState = AimingState.None;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        CreatePoisonLoopSource();
    }

    private void CreatePoisonLoopSource()
    {
        GameObject poisonAudioObject = new GameObject("PoisonLoopAudio");
        poisonAudioObject.transform.SetParent(transform);

        poisonLoopSource = poisonAudioObject.AddComponent<AudioSource>();

        poisonLoopSource.playOnAwake = false;
        poisonLoopSource.loop = true;
        poisonLoopSource.spatialBlend = 0f;
        poisonLoopSource.volume = poisonBubblingVolume;
    }

    private void Update()
    {
        UpdateCooldownUI(cooldownOverlayE, lastCastTimeE, cooldownE);
        UpdateCooldownUI(cooldownOverlayR, lastCastTimeR, cooldownR);
        UpdateCooldownUI(
            cooldownOverlaySpace,
            lastCastTimeSpace,
            cooldownSpace
        );

        HandleSkillEInput();
        HandleSkillRInput();
        HandleSkillSpaceInput();

        HandleAiming();
    }

    private void HandleSkillEInput()
    {
        if (!Input.GetKeyDown(KeyCode.E))
        {
            return;
        }

        if (currentState == AimingState.AimingE)
        {
            CancelAiming();
        }
        else if (Time.time >= lastCastTimeE + cooldownE)
        {
            StartAiming(AimingState.AimingE);
        }
    }

    private void HandleSkillRInput()
    {
        if (!Input.GetKeyDown(KeyCode.R))
        {
            return;
        }

        if (currentState == AimingState.AimingR)
        {
            CancelAiming();
        }
        else if (Time.time >= lastCastTimeR + cooldownR)
        {
            StartAiming(AimingState.AimingR);
        }
    }

    private void HandleSkillSpaceInput()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
        {
            return;
        }

        if (currentState == AimingState.AimingSpace)
        {
            CancelAiming();
        }
        else if (Time.time >= lastCastTimeSpace + cooldownSpace)
        {
            StartAiming(AimingState.AimingSpace);
        }
    }

    private void HandleAiming()
    {
        if (currentState == AimingState.None)
        {
            return;
        }

        Vector3 mousePos =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mousePos.z = 0f;

        previewIndicator.transform.position = mousePos;

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            ExecuteSkill(mousePos);
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelAiming();
        }
    }

    private void UpdateCooldownUI(
        Image overlay,
        float lastCastTime,
        float cooldown
    )
    {
        if (overlay == null)
        {
            return;
        }

        float timePassed = Time.time - lastCastTime;

        if (timePassed < cooldown)
        {
            overlay.fillAmount =
                1f - (timePassed / cooldown);
        }
        else
        {
            overlay.fillAmount = 0f;
        }
    }

    private void StartAiming(AimingState state)
    {
        currentState = state;
        GameAudio.PlaySFX(GameAudio.UiClick, 0.65f);

        GameObject prefabToUse = GetPrefabForState(state);

        float skillRadius = GetSkillRadius(prefabToUse);

        DrawCirclePreview circlePreview =
            previewIndicator.GetComponent<DrawCirclePreview>();

        if (circlePreview != null)
        {
            circlePreview.DrawCircle(skillRadius);
        }

        previewIndicator.SetActive(true);
    }

    private GameObject GetPrefabForState(AimingState state)
    {
        if (state == AimingState.AimingE)
        {
            return explosionPrefabE;
        }

        if (state == AimingState.AimingR)
        {
            return explosionPrefabR;
        }

        if (state == AimingState.AimingSpace)
        {
            return explosionPrefabSpace;
        }

        return null;
    }

    private float GetSkillRadius(GameObject prefabToUse)
    {
        float skillRadius = 2f;

        if (prefabToUse == null)
        {
            return skillRadius;
        }

        ExplosionEffect explosionEffect =
            prefabToUse.GetComponent<ExplosionEffect>();

        if (explosionEffect != null)
        {
            return explosionEffect.radius;
        }

        PoisonEffect poisonEffect =
            prefabToUse.GetComponent<PoisonEffect>();

        if (poisonEffect != null)
        {
            return poisonEffect.radius;
        }

        return skillRadius;
    }

    private void ExecuteSkill(Vector3 pos)
    {
        GameObject prefabToSpawn = null;
        AudioClip castSoundToPlay = null;

        if (currentState == AimingState.AimingE)
        {
            prefabToSpawn = explosionPrefabE;
            castSoundToPlay = castSoundE;
            lastCastTimeE = Time.time;
        }
        else if (currentState == AimingState.AimingR)
        {
            prefabToSpawn = explosionPrefabR;
            castSoundToPlay = castSoundR;
            lastCastTimeR = Time.time;
        }
        else if (currentState == AimingState.AimingSpace)
        {
            prefabToSpawn = explosionPrefabSpace;
            castSoundToPlay = castSoundSpace;
            lastCastTimeSpace = Time.time;

            StartPoisonBubblingSound();
        }

        if (prefabToSpawn != null)
        {
            Instantiate(
                prefabToSpawn,
                pos,
                Quaternion.identity
            );
        }

        PlayCastSound(castSoundToPlay);

        CancelAiming();
    }

    private void PlayCastSound(AudioClip clip)
    {
        if (audioSource == null || clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip, castVolume);
    }

    private void StartPoisonBubblingSound()
    {
        if (poisonLoopSource == null ||
            poisonBubblingSound == null)
        {
            return;
        }

        if (poisonSoundCoroutine != null)
        {
            StopCoroutine(poisonSoundCoroutine);
        }

        poisonSoundCoroutine =
            StartCoroutine(PoisonSoundRoutine());
    }

    private System.Collections.IEnumerator PoisonSoundRoutine()
    {
        poisonLoopSource.clip = poisonBubblingSound;
        poisonLoopSource.volume = poisonBubblingVolume;
        poisonLoopSource.Play();

        yield return new WaitForSeconds(
            poisonBubblingDuration
        );

        poisonLoopSource.Stop();
        poisonLoopSource.clip = null;

        poisonSoundCoroutine = null;
    }

    private void CancelAiming()
    {
        currentState = AimingState.None;

        if (previewIndicator != null)
        {
            previewIndicator.SetActive(false);
        }
    }
}
