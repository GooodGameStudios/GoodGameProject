using System;
using UnityEngine;

// Przechowuje sloty, aktywny slot i wysyla zdarzenia,
// na podstawie ktorych UI oraz widok przedmiotu w rekach
// aktualizuja swoj wyglad. 
public class Inventory : MonoBehaviour
{
    [SerializeField] private int slotCount = 5;

    private InventorySlot[] slots;
    private int activeSlotIndex = -1;

    // Wywolywane gdy w danym slocie pojawia sie przedmiot albo zmienia sie liczba sztuk w stacku.
    public static event Action<int, InventorySlot> OnItemAdded;

    // Wywolywane gdy dany slot zostaje wyczyszczony (np. po wyrzuceniu przedmiotu).
    public static event Action<int> OnItemRemoved;

    // Wywolywane po zmianie aktywnego (trzymanego w rece) slotu.
    public static event Action<int> OnActiveSlotSelected;

    // Liczba wszystkich slotow w ekwipunku.
    public int SlotCount => slots.Length;

    public int ActiveSlotIndex => activeSlotIndex;

    public InventorySlot ActiveSlot => activeSlotIndex >= 0 ? slots[activeSlotIndex] : null;

    // Zwraca zawartosc slotu o podanym indeksie (moze byc null, jesli slot jest pusty).
    public InventorySlot GetSlot(int index) => slots[index];

    private void Awake()
    {
        slots = new InventorySlot[slotCount];
    }

    // Aktywacja wybranego slotu oraz jego dezaktywacja przy ponownym nacisnieciu klawisza
    public void SelectSlot(int index)
    {
        activeSlotIndex = activeSlotIndex == index ? -1 : index;
        OnActiveSlotSelected?.Invoke(activeSlotIndex);
    }

    // Dodanie przedmiotu do ekwipunku.
    public int AddItem(IItemData item, int stackCount)
    {
        int remaining = stackCount;

        // Dolozenie do slotow, ktore juz maja ten sam przedmiot i nie sa pelne.
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

        // Jesli cos zostalo, wstawienie reszty do pierwszego wolnego slotu.
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

        // Ilosc pozostalego przedmiotu, ktore nie zmiescily sie w inventory.
        return stackCount - remaining;
    }

    // Podmiana trzymanego przedmiotu w reku na ten z ziemi.
    public void SetSlot(int index, IItemData item, int stackCount)
    {
        OnItemRemoved?.Invoke(index);
        slots[index] = new InventorySlot { Item = item, StackCount = stackCount };
        OnItemAdded?.Invoke(index, slots[index]);
    }

    // Wyrzucenie przedmiotu z ekwipunku.
    public InventorySlot RemoveSlot(int index)
    {
        InventorySlot slot = slots[index];
        if (slot == null) return null;

        slots[index] = null;
        OnItemRemoved?.Invoke(index);
        return slot;
    }
}
