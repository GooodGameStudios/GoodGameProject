using UnityEngine;
using UnityEngine.Pool;

[DisallowMultipleComponent]
public class PooledObject : MonoBehaviour
{
    public GameObject Prefab { get; internal set; }
    internal ObjectPool<GameObject> Pool { get; set; }
}
