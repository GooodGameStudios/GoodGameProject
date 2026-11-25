using UnityEngine;

// Korzen danych zapisu - wszystko, co trzeba zachowac miedzy sesjami gry,
// trafia tutaj jako kolejne pola (np. inventory, stan swiata) w kolejnych krokach.
[System.Serializable]
public class SaveData
{
    // Nazwa sceny, w ktorej gracz byl w momencie zapisu (SceneManager.GetActiveScene().name).
    public string sceneName;

    public PlayerSaveData player = new PlayerSaveData();
}

// Dane gracza potrzebne do przywrocenia jego stanu po wczytaniu zapisu.
[System.Serializable]
public class PlayerSaveData
{
    public Vector3 position;

    // Obrot korpusu gracza (yaw) - transform.rotation z PlayerMovement/CameraHandler.
    public Quaternion rotation;

    // Pochylenie kamery gora/dol - odpowiednik CameraHandler.cameraRotationX.
    public float cameraPitch;

    public float currentStamina;
}
