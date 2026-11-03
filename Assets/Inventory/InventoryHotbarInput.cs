using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryHotbarInput : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    public void OnInventorySlot1(InputAction.CallbackContext context) => SelectSlot(0, context);
    public void OnInventorySlot2(InputAction.CallbackContext context) => SelectSlot(1, context);
    public void OnInventorySlot3(InputAction.CallbackContext context) => SelectSlot(2, context);
    public void OnInventorySlot4(InputAction.CallbackContext context) => SelectSlot(3, context);
    public void OnInventorySlot5(InputAction.CallbackContext context) => SelectSlot(4, context);

    private void SelectSlot(int index, InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        inventory.SelectSlot(index);
    }
}
