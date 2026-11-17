using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ResumeGame()
    {
        SceneManager.LoadScene("Patryk Scene");
    }

    public void NewGame()
    {
        SceneManager.LoadScene("Patryk Scene");
    }

    public void Settings()
    {
        Debug.Log("Settings");
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame");
    }
}