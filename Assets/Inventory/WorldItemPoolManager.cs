using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[DefaultExecutionOrder(-1000)]
public class WorldItemPoolManager : MonoBehaviour
{
    private static WorldItemPoolManager instance;

    public static WorldItemPoolManager Instance
    {
        get
        {
            if (instance == null)
            {
                var ownerObject = new GameObject(nameof(WorldItemPoolManager));
                instance = ownerObject.AddComponent<WorldItemPoolManager>();
            }

            return instance;
        }
    }

    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxSize = 50;

    private readonly Dictionary<GameObject, ObjectPool<GameObject>> pools = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    public GameObject Get(GameObject prefab)
    {
        ObjectPool<GameObject> pool = GetOrCreatePool(prefab);
        return pool.Get();
    }

    public void Release(GameObject instanceToRelease)
    {
        if (instanceToRelease == null) return;

        PooledObject pooledObject = instanceToRelease.GetComponentInParent<PooledObject>();
        if (pooledObject == null || pooledObject.Pool == null)
        {
            Destroy(instanceToRelease);
            return;
        }

        pooledObject.Pool.Release(pooledObject.gameObject);
    }

    public void RegisterExisting(GameObject root, GameObject prefab)
    {
        if (root == null || prefab == null) return;

        PooledObject pooledObject = root.GetComponent<PooledObject>();
        if (pooledObject != null && pooledObject.Pool != null) return;

        if (pooledObject == null)
        {
            pooledObject = root.AddComponent<PooledObject>();
        }

        pooledObject.Prefab = prefab;
        pooledObject.Pool = GetOrCreatePool(prefab);
    }

    private ObjectPool<GameObject> GetOrCreatePool(GameObject prefab)
    {
        if (pools.TryGetValue(prefab, out ObjectPool<GameObject> pool)) return pool;

        pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(prefab),
            actionOnGet: OnGetFromPool,
            actionOnRelease: OnReleaseToPool,
            actionOnDestroy: Destroy,
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize);

        pools[prefab] = pool;
        return pool;
    }

    private static void OnGetFromPool(GameObject instance)
    {
        instance.SetActive(true);

        if (instance.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void OnReleaseToPool(GameObject instance)
    {
        instance.SetActive(false);
        instance.transform.SetParent(transform);
    }
}
