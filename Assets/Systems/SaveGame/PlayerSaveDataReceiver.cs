using UnityEngine;

// Umieszczany na graczu. Nasluchuje SaveDataLoader.OnSaveDataLoaded i przywraca
// pozycje, obrot, pochylenie kamery oraz stamine zapisane w PlayerSaveData.
public class PlayerSaveDataReceiver : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private CameraHandler cameraHandler;
    [SerializeField] private PlayerStamina stamina;

    private void OnEnable()
    {
        SaveDataLoader.OnSaveDataLoaded += HandleSaveDataLoaded;
    }

    private void OnDisable()
    {
        SaveDataLoader.OnSaveDataLoaded -= HandleSaveDataLoaded;
    }

    private void HandleSaveDataLoaded(SaveData data)
    {
        ApplyPlayerData(data.player);
    }

    private void ApplyPlayerData(PlayerSaveData playerData)
    {
        // CharacterController trzyma wlasny wewnetrzny stan ruchu - wylaczamy go
        // na czas recznego ustawienia pozycji, zeby nie "wypchnal" gracza z powrotem.
        if (controller != null) controller.enabled = false;

        transform.SetPositionAndRotation(playerData.position, playerData.rotation);

        if (controller != null) controller.enabled = true;

        if (cameraHandler != null)
        {
            cameraHandler.SetCameraPitch(playerData.cameraPitch);
        }

        if (stamina != null)
        {
            stamina.SetStamina(playerData.currentStamina);
        }
    }
}
