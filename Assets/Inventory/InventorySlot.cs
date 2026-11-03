// Pojedynczy slot ekwipunku - przechowuje jaki przedmiot w nim jest
// i ile sztuk tego przedmiotu znajduje sie w stacku.
public class InventorySlot
{
    public IItemData Item { get; set; }
    public int StackCount { get; set; }
}
