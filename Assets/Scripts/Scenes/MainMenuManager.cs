using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private const string LastLevelKey = "LastLevelScene";

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void LoadGuide()
    {
        SceneManager.LoadScene("Guide");
    }

    public void LoadLevel0()
    {
        LoadLevel("Level0");
    }

    public void LoadLevel05()
    {
        LoadLevel("Level05");
    }

    public void LoadLevel1()
    {
        LoadLevel("Level1");
    }

    public void LoadVictory()
    {
        SceneManager.LoadScene("Victory");
    }

    public void LoadGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void RetryLastLevel()
    {
        string levelName = PlayerPrefs.GetString(LastLevelKey, "Level0");
        SceneManager.LoadScene(levelName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void LoadLevel(string levelName)
    {
        PlayerPrefs.SetString(LastLevelKey, levelName);
        PlayerPrefs.Save();
        SceneManager.LoadScene(levelName);
    }
}

