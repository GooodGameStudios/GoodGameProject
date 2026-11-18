using UnityEngine;

// Pokazuje model aktywnie wybranej broni/narzedzia w WeaponHolder - sockecie
// osadzonym w rigu rak (f_hands_01), ktory porusza sie wraz z animacjami.
// Dziala tylko dla przedmiotow typu WeaponData - inne przedmioty nie trafiaja do reki.
public class WeaponHolderController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform weaponHolder;

    private GameObject currentView;

    private void OnEnable()
    {
        Inventory.OnActiveSlotSelected += HandleActiveSlotSelected;
        Inventory.OnItemAdded += HandleItemAdded;
        Inventory.OnItemRemoved += HandleItemRemoved;
    }

    private void OnDisable()
    {
        Inventory.OnActiveSlotSelected -= HandleActiveSlotSelected;
        Inventory.OnItemAdded -= HandleItemAdded;
        Inventory.OnItemRemoved -= HandleItemRemoved;
    }

    private void HandleActiveSlotSelected(int slotIndex) => RefreshView();

    // Lapie przypadek podmiany zawartosci aktywnego slotu (np. podniesienie
    // nowej broni z ziemi w miejsce trzymanej), bez zmiany samego indeksu slotu.
    private void HandleItemAdded(int slotIndex, InventorySlot slot)
    {
        if (slotIndex == inventory.ActiveSlotIndex) RefreshView();
    }

    private void HandleItemRemoved(int slotIndex)
    {
        if (slotIndex == inventory.ActiveSlotIndex) RefreshView();
    }

    private void RefreshView()
    {
        ClearCurrentView();

        InventorySlot slot = inventory.ActiveSlot;
        if (slot?.Item is not WeaponData weapon || weapon.Prefab == null) return;

        currentView = WorldItemPoolManager.Instance.Get(weapon.Prefab);
        currentView.transform.SetParent(weaponHolder);
        currentView.transform.localPosition = Vector3.zero;
        currentView.transform.localRotation = Quaternion.identity;
    }

    private void ClearCurrentView()
    {
        if (currentView == null) return;

        WorldItemPoolManager.Instance.Release(currentView);
        currentView = null;
    }
}
