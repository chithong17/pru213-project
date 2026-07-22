using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameExitController : MonoBehaviour
{
    public void ExitToLevelSelect()
    {
        GameAudio.PlaySFX(GameAudio.UiClick, 0.8f);
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelect");
    }

    public void ExitToMainMenu()
    {
        GameAudio.PlaySFX(GameAudio.UiClick, 0.8f);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
