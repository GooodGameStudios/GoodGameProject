using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private PlayerInteractor interactor;
    [SerializeField] private int slotCount = 5;
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float dropDistance = 1f;

    private InventorySlot[] slots;
    private int activeSlotIndex = -1;

    public static event Action<int, InventorySlot> OnItemAdded;
    public static event Action<int> OnActiveSlotSelected;
    public static event Action<int> OnItemRemoved;

    public int SlotCount => slots.Length;
    public InventorySlot GetSlot(int index) => slots[index];

    private void Awake()
    {
        slots = new InventorySlot[slotCount];
    }

    public void OnPickUpItem(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        TryPickUpTarget();
    }

    public void OnDropItem(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        DropActiveItem();
    }

    public void OnInventorySlot1(InputAction.CallbackContext context) => SelectSlot(0, context);
    public void OnInventorySlot2(InputAction.CallbackContext context) => SelectSlot(1, context);
    public void OnInventorySlot3(InputAction.CallbackContext context) => SelectSlot(2, context);
    public void OnInventorySlot4(InputAction.CallbackContext context) => SelectSlot(3, context);
    public void OnInventorySlot5(InputAction.CallbackContext context) => SelectSlot(4, context);

    private void SelectSlot(int index, InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        activeSlotIndex = activeSlotIndex == index ? -1 : index;
        OnActiveSlotSelected?.Invoke(activeSlotIndex);
    }

    private void TryPickUpTarget()
    {
        if (interactor == null || interactor.CurrentTarget == null) return;

        WorldItem worldItem = interactor.CurrentTarget.GetComponent<WorldItem>();
        if (worldItem == null || worldItem.ItemData == null) return;

        int added = AddItem(worldItem.ItemData, worldItem.StackCount);
        int remaining = worldItem.StackCount - added;

        if (remaining <= 0)
        {
            Destroy(worldItem.gameObject);
            return;
        }

        if (activeSlotIndex >= 0 && slots[activeSlotIndex] != null)
        {
            SwapActiveItem(worldItem, remaining);
            return;
        }

        worldItem.Initialize(worldItem.ItemData, remaining);
    }

    private void SwapActiveItem(WorldItem worldItem, int remainingStackCount)
    {
        InventorySlot heldSlot = slots[activeSlotIndex];
        IItemData pickedItem = worldItem.ItemData;

        OnItemRemoved?.Invoke(activeSlotIndex);

        int newStackCount = Mathf.Min(remainingStackCount, pickedItem.MaxStackSize);
        slots[activeSlotIndex] = new InventorySlot { Item = pickedItem, StackCount = newStackCount };
        OnItemAdded?.Invoke(activeSlotIndex, slots[activeSlotIndex]);

        SpawnDroppedItem(heldSlot.Item, heldSlot.StackCount);

        int leftover = remainingStackCount - newStackCount;
        if (leftover <= 0)
        {
            Destroy(worldItem.gameObject);
        }
        else
        {
            worldItem.Initialize(pickedItem, leftover);
        }
    }

    private int AddItem(IItemData item, int stackCount)
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

    private void DropActiveItem()
    {
        if (activeSlotIndex < 0) return;

        InventorySlot slot = slots[activeSlotIndex];
        if (slot == null) return;

        slots[activeSlotIndex] = null;
        OnItemRemoved?.Invoke(activeSlotIndex);

        SpawnDroppedItem(slot.Item, slot.StackCount);
    }

    private void SpawnDroppedItem(IItemData item, int stackCount)
    {
        if (item.Prefab == null) return;

        Transform origin = interactor != null ? interactor.InteractionOrigin : transform;
        Vector3 spawnPosition = origin.position + origin.forward * dropDistance;

        GameObject dropped = Instantiate(item.Prefab, spawnPosition, origin.rotation);

        WorldItem worldItem = dropped.GetComponentInChildren<WorldItem>();
        if (worldItem != null)
        {
            worldItem.Initialize(item, stackCount);
        }

        Rigidbody rb = dropped.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = dropped.AddComponent<Rigidbody>();
        }

        rb.AddForce(origin.forward * throwForce, ForceMode.Impulse);
    }
}
