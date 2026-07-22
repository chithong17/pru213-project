using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip backgroundMusic;

    [Header("SFX Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip buildTowerSFX;
    [SerializeField] private AudioClip towerShootSFX;

    [Header("Music Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.5f;

    [Header("SFX Volumes")]
    [Range(0f, 1f)]
    [SerializeField] private float buildTowerVolume = 0.8f;

    [Range(0f, 1f)]
    [SerializeField] private float towerShootVolume = 0.6f;

    public AudioClip BuildTowerSFX => buildTowerSFX;
    public AudioClip TowerShootSFX => towerShootSFX;

    public float BuildTowerVolume => buildTowerVolume;
    public float TowerShootVolume => towerShootVolume;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    private void PlayBackgroundMusic()
    {
        if (musicSource == null || backgroundMusic == null)
        {
            Debug.LogWarning("Missing music source or background music.");
            return;
        }

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (sfxSource == null || clip == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip, volume);
    }
}