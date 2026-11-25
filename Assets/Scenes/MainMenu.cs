using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private ConfirmationPopup overwriteSavePopup;

    public void ResumeGame()
    {
        SaveData data = SaveSystem.Load();

        if (data == null)
        {
            Debug.LogWarning("Brak zapisu do wczytania.");
            return;
        }

        SaveSystem.SetPendingLoad(data);
        SceneManager.LoadScene(data.sceneName);
    }

    public void NewGame()
    {
        const string startingScene = "KlaudiuszScene";

        if (SaveSystem.SaveExists())
        {
            overwriteSavePopup.Show(() => StartNewGame(startingScene));
            return;
        }

        StartNewGame(startingScene);
    }

    private void StartNewGame(string startingScene)
    {
        SaveSystem.Save(new SaveData { sceneName = startingScene });
        SceneManager.LoadScene(startingScene);
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