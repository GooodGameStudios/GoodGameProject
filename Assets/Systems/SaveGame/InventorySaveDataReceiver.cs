using UnityEngine;

// Odpowiednik PlayerSaveDataReceiver, ale dla ekwipunku. Nasluchuje
// SaveDataLoader.OnSaveDataLoaded i odtwarza sloty Inventory, rozwiazujac
// zapisana ItemName na konkretny IItemData przez ItemDatabase.
public class InventorySaveDataReceiver : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private ItemDatabase itemDatabase;

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
        ApplyInventoryData(data.inventory);
    }

    private void ApplyInventoryData(InventorySaveData inventoryData)
    {
        if (inventoryData?.slots == null) return;

        int slotsToRestore = Mathf.Min(inventoryData.slots.Length, inventory.SlotCount);

        for (int i = 0; i < slotsToRestore; i++)
        {
            InventorySlotSaveData slotData = inventoryData.slots[i];

            if (string.IsNullOrEmpty(slotData.itemName))
            {
                inventory.RemoveSlot(i);
                continue;
            }

            IItemData item = itemDatabase.GetByName(slotData.itemName);
            if (item == null)
            {
                Debug.LogWarning($"Nie znaleziono przedmiotu o nazwie '{slotData.itemName}' w ItemDatabase - slot {i} zostanie pominiety.");
                continue;
            }

            inventory.SetSlot(i, item, slotData.stackCount);
        }
    }
}
