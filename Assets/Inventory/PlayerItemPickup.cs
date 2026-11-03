using UnityEngine;
using UnityEngine.InputSystem;

// Laczy interaktor gracza (co widzi gracz) z ekwipunkiem (Inventory) i spawnerem
// przedmiotow (ItemDropSpawner). Caly przeplyw podnoszenia, podmiany trzymanego
// przedmiotu i upuszczania przedmiotu jest obslugiwany tutaj.
public class PlayerItemPickup : MonoBehaviour
{
    [SerializeField] private PlayerInteractor interactor;
    [SerializeField] private Inventory inventory;
    [SerializeField] private ItemDropSpawner dropSpawner;

    // Callback z Input Systemu - wywolywany po wcisnieciu klawisza podniesienia przedmiotu.
    public void OnPickUpItem(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        TryPickUpTarget();
    }

    // Callback z Input Systemu - wywolywany po wcisnieciu klawisza upuszczenia przedmiotu.
    public void OnDropItem(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        DropActiveItem();
    }

    // Sprawdza, na co aktualnie patrzy gracz (interactor.CurrentTarget), i probuje
    // dodac ten przedmiot do ekwipunku. Jesli przedmiot nie zmiesci sie w calosci
    // (ekwipunek pelny) a gracz trzyma w rece cos innego, nastepuje podmiana -
    // trzymany przedmiot leci na ziemie, a podnoszony trafia na jego miejsce.
    private void TryPickUpTarget()
    {
        if (interactor == null || interactor.CurrentTarget == null) return;

        WorldItem worldItem = interactor.CurrentTarget.GetComponent<WorldItem>();
        if (worldItem == null || worldItem.ItemData == null) return;

        IItemData pickedItem = worldItem.ItemData;
        int added = inventory.AddItem(pickedItem, worldItem.StackCount);
        int remaining = worldItem.StackCount - added;

        // Caly stack zostal dodany do ekwipunku - przedmiot ze swiata mozna usunac.
        if (remaining <= 0)
        {
            Destroy(worldItem.gameObject);
            return;
        }

        // Nie wszystko zmiescilo sie w ekwipunku. Swap ma sens tylko gdy gracz trzyma
        // w rece INNY przedmiot - jesli trzyma ten sam, AddItem juz zmergowal co sie dalo
        // (w tym do trzymanego slotu), a reszta po prostu zostaje na ziemi (patrz nizej).
        if (inventory.ActiveSlot != null && inventory.ActiveSlot.Item != pickedItem)
        {
            SwapActiveItem(worldItem, remaining);
            return;
        }

        // Gracz nic nie trzyma w rece albo trzyma ten sam przedmiot (juz zmergowany
        // do maksimum) - reszta, ktora sie nie zmiescila, zostaje na ziemi.
        worldItem.Initialize(pickedItem, remaining);
    }

    // Podmienia przedmiot z aktywnego (trzymanego w rece) slotu na przedmiot ze swiata.
    // remainingStackCount to ta czesc podnoszonego przedmiotu, ktora nie zmiescila sie
    // normalnym AddItem i dlatego wymusila podmiane.
    private void SwapActiveItem(WorldItem worldItem, int remainingStackCount)
    {
        InventorySlot heldSlot = inventory.ActiveSlot;
        IItemData pickedItem = worldItem.ItemData;

        // Nowy stack w slocie nie moze przekroczyc maksymalnego rozmiaru stacku przedmiotu.
        int newStackCount = Mathf.Min(remainingStackCount, pickedItem.MaxStackSize);
        inventory.SetSlot(inventory.ActiveSlotIndex, pickedItem, newStackCount);

        // Dotychczas trzymany przedmiot zostaje wyrzucony w swiat.
        dropSpawner.Spawn(heldSlot.Item, heldSlot.StackCount, interactor.InteractionOrigin);

        // Jesli po wypelnieniu nowego stacku cos jeszcze zostalo (nadmiar ponad max stack),
        // ta reszta zostaje na ziemi w zmniejszonym obiekcie swiata.
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

    // Usuwa przedmiot z aktywnego slotu i wyrzuca go do swiata gry.
    private void DropActiveItem()
    {
        if (inventory.ActiveSlotIndex < 0) return;

        InventorySlot slot = inventory.RemoveSlot(inventory.ActiveSlotIndex);
        if (slot == null) return;

        dropSpawner.Spawn(slot.Item, slot.StackCount, interactor.InteractionOrigin);
    }
}
