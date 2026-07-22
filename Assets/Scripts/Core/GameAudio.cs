using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameAudio
{
    public const string UiHover = "ui_hover";
    public const string UiClick = "ui_click";
    public const string CardSelect = "card_select";
    public const string WaveStart = "wave_start";
    public const string BaseHit = "base_hit";
    public const string TowerUpgrade = "tower_upgrade";
    public const string BuildTower = "build_tower";
    public const string NotEnoughGold = "not_enough_gold";
    public const string BossTime = "boss_time";
    public const string BossDie = "boss_die";
    public const string Victory = "victory";
    public const string GameOver = "game_over";
    public const string MenuMusic = "sound-start";

    private const string ResourceRoot = "Audio/SFX/";
    private const float UiClickVolumeBoost = 1.4f;
    private const float MenuMusicVolume = 0.45f;

    private static readonly Dictionary<string, AudioClip> clipCache =
        new Dictionary<string, AudioClip>();

    private static readonly Dictionary<string, AudioSource> loopSources =
        new Dictionary<string, AudioSource>();

    private static AudioSource oneShotSource;
    private static GameObject audioObject;
    private static bool initialized;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeOnLoad()
    {
        EnsureInitialized();
    }

    public static void PlaySFX(string clipName, float volume = 1f)
    {
        EnsureInitialized();

        AudioClip clip = LoadClip(clipName);
        if (clip == null || oneShotSource == null)
        {
            return;
        }

        if (clipName == UiClick)
        {
            volume *= UiClickVolumeBoost;
        }

        oneShotSource.PlayOneShot(clip, Mathf.Clamp01(volume));
    }

    public static void PlayLoop(string clipName, float volume = 1f)
    {
        EnsureInitialized();

        AudioClip clip = LoadClip(clipName);
        if (clip == null || audioObject == null)
        {
            return;
        }

        if (!loopSources.TryGetValue(clipName, out AudioSource loopSource) ||
            loopSource == null)
        {
            loopSource = audioObject.AddComponent<AudioSource>();
            loopSource.loop = true;
            loopSources[clipName] = loopSource;
        }

        if (loopSource.isPlaying && loopSource.clip == clip)
        {
            return;
        }

        loopSource.clip = clip;
        loopSource.volume = volume;
        loopSource.loop = true;
        loopSource.Play();
    }

    public static void StopLoop(string clipName)
    {
        if (!loopSources.TryGetValue(clipName, out AudioSource loopSource) ||
            loopSource == null)
        {
            return;
        }

        loopSource.Stop();
        loopSource.clip = null;
    }

    private static void EnsureInitialized()
    {
        if (initialized && audioObject != null && oneShotSource != null)
        {
            return;
        }

        audioObject = new GameObject("GameAudio");
        Object.DontDestroyOnLoad(audioObject);

        oneShotSource = audioObject.AddComponent<AudioSource>();
        initialized = true;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static AudioClip LoadClip(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
        {
            return null;
        }

        if (clipCache.TryGetValue(clipName, out AudioClip cachedClip))
        {
            return cachedClip;
        }

        AudioClip clip = Resources.Load<AudioClip>(ResourceRoot + clipName);
        clipCache[clipName] = clip;

        if (clip == null)
        {
            Debug.LogWarning("Missing audio clip in Resources: " + clipName);
        }

        return clip;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopLoop(BossTime);

        if (IsMenuScene(scene.name))
        {
            PlayLoop(MenuMusic, MenuMusicVolume);
        }
        else
        {
            StopLoop(MenuMusic);
        }

        if (scene.name == "Victory")
        {
            PlaySFX(Victory, 0.9f);
        }
        else if (scene.name == "GameOver")
        {
            PlaySFX(GameOver, 0.9f);
        }
    }

    private static bool IsMenuScene(string sceneName)
    {
        return sceneName == "MainMenu" ||
            sceneName == "Guide" ||
            sceneName == "LevelSelect";
    }
}

