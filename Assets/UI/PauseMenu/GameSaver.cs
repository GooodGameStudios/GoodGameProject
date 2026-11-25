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
        InventorySaveDataProvider.OnInventoryDataCaptured += HandleInventoryDataCaptured;
    }

    private void OnDisable()
    {
        PlayerSaveDataProvider.OnPlayerDataCaptured -= HandlePlayerDataCaptured;
        InventorySaveDataProvider.OnInventoryDataCaptured -= HandleInventoryDataCaptured;
    }

    // Wypelniane synchronicznie w ramach OnSaveRequested?.Invoke() w SaveGame -
    // kazdy provider odpowiada wlasnymi danymi, zanim zapis trafi na dysk.
    private PlayerSaveData capturedPlayerData;
    private InventorySaveData capturedInventoryData;
    private bool playerDataWasCaptured;

    // Podpinane pod przycisk "Save" w UI.
    public void SaveGame()
    {
        playerDataWasCaptured = false;
        capturedInventoryData = null;

        OnSaveRequested?.Invoke();

        if (!playerDataWasCaptured)
        {
            Debug.LogWarning("Nikt nie odpowiedzial na zadanie zapisu - sprawdz, czy PlayerSaveDataProvider jest obecny i aktywny w scenie.");
            return;
        }

        SaveData data = new SaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            player = capturedPlayerData,
            // InventorySaveDataProvider jest opcjonalny - jego brak w scenie nie blokuje zapisu.
            inventory = capturedInventoryData ?? new InventorySaveData()
        };

        SaveSystem.Save(data);
    }

    private void HandlePlayerDataCaptured(PlayerSaveData playerData)
    {
        playerDataWasCaptured = true;
        capturedPlayerData = playerData;
    }

    private void HandleInventoryDataCaptured(InventorySaveData inventoryData)
    {
        capturedInventoryData = inventoryData;
    }
}
