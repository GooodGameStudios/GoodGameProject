using UnityEngine;

// Korzen danych zapisu - wszystko, co trzeba zachowac miedzy sesjami gry,
// trafia tutaj jako kolejne pola (np. inventory, stan swiata) w kolejnych krokach.
[System.Serializable]
public class SaveData
{
    // Nazwa sceny, w ktorej gracz byl w momencie zapisu (SceneManager.GetActiveScene().name).
    public string sceneName;

    public PlayerSaveData player = new PlayerSaveData();

    public InventorySaveData inventory = new InventorySaveData();
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

// Zawartosc calego ekwipunku - jeden wpis na slot, w tej samej kolejnosci co Inventory.GetSlot.
[System.Serializable]
public class InventorySaveData
{
    public InventorySlotSaveData[] slots = new InventorySlotSaveData[0];
}

// Zawartosc jednego slotu. itemName == null/puste oznacza pusty slot.
[System.Serializable]
public class InventorySlotSaveData
{
    public string itemName;
    public int stackCount;
}
