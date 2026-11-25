using System;
using UnityEngine;

// Odpowiednik PlayerSaveDataProvider, ale dla ekwipunku. Nasluchuje
// GameSaver.OnSaveRequested, zbiera zawartosc wszystkich slotow Inventory
// i odpowiada eventem OnInventoryDataCaptured - bez referencji do GameSaver.
public class InventorySaveDataProvider : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    public static event Action<InventorySaveData> OnInventoryDataCaptured;

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
        OnInventoryDataCaptured?.Invoke(CaptureInventoryData());
    }

    private InventorySaveData CaptureInventoryData()
    {
        var slots = new InventorySlotSaveData[inventory.SlotCount];

        for (int i = 0; i < inventory.SlotCount; i++)
        {
            InventorySlot slot = inventory.GetSlot(i);

            slots[i] = slot == null
                ? new InventorySlotSaveData { itemName = null, stackCount = 0 }
                : new InventorySlotSaveData { itemName = slot.Item.ItemName, stackCount = slot.StackCount };
        }

        return new InventorySaveData { slots = slots };
    }
}
