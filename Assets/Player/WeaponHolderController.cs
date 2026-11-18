using UnityEngine;

// Pokazuje model aktywnie wybranej broni/narzedzia w WeaponHolder - sockecie
// osadzonym w rigu rak (f_hands_01), ktory porusza sie wraz z animacjami.
// Dziala tylko dla przedmiotow typu WeaponData - inne przedmioty nie trafiaja do reki.
public class WeaponHolderController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform weaponHolder;

    private GameObject currentView;

    // Gdy przechodzimy na slot bez broni, model nie jest usuwany od razu -
    // czeka na zakonczenie animacji chowania (HidePickaxeStateBehaviour).
    private bool pendingClear;

    // Gdy wybieramy bron z pustej reki, model nie pojawia sie od razu -
    // czeka na start animacji wyciagania (PullPickaxeStateBehaviour).
    private GameObject pendingWeaponPrefab;

    private void OnEnable()
    {
        Inventory.OnActiveSlotSelected += HandleActiveSlotSelected;
        Inventory.OnItemAdded += HandleItemAdded;
        Inventory.OnItemRemoved += HandleItemRemoved;
        HidePickaxeStateBehaviour.OnHidePickaxeFinished += ClearPendingView;
        PullPickaxeStateBehaviour.OnPullPickaxeStarted += HandlePullPickaxeStarted;
        PlayerItemPickup.OnHeldItemDropped += ClearPendingView;
    }

    private void OnDisable()
    {
        Inventory.OnActiveSlotSelected -= HandleActiveSlotSelected;
        Inventory.OnItemAdded -= HandleItemAdded;
        Inventory.OnItemRemoved -= HandleItemRemoved;
        HidePickaxeStateBehaviour.OnHidePickaxeFinished -= ClearPendingView;
        PullPickaxeStateBehaviour.OnPullPickaxeStarted -= HandlePullPickaxeStarted;
        PlayerItemPickup.OnHeldItemDropped -= ClearPendingView;
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
        InventorySlot slot = inventory.ActiveSlot;
        if (slot?.Item is not WeaponData weapon || weapon.Prefab == null)
        {
            // Nic nowego do pokazania - odwolujemy ewentualne czekajace pojawienie sie
            // broni, a jesli w reku juz byl model, usuwamy go dopiero gdy animacja
            // chowania pickaxe faktycznie sie zakonczy.
            pendingWeaponPrefab = null;
            pendingClear = currentView != null;
            return;
        }

        pendingClear = false;

        // Rece juz trzymaja jakas bron - dla samej zamiany nie ma animacji wyciagania,
        // wiec podmieniamy model natychmiast.
        if (currentView != null)
        {
            ShowWeapon(weapon.Prefab);
            return;
        }

        // Rece sa puste - model pojawi sie dopiero na starcie animacji wyciagania
        // (PullPickaxeStateBehaviour), nie od razu po wybraniu slotu na hotbarze.
        pendingWeaponPrefab = weapon.Prefab;
    }

    // Uruchamiane, gdy animacja "pulling_out_pickaxe" faktycznie sie zaczyna (PullPickaxeStateBehaviour).
    private void HandlePullPickaxeStarted()
    {
        if (pendingWeaponPrefab == null) return;

        ShowWeapon(pendingWeaponPrefab);
    }

    // Usuwa czekajacy model, gdy animacja "hiding_pickaxe" dobiegnie konca (HidePickaxeStateBehaviour)
    // albo gdy trzymany przedmiot zostaje fizycznie wyrzucony w swiat (PlayerItemPickup.OnHeldItemDropped) -
    // w drugim przypadku bez tego model byby widoczny jednoczesnie w reku i juz lezacy/lecacy w swiecie.
    private void ClearPendingView()
    {
        if (!pendingClear) return;

        ClearCurrentView();
        pendingClear = false;
    }

    private void ShowWeapon(GameObject prefab)
    {
        ClearCurrentView();
        pendingWeaponPrefab = null;

        currentView = WorldItemPoolManager.Instance.Get(prefab);
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
