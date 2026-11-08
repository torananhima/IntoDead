using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    [Header("Pool Configuration")]
    [SerializeField] private List<ObjectPoolConfig> poolConfigs = new();

    private readonly Dictionary<GameObject, PoolEntry> pools = new();
    private readonly Dictionary<GameObject, PoolEntry> instanceToPool = new();

    public static ObjectPoolManager Instance;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var config in poolConfigs)
        {
            if (config.prefab == null)
                continue;

            if (pools.ContainsKey(config.prefab))
            {
                Debug.LogWarning($"Duplicate pool: {config.prefab.name}");
                continue;
            }

            PoolEntry pool = new PoolEntry(this, config);
            pools.Add(config.prefab, pool);
        }
    }

    // Spawn an object
    public GameObject Spawn(
        GameObject prefab,
        Vector3 position,
        Quaternion rotation,
        Transform parent = null)
    {
        if (prefab == null)
            return null;

        if (!pools.TryGetValue(prefab, out PoolEntry pool))
        {
            Debug.LogError($"No pool registered for {prefab.name}");
            return null;
        }

        GameObject instance = pool.Get();

        instance.transform.SetParent(parent, true);
        instance.transform.SetPositionAndRotation(position, rotation);

        pool.NotifySpawn(instance);

        return instance;
    }

    // Return an object
    public bool Despawn(GameObject instance)
    {
        if (instance == null)
            return false;

        if (!instanceToPool.TryGetValue(instance, out PoolEntry pool))
        {
            Debug.LogWarning($"Object {instance.name} does not belong to this pool.");
            return false;
        }

        if (!pool.IsActive(instance))
        {
            Debug.LogWarning($"Object {instance.name} is already in the pool.");
            return false;
        }

        pool.Release(instance);
        return true;
    }

    private GameObject CreateInstance(GameObject prefab)
    {
        GameObject instance = Instantiate(prefab, transform);
        instance.name = prefab.name;
        instance.SetActive(false);

        return instance;
    }

    private void OnDestroy()
    {
        foreach (var pool in pools.Values)
            pool.Dispose();

        pools.Clear();
        instanceToPool.Clear();
    }

    // Individual pool implementation
    private class PoolEntry
    {
        private readonly ObjectPoolManager manager;
        private readonly GameObject prefab;

        private readonly ObjectPool<GameObject> pool;

        private readonly HashSet<GameObject> activeInstances = new();

        private bool isWarmingUp;

        private readonly int maxSize;
        public PoolEntry(ObjectPoolManager manager, ObjectPoolConfig config)
        {
            this.manager = manager;
            prefab = config.prefab;

            maxSize = Mathf.Max(1, config.maxRetained);

            pool = new ObjectPool<GameObject>(
                Create,
                OnGet,
                OnRelease,
                OnDestroyObject,
                true,
                maxSize,
                maxSize
            );

            WarmUp(config.prewarmCount);
        }

        private GameObject Create()
        {
            GameObject instance = manager.CreateInstance(prefab);

            manager.instanceToPool.Add(instance, this);

            return instance;
        }

        private void OnGet(GameObject instance)
        {
            activeInstances.Add(instance);
            instance.SetActive(true);
        }

        private void OnRelease(GameObject instance)
        {
            if (!isWarmingUp)
                NotifyDespawn(instance);

            instance.SetActive(false);
            instance.transform.SetParent(manager.transform, false);

            activeInstances.Remove(instance);
        }

        private void OnDestroyObject(GameObject instance)
        {
            manager.instanceToPool.Remove(instance);
            Destroy(instance);
        }

        public GameObject Get()
        {
            return pool.Get();
        }

        public void Release(GameObject instance)
        {
            pool.Release(instance);
        }

        public bool IsActive(GameObject instance)
        {
            return activeInstances.Contains(instance);
        }

        public void NotifySpawn(GameObject instance)
        {
            if (instance == null)
                return;

            foreach (MonoBehaviour component in instance.GetComponents<MonoBehaviour>())
            {
                if (component is IPoolable poolable)
                    poolable.OnPoolSpawn();
            }
        }

        private void NotifyDespawn(GameObject instance)
        {
            if (instance == null)
                return;

            foreach (MonoBehaviour component in instance.GetComponents<MonoBehaviour>())
            {
                if (component is IPoolable poolable)
                    poolable.OnPoolDespawn();
            }
        }

        private void WarmUp(int count)
        {
            isWarmingUp = true;

            try
            {
                count = Mathf.Clamp(count, 0, maxSize);

                for (int i = 0; i < count; i++)
                {
                    GameObject instance = pool.Get();
                    pool.Release(instance);
                }
            }
            finally
            {
                isWarmingUp = false;
            }
        }

        public void Dispose()
        {
            foreach (GameObject instance in activeInstances)
            {
                if (instance != null)
                    Destroy(instance);
            }

            activeInstances.Clear();
            pool.Clear();
        }
    }
}