using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Image[] slotImages;
    [SerializeField] private TMP_Text[] stackTexts;

    private void OnEnable()
    {
        Inventory.OnItemAdded += HandleItemAdded;
        Inventory.OnItemRemoved += HandleItemRemoved;
    }

    private void OnDisable()
    {
        Inventory.OnItemAdded -= HandleItemAdded;
        Inventory.OnItemRemoved -= HandleItemRemoved;
    }

    private void HandleItemAdded(int slotIndex, InventorySlot slot)
    {
        if (slotIndex < 0 || slotIndex >= slotImages.Length) return;

        slotImages[slotIndex].sprite = slot.Item.Icon;

        if (slotIndex < stackTexts.Length)
        {
            stackTexts[slotIndex].text = slot.StackCount > 1 ? slot.StackCount.ToString() : string.Empty;
        }
    }

    private void HandleItemRemoved(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slotImages.Length) return;

        slotImages[slotIndex].sprite = null;

        if (slotIndex < stackTexts.Length)
        {
            stackTexts[slotIndex].text = string.Empty;
        }
    }
}
