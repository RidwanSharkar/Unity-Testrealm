using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages projectile spawning with object pooling for performance optimization.
/// Singleton pattern for global access.
/// </summary>
public class ProjectileManager : Singleton<ProjectileManager>
{
    [Header("Pool Settings")]
    [SerializeField] private int defaultPoolSize = 20;
    [SerializeField] private bool allowPoolGrowth = true;
    [SerializeField] private Transform poolParent;
    
    [Header("Projectile Prefabs")]
    [SerializeField] private GameObject[] projectilePrefabs;
    
    // Object pools for each projectile type
    private Dictionary<string, Queue<GameObject>> projectilePools = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, GameObject> prefabLookup = new Dictionary<string, GameObject>();
    
    protected override void Awake()
    {
        base.Awake();
        
        // Create pool parent if not assigned
        if (poolParent == null)
        {
            GameObject poolObj = new GameObject("ProjectilePool");
            poolObj.transform.SetParent(transform);
            poolParent = poolObj.transform;
        }
        
        // Initialize pools for all prefabs
        InitializePools();
    }
    
    /// <summary>
    /// Initialize object pools for all projectile prefabs
    /// </summary>
    private void InitializePools()
    {
        if (projectilePrefabs == null || projectilePrefabs.Length == 0)
            return;
        
        foreach (GameObject prefab in projectilePrefabs)
        {
            if (prefab == null) continue;
            
            string prefabName = prefab.name;
            
            // Create pool for this prefab
            projectilePools[prefabName] = new Queue<GameObject>();
            prefabLookup[prefabName] = prefab;
            
            // Pre-instantiate objects
            for (int i = 0; i < defaultPoolSize; i++)
            {
                GameObject obj = CreateNewProjectile(prefab);
                obj.SetActive(false);
                projectilePools[prefabName].Enqueue(obj);
            }
            
            Debug.Log($"Initialized projectile pool for '{prefabName}' with {defaultPoolSize} objects");
        }
    }
    
    /// <summary>
    /// Spawn projectile from pool
    /// </summary>
    public GameObject SpawnProjectile(string prefabName, Vector3 position, Quaternion rotation)
    {
        if (!projectilePools.ContainsKey(prefabName))
        {
            Debug.LogWarning($"No pool found for projectile '{prefabName}'");
            return null;
        }
        
        GameObject projectile = null;
        
        // Try to get from pool
        if (projectilePools[prefabName].Count > 0)
        {
            projectile = projectilePools[prefabName].Dequeue();
        }
        else if (allowPoolGrowth && prefabLookup.ContainsKey(prefabName))
        {
            // Create new if pool is empty and growth is allowed
            projectile = CreateNewProjectile(prefabLookup[prefabName]);
            Debug.Log($"Pool for '{prefabName}' grew (created new instance)");
        }
        else
        {
            Debug.LogWarning($"Pool for '{prefabName}' is empty and growth is disabled!");
            return null;
        }
        
        // Configure and activate
        if (projectile != null)
        {
            projectile.transform.position = position;
            projectile.transform.rotation = rotation;
            projectile.SetActive(true);
        }
        
        return projectile;
    }
    
    /// <summary>
    /// Spawn projectile from pool using prefab reference
    /// </summary>
    public GameObject SpawnProjectile(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            Debug.LogWarning("Cannot spawn null projectile prefab");
            return null;
        }
        
        return SpawnProjectile(prefab.name, position, rotation);
    }
    
    /// <summary>
    /// Return projectile to pool
    /// </summary>
    public void ReturnProjectile(string prefabName, GameObject projectile)
    {
        if (projectile == null) return;
        
        if (!projectilePools.ContainsKey(prefabName))
        {
            Debug.LogWarning($"No pool found for projectile '{prefabName}', destroying instead");
            Destroy(projectile);
            return;
        }
        
        // Deactivate and return to pool
        projectile.SetActive(false);
        projectile.transform.SetParent(poolParent);
        projectilePools[prefabName].Enqueue(projectile);
    }
    
    /// <summary>
    /// Return projectile to pool (auto-detect type)
    /// </summary>
    public void ReturnProjectile(GameObject projectile)
    {
        if (projectile == null) return;
        
        // Try to get original prefab name from projectile
        string prefabName = projectile.name.Replace("(Clone)", "").Trim();
        ReturnProjectile(prefabName, projectile);
    }
    
    /// <summary>
    /// Create new projectile instance
    /// </summary>
    private GameObject CreateNewProjectile(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, poolParent);
        obj.name = prefab.name; // Remove "(Clone)" suffix
        return obj;
    }
    
    /// <summary>
    /// Clear all pools
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var pool in projectilePools.Values)
        {
            while (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
        }
        
        projectilePools.Clear();
        prefabLookup.Clear();
    }
    
    /// <summary>
    /// Get pool size for specific projectile
    /// </summary>
    public int GetPoolSize(string prefabName)
    {
        if (projectilePools.ContainsKey(prefabName))
        {
            return projectilePools[prefabName].Count;
        }
        return 0;
    }
    
    /// <summary>
    /// Add projectile prefab to pool at runtime
    /// </summary>
    public void RegisterProjectilePrefab(GameObject prefab, int poolSize = -1)
    {
        if (prefab == null) return;
        
        string prefabName = prefab.name;
        
        if (projectilePools.ContainsKey(prefabName))
        {
            Debug.LogWarning($"Projectile '{prefabName}' is already registered");
            return;
        }
        
        // Create new pool
        projectilePools[prefabName] = new Queue<GameObject>();
        prefabLookup[prefabName] = prefab;
        
        // Use default pool size if not specified
        int size = poolSize > 0 ? poolSize : defaultPoolSize;
        
        // Pre-instantiate objects
        for (int i = 0; i < size; i++)
        {
            GameObject obj = CreateNewProjectile(prefab);
            obj.SetActive(false);
            projectilePools[prefabName].Enqueue(obj);
        }
        
        Debug.Log($"Registered projectile '{prefabName}' with pool size {size}");
    }
    
    private void OnDestroy()
    {
        ClearAllPools();
    }
}

/// <summary>
/// Extension class for pooled projectiles
/// Automatically returns projectiles to pool when destroyed
/// </summary>
public class PooledProjectile : MonoBehaviour
{
    private string prefabName;
    private bool usePooling = true;
    
    private void Awake()
    {
        prefabName = gameObject.name.Replace("(Clone)", "").Trim();
    }
    
    /// <summary>
    /// Return to pool instead of destroying
    /// </summary>
    public void ReturnToPool()
    {
        if (usePooling && ProjectileManager.Instance != null)
        {
            ProjectileManager.Instance.ReturnProjectile(prefabName, gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Disable pooling (will destroy normally)
    /// </summary>
    public void DisablePooling()
    {
        usePooling = false;
    }
}

