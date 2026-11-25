using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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
            // TODO: zanim nadpiszemy istniejacy zapis, pokazac tutaj popup
            // z potwierdzeniem ("czy na pewno chcesz nadpisac zapis?").
        }

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