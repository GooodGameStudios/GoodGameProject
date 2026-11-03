using UnityEngine;

// Reprezentuje przedmiot lezacy w swiecie gry, ktory gracz moze podniesc.
public class WorldItem : MonoBehaviour
{

    [SerializeField] private ScriptableObject itemDataAsset;
    [SerializeField] private int stackCount = 1;


    public IItemData ItemData => itemDataAsset as IItemData;

    public int StackCount => stackCount;

    // Ustawia dane przedmiotu i liczbe sztuk. Uzywane przy spawnowaniu/upuszczaniu
    // przedmiotu w swiecie oraz przy aktualizacji resztek stacku na ziemi.
    public void Initialize(IItemData item, int count)
    {
        itemDataAsset = item as ScriptableObject;
        stackCount = count;
    }
}
