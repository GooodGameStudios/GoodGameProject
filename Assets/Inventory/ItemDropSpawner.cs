using UnityEngine;

// Odpowiada za fizyczne umieszczenie przedmiotu w swiecie gry

public class ItemDropSpawner : MonoBehaviour
{
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float dropDistance = 1f;

    // Tworzy prefab przedmiotu przed danym punktem (origin), ustawia jego dane
    // (itemData/stackCount) i rzuca nim do przodu sila throwForce.
    public void Spawn(IItemData item, int stackCount, Transform origin)
    {
        if (item.Prefab == null) return;

        Vector3 spawnPosition = origin.position + origin.forward * dropDistance;
        GameObject dropped = Instantiate(item.Prefab, spawnPosition, origin.rotation);

        // Przekazanie informacji o przedmiocie i liczbie sztuk do nowo utworzonego obiektu.
        WorldItem worldItem = dropped.GetComponentInChildren<WorldItem>();
        if (worldItem != null)
        {
            worldItem.Initialize(item, stackCount);
        }

        // Przedmiot potrzebuje Rigidbody, aby mozna go bylo rzucic fizyka.
        Rigidbody rb = dropped.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = dropped.AddComponent<Rigidbody>();
        }

        rb.AddForce(origin.forward * throwForce, ForceMode.Impulse);
    }
}
