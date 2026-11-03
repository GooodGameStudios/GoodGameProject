using UnityEngine;

public class ItemDropSpawner : MonoBehaviour
{
    [SerializeField] private float throwForce = 5f;
    [SerializeField] private float dropDistance = 1f;

    public void Spawn(IItemData item, int stackCount, Transform origin)
    {
        if (item.Prefab == null) return;

        Vector3 spawnPosition = origin.position + origin.forward * dropDistance;
        GameObject dropped = Instantiate(item.Prefab, spawnPosition, origin.rotation);

        WorldItem worldItem = dropped.GetComponentInChildren<WorldItem>();
        if (worldItem != null)
        {
            worldItem.Initialize(item, stackCount);
        }

        Rigidbody rb = dropped.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = dropped.AddComponent<Rigidbody>();
        }

        rb.AddForce(origin.forward * throwForce, ForceMode.Impulse);
    }
}
