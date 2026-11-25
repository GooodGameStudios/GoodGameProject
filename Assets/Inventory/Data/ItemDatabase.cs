using UnityEngine;

// Rejestr wszystkich przedmiotow w grze, potrzebny do odtworzenia referencji
// do ScriptableObject po ItemName przy wczytywaniu zapisu (JsonUtility nie
// przechowuje referencji do assetow - tylko ItemName, ktory jest unikalny
// dla kazdego przedmiotu). ItemId jest zarezerwowane na pozniej, pod
// przyszly system zapisu stanu swiata (ktore konkretne WorldItem zostaly
// juz podniesione) i nie jest tutaj uzywany.
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private ScriptableObject[] items;

    public IItemData GetByName(string itemName)
    {
        if (string.IsNullOrEmpty(itemName)) return null;

        foreach (ScriptableObject obj in items)
        {
            if (obj is IItemData item && item.ItemName == itemName)
            {
                return item;
            }
        }

        return null;
    }
}
