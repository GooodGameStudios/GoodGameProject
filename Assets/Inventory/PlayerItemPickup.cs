using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerItemPickup : MonoBehaviour
{
    [SerializeField] private PlayerInteractor interactor;
    [SerializeField] private Inventory inventory;
    [SerializeField] private ItemDropSpawner dropSpawner;

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

    private void TryPickUpTarget()
    {
        if (interactor == null || interactor.CurrentTarget == null) return;

        WorldItem worldItem = interactor.CurrentTarget.GetComponent<WorldItem>();
        if (worldItem == null || worldItem.ItemData == null) return;

        int added = inventory.AddItem(worldItem.ItemData, worldItem.StackCount);
        int remaining = worldItem.StackCount - added;

        if (remaining <= 0)
        {
            Destroy(worldItem.gameObject);
            return;
        }

        if (inventory.ActiveSlot != null)
        {
            SwapActiveItem(worldItem, remaining);
            return;
        }

        worldItem.Initialize(worldItem.ItemData, remaining);
    }

    private void SwapActiveItem(WorldItem worldItem, int remainingStackCount)
    {
        InventorySlot heldSlot = inventory.ActiveSlot;
        IItemData pickedItem = worldItem.ItemData;

        int newStackCount = Mathf.Min(remainingStackCount, pickedItem.MaxStackSize);
        inventory.SetSlot(inventory.ActiveSlotIndex, pickedItem, newStackCount);

        dropSpawner.Spawn(heldSlot.Item, heldSlot.StackCount, interactor.InteractionOrigin);

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

    private void DropActiveItem()
    {
        if (inventory.ActiveSlotIndex < 0) return;

        InventorySlot slot = inventory.RemoveSlot(inventory.ActiveSlotIndex);
        if (slot == null) return;

        dropSpawner.Spawn(slot.Item, slot.StackCount, interactor.InteractionOrigin);
    }
}
