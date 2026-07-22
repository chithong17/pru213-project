using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Class định nghĩa một nhóm quái vật.
[System.Serializable]
public class EnemyGroup
{
    public GameObject enemyPrefab;
    public int count;
}

// Class định nghĩa một Wave.
[System.Serializable]
public class WaveConfig
{
    public EnemyGroup[] enemyGroups;
    public float timeBetweenSpawns = 1f;
}

public class WaveManager : MonoBehaviour
{
    public enum SpawnLaneMode
    {
        Alternate,
        Random,
        All
    }

    [Header("References")]
    public EnemySpawner enemySpawner;
    public EnemySpawner[] extraEnemySpawners;
    public CardChoiceManager cardManager;

    [Header("Wave Settings")]
    public int currentWave;
    public WaveConfig[] waves;
    public float timeBetweenWaves = 2f;
    public float timeBeforeNextScene = 1.5f;
    public SpawnLaneMode spawnLaneMode = SpawnLaneMode.Alternate;

    [Header("HUD")]
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private GameObject waveBanner;
    [SerializeField] private TMP_Text waveBannerText;
    [SerializeField] private float bannerDisplayTime = 1.5f;

    private Coroutine waveBannerCoroutine;
    private int nextSpawnerIndex;

    private void Start()
    {
        PlayerPrefs.SetString("LastLevelScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        // Ẩn banner trước khi bắt đầu wave.
        if (waveBanner != null)
        {
            waveBanner.SetActive(false);
        }

        StartCoroutine(RunWaves());
    }

    private IEnumerator RunWaves()
    {
        if (GetSpawnerCount() == 0)
        {
            Debug.LogWarning(
                "WaveManager chưa được gắn EnemySpawner! Object: "
                + gameObject.name
            );

            yield break;
        }

        if (waves == null || waves.Length == 0)
        {
            Debug.LogWarning(
                "WaveManager chưa có dữ liệu Wave! Object: "
                + gameObject.name
            );

            yield break;
        }

        for (int waveIndex = 0; waveIndex < waves.Length; waveIndex++)
        {
            currentWave = waveIndex + 1;
            WaveConfig currentWaveConfig = waves[waveIndex];

            UpdateWaveHUD();
            GameAudio.PlaySFX(GameAudio.WaveStart, 0.75f);
            ShowWaveBanner();

            // Đợi banner xuất hiện một chút trước khi sinh quái.
            yield return new WaitForSeconds(0.5f);

            // 1. Sinh quái theo kịch bản Wave.
            yield return StartCoroutine(SpawnWave(currentWaveConfig));

            // 2. Đợi toàn bộ quái của Wave bị tiêu diệt.
            yield return StartCoroutine(WaitForAllEnemiesToBeDefeated());

            // 3. Hiển thị thẻ bài sau mỗi 3 Wave.
            if (currentWave % 3 == 0 && currentWave < waves.Length)
            {
                if (cardManager != null)
                {
                    cardManager.ShowRandomCards();

                    yield return new WaitUntil(
                        () => Time.timeScale > 0f
                    );
                }
            }

            // 4. Nghỉ trước Wave tiếp theo.
            if (waveIndex < waves.Length - 1)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        Debug.Log("Đã hoàn thành toàn bộ Wave!");

        yield return new WaitForSeconds(timeBeforeNextScene);
        LoadNextScene();
    }

    private IEnumerator SpawnWave(WaveConfig config)
    {
        if (config == null || config.enemyGroups == null)
        {
            yield break;
        }

        foreach (EnemyGroup group in config.enemyGroups)
        {
            if (group == null || group.enemyPrefab == null)
            {
                Debug.LogWarning(
                    $"Wave {currentWave} có EnemyGroup chưa gắn prefab!"
                );

                continue;
            }

            for (int i = 0; i < group.count; i++)
            {
                SpawnEnemyFromLane(group.enemyPrefab);

                yield return new WaitForSeconds(
                    config.timeBetweenSpawns
                );
            }
        }
    }

    private IEnumerator WaitForAllEnemiesToBeDefeated()
    {
        while (HasActiveEnemies())
        {
            yield return null;
        }
    }

    private bool HasActiveEnemies()
    {
        for (int spawnerIndex = 0;
             spawnerIndex < GetSpawnerCount();
             spawnerIndex++)
        {
            EnemySpawner spawner = GetSpawnerAt(spawnerIndex);

            if (spawner == null || spawner.enemiesParent == null)
            {
                continue;
            }

            for (int i = 0; i < spawner.enemiesParent.childCount; i++)
            {
                Transform child = spawner.enemiesParent.GetChild(i);

                if (child.gameObject.activeSelf)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void SpawnEnemyFromLane(GameObject enemyPrefab)
    {
        int spawnerCount = GetSpawnerCount();

        if (spawnerCount == 0)
        {
            return;
        }

        if (spawnLaneMode == SpawnLaneMode.All)
        {
            for (int i = 0; i < spawnerCount; i++)
            {
                SpawnFromSpawner(GetSpawnerAt(i), enemyPrefab);
            }

            return;
        }

        int selectedIndex;

        if (spawnLaneMode == SpawnLaneMode.Random)
        {
            selectedIndex = Random.Range(0, spawnerCount);
        }
        else
        {
            selectedIndex = nextSpawnerIndex % spawnerCount;
            nextSpawnerIndex++;
        }

        SpawnFromSpawner(GetSpawnerAt(selectedIndex), enemyPrefab);
    }

    private void SpawnFromSpawner(
        EnemySpawner spawner,
        GameObject enemyPrefab
    )
    {
        if (spawner != null)
        {
            spawner.SpawnEnemy(enemyPrefab);
        }
    }

    private int GetSpawnerCount()
    {
        int count = enemySpawner != null ? 1 : 0;

        if (extraEnemySpawners != null)
        {
            for (int i = 0; i < extraEnemySpawners.Length; i++)
            {
                if (extraEnemySpawners[i] != null)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private EnemySpawner GetSpawnerAt(int index)
    {
        if (enemySpawner != null)
        {
            if (index == 0)
            {
                return enemySpawner;
            }

            index--;
        }

        if (extraEnemySpawners == null)
        {
            return null;
        }

        for (int i = 0; i < extraEnemySpawners.Length; i++)
        {
            EnemySpawner spawner = extraEnemySpawners[i];

            if (spawner == null)
            {
                continue;
            }

            if (index == 0)
            {
                return spawner;
            }

            index--;
        }

        return null;
    }

    private void UpdateWaveHUD()
    {
        if (waveText != null)
        {
            waveText.text = $"Wave {currentWave}";
        }
    }

    private void ShowWaveBanner()
    {
        if (waveBannerCoroutine != null)
        {
            StopCoroutine(waveBannerCoroutine);
        }

        waveBannerCoroutine =
            StartCoroutine(WaveBannerRoutine());
    }

    private void LoadNextScene()
    {
        string nextSceneName = GetNextSceneName();

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private string GetNextSceneName()
    {
        return "Victory";
    }
    private IEnumerator WaveBannerRoutine()
    {
        if (waveBannerText != null)
        {
            waveBannerText.text = $"WAVE {currentWave}";
        }

        if (waveBanner != null)
        {
            waveBanner.SetActive(true);
        }

        yield return new WaitForSeconds(bannerDisplayTime);

        if (waveBanner != null)
        {
            waveBanner.SetActive(false);
        }

        waveBannerCoroutine = null;
    }
}





