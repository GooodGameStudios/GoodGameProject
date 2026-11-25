using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Zapisuje aktualny stan gry (scena + gracz) do jedynego zapisu na dysku.
// Podpinany pod przyciski UI (np. "Save" w menu pauzy). Nie trzyma zadnej
// referencji do gracza - wysyla tylko zadanie zapisu eventem i czeka na
// odpowiedz z danymi od PlayerSaveDataProvider (patrz HandlePlayerDataCaptured).
public class GameSaver : MonoBehaviour
{
    // Wysylane po wcisnieciu przycisku "Save" - sygnal, ze gracz chce zapisac gre.
    public static event Action OnSaveRequested;

    private void OnEnable()
    {
        PlayerSaveDataProvider.OnPlayerDataCaptured += HandlePlayerDataCaptured;
    }

    private void OnDisable()
    {
        PlayerSaveDataProvider.OnPlayerDataCaptured -= HandlePlayerDataCaptured;
    }

    // Ustawiane w HandlePlayerDataCaptured - pozwala wykryc, czy ktokolwiek
    // odpowiedzial na OnSaveRequested (patrz ostrzezenie w SaveGame).
    private bool requestWasHandled;

    // Podpinane pod przycisk "Save" w UI.
    public void SaveGame()
    {
        requestWasHandled = false;

        OnSaveRequested?.Invoke();

        if (!requestWasHandled)
        {
            Debug.LogWarning("Nikt nie odpowiedzial na zadanie zapisu - sprawdz, czy PlayerSaveDataProvider jest obecny i aktywny w scenie.");
        }
    }

    // Wywolywane synchronicznie w ramach OnSaveRequested?.Invoke() powyzej -
    // PlayerSaveDataProvider odpowiada na zadanie wlasnymi danymi.
    private void HandlePlayerDataCaptured(PlayerSaveData playerData)
    {
        requestWasHandled = true;

        SaveData data = new SaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            player = playerData
        };

        SaveSystem.Save(data);
    }
}
