using System.Collections.Generic;
using UnityEngine;

// Pokazuje model 3D przedmiotu trzymanego w rekach gracza, na podstawie
// zdarzen wysylanych przez Inventory. Dla kazdego slotu, ktory kiedykolwiek
// mial przedmiot, tworzy jeden model i przelacza jego widocznosc.
public class ItemViewController : MonoBehaviour
{
    [SerializeField] private Transform viewpoint;

    // Model 3D przypisany do kazdego slotu, ktory mial juz jakis przedmiot.
    private readonly Dictionary<int, GameObject> slotViews = new();
    private int activeSlotIndex = -1;

    private void OnEnable()
    {
        Inventory.OnItemAdded += HandleItemAdded;
        Inventory.OnActiveSlotSelected += HandleActiveSlotSelected;
        Inventory.OnItemRemoved += HandleItemRemoved;
    }

    private void OnDisable()
    {
        Inventory.OnItemAdded -= HandleItemAdded;
        Inventory.OnActiveSlotSelected -= HandleActiveSlotSelected;
        Inventory.OnItemRemoved -= HandleItemRemoved;
    }

    // Gdy w slocie pojawia sie przedmiot po raz pierwszy, tworzy jego model
    // w punkcie viewpoint i wlacza go tylko wtedy, gdy to jest aktywny slot.
    private void HandleItemAdded(int slotIndex, InventorySlot slot)
    {
        if (slotViews.ContainsKey(slotIndex)) return;
        if (slot.Item.Prefab == null) return;

        GameObject view = Instantiate(slot.Item.Prefab, viewpoint);
        view.transform.localPosition = Vector3.zero;
        view.transform.localRotation = Quaternion.identity;
        view.SetActive(slotIndex == activeSlotIndex);

        slotViews[slotIndex] = view;
    }

    // Przy zmianie aktywnego slotu chowa model poprzednio trzymanego przedmiotu
    // i pokazuje model nowo wybranego przedmiotu.
    private void HandleActiveSlotSelected(int slotIndex)
    {
        if (slotViews.TryGetValue(activeSlotIndex, out GameObject previousView) && previousView != null)
        {
            previousView.SetActive(false);
        }

        activeSlotIndex = slotIndex;

        if (slotViews.TryGetValue(activeSlotIndex, out GameObject newView) && newView != null)
        {
            newView.SetActive(true);
        }
    }

    // Gdy slot zostaje oprozniony (upuszczenie/podmiana przedmiotu), usuwa jego model.
    private void HandleItemRemoved(int slotIndex)
    {
        if (!slotViews.TryGetValue(slotIndex, out GameObject view)) return;

        Destroy(view);
        slotViews.Remove(slotIndex);
    }
}
