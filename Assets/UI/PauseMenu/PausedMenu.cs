using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject[] windowsToHide;

    private bool isMenuOpen = false;
    private bool[] previousWindowStates;
    public bool IsMenuOpen => isMenuOpen;

    private void Start()
    {
        pauseMenuUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if (isMenuOpen)
            ResumeGame();
        else
            OpenPauseMenu();
    }

    private void OpenPauseMenu()
    {
        isMenuOpen = true;
        previousWindowStates = new bool[windowsToHide.Length];

        for (int i = 0; i < windowsToHide.Length; i++)
        {
            if (windowsToHide[i] == null) continue;
            previousWindowStates[i] = windowsToHide[i].activeSelf;
            windowsToHide[i].SetActive(false);
        }
        pauseMenuUI.SetActive(true);
        UpdateCursor();
    }



    public void ResumeGame()
    {
        isMenuOpen = false;
        pauseMenuUI.SetActive(false);
        for (int i = 0; i < windowsToHide.Length; i++)
        {
            if (windowsToHide[i] == null) continue;
            windowsToHide[i].SetActive(previousWindowStates[i]);
        }
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        Cursor.visible = isMenuOpen;
        Cursor.lockState = isMenuOpen
            ? CursorLockMode.None
            : CursorLockMode.Locked;
    }



    public void LoadLastSave()
    {
        Debug.Log("LoadSave");
    }

    public void Settings()
    {
        Debug.Log("Settings");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}