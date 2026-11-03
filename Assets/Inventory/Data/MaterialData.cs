using UnityEngine;

// Dane przedmiotu, ktory mozna stackowac w jednym slocie (np. krystal, drewno).
[CreateAssetMenu(fileName = "NewMaterial", menuName = "Inventory/Material Data")]
public class MaterialData : ScriptableObject, IItemData
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;
    [SerializeField] private int maxStackSize = 1;
    [SerializeField] private GameObject prefab;

    public string ItemName => itemName;
    public Sprite Icon => icon;
    public int MaxStackSize => maxStackSize;
    public GameObject Prefab => prefab;
}
