using UnityEngine;

// Dane przedmiotu typu bron.
[CreateAssetMenu(fileName = "NewWeapon", menuName = "Inventory/Weapon Data")]
public class WeaponData : ScriptableObject, IItemData
{
    [SerializeField] private string itemId;
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float knockback = 5f;
    [SerializeField] private float maxDurability = 100f;
    [SerializeField] private GameObject prefab;

    public string ItemId => itemId;
    public string ItemName => itemName;
    public Sprite Icon => icon;
    public float Damage => damage;
    public float Knockback => knockback;
    public float MaxDurability => maxDurability;
    public GameObject Prefab => prefab;
    // Bron zawsze zajmuje caly slot - nie da sie jej stackowac.
    public int MaxStackSize => 1;
}
