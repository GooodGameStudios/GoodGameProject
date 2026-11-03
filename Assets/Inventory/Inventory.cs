using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int slotCount = 5;

    private InventorySlot[] slots;
    private int activeSlotIndex = -1;

    public static event Action<int, InventorySlot> OnItemAdded;
    public static event Action<int> OnItemRemoved;
    public static event Action<int> OnActiveSlotSelected;

    public int SlotCount => slots.Length;
    public int ActiveSlotIndex => activeSlotIndex;
    public InventorySlot ActiveSlot => activeSlotIndex >= 0 ? slots[activeSlotIndex] : null;
    public InventorySlot GetSlot(int index) => slots[index];

    private void Awake()
    {
        slots = new InventorySlot[slotCount];
    }

    public void SelectSlot(int index)
    {
        activeSlotIndex = activeSlotIndex == index ? -1 : index;
        OnActiveSlotSelected?.Invoke(activeSlotIndex);
    }

    public int AddItem(IItemData item, int stackCount)
    {
        int remaining = stackCount;

        for (int i = 0; i < slots.Length && remaining > 0; i++)
        {
            if (slots[i] != null && slots[i].Item == item && slots[i].StackCount < item.MaxStackSize)
            {
                int added = Mathf.Min(remaining, item.MaxStackSize - slots[i].StackCount);
                slots[i].StackCount += added;
                remaining -= added;
                OnItemAdded?.Invoke(i, slots[i]);
            }
        }

        if (remaining > 0)
        {
            int freeSlotIndex = Array.FindIndex(slots, slot => slot == null);
            if (freeSlotIndex != -1)
            {
                int added = Mathf.Min(remaining, item.MaxStackSize);
                slots[freeSlotIndex] = new InventorySlot { Item = item, StackCount = added };
                remaining -= added;
                OnItemAdded?.Invoke(freeSlotIndex, slots[freeSlotIndex]);
            }
        }

        return stackCount - remaining;
    }

    public void SetSlot(int index, IItemData item, int stackCount)
    {
        OnItemRemoved?.Invoke(index);
        slots[index] = new InventorySlot { Item = item, StackCount = stackCount };
        OnItemAdded?.Invoke(index, slots[index]);
    }

    public InventorySlot RemoveSlot(int index)
    {
        InventorySlot slot = slots[index];
        if (slot == null) return null;

        slots[index] = null;
        OnItemRemoved?.Invoke(index);
        return slot;
    }
}
