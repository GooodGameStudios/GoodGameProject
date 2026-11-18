using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject[] windowsToHide;

    [SerializeField] private InputActionAsset inputActions;

    private InputAction pauseAction;

    private bool isMenuOpen = false;
    private bool[] previousWindowStates;

    public bool IsMenuOpen => isMenuOpen;

    private void Awake()
    {
        pauseAction = inputActions.FindActionMap("PlayerMenus").FindAction("MainMenu");
    }

    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.performed += OnPause;
            pauseAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.performed -= OnPause;
            pauseAction.Disable();
        }
    }

    private void Start()
    {
        pauseMenuUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        TogglePauseMenu();
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