using System;
using UnityEngine;

// Umieszczany na graczu. Na zadanie (GameSaver.OnSaveRequested) zbiera aktualny
// stan gracza i odpowiada eventem OnPlayerDataCaptured - zamiast wystawiac sie
// jako referencja w Inspectorze GameSavera.
public class PlayerSaveDataProvider : MonoBehaviour
{
    [SerializeField] private CameraHandler cameraHandler;
    [SerializeField] private PlayerStamina stamina;

    public static event Action<PlayerSaveData> OnPlayerDataCaptured;

    private void OnEnable()
    {
        GameSaver.OnSaveRequested += HandleSaveRequested;
    }

    private void OnDisable()
    {
        GameSaver.OnSaveRequested -= HandleSaveRequested;
    }

    private void HandleSaveRequested()
    {
        OnPlayerDataCaptured?.Invoke(CapturePlayerData());
    }

    private PlayerSaveData CapturePlayerData()
    {
        return new PlayerSaveData
        {
            position = transform.position,
            rotation = transform.rotation,
            cameraPitch = cameraHandler != null ? cameraHandler.CameraPitch : 0f,
            currentStamina = stamina != null ? stamina.CurrentStamina : 0f
        };
    }
}
