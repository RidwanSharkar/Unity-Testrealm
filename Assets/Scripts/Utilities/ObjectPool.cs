using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic object pool for performance optimization.
/// Reuses GameObjects instead of constantly instantiating and destroying them.
/// </summary>
/// <typeparam name="T">Component type to pool</typeparam>
public class ObjectPool<T> where T : Component
{
    private readonly T prefab;
    private readonly int initialSize;
    private readonly Transform parent;
    
    private Queue<T> availableObjects = new Queue<T>();
    private List<T> allObjects = new List<T>();
    
    /// <summary>
    /// Create a new object pool
    /// </summary>
    public ObjectPool(T prefab, int initialSize = 10, Transform parent = null)
    {
        this.prefab = prefab;
        this.initialSize = initialSize;
        this.parent = parent;
        
        // Pre-instantiate initial objects
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewObject();
        }
    }
    
    /// <summary>
    /// Get an object from the pool
    /// </summary>
    public T Get()
    {
        T obj;
        
        if (availableObjects.Count > 0)
        {
            obj = availableObjects.Dequeue();
        }
        else
        {
            obj = CreateNewObject();
        }
        
        obj.gameObject.SetActive(true);
        return obj;
    }
    
    /// <summary>
    /// Return an object to the pool
    /// </summary>
    public void Return(T obj)
    {
        if (obj == null) return;
        
        obj.gameObject.SetActive(false);
        availableObjects.Enqueue(obj);
    }
    
    /// <summary>
    /// Create a new object and add it to the pool
    /// </summary>
    private T CreateNewObject()
    {
        T obj = Object.Instantiate(prefab, parent);
        obj.gameObject.SetActive(false);
        allObjects.Add(obj);
        availableObjects.Enqueue(obj);
        return obj;
    }
    
    /// <summary>
    /// Clear all pooled objects
    /// </summary>
    public void Clear()
    {
        foreach (T obj in allObjects)
        {
            if (obj != null)
            {
                Object.Destroy(obj.gameObject);
            }
        }
        
        availableObjects.Clear();
        allObjects.Clear();
    }
    
    /// <summary>
    /// Get count of available objects
    /// </summary>
    public int AvailableCount => availableObjects.Count;
    
    /// <summary>
    /// Get total count of all objects
    /// </summary>
    public int TotalCount => allObjects.Count;
}

/// <summary>
/// Simple GameObject pool (non-generic version)
/// </summary>
public class GameObjectPool
{
    private GameObject prefab;
    private int initialSize;
    private Transform parent;
    
    private Queue<GameObject> availableObjects = new Queue<GameObject>();
    private List<GameObject> allObjects = new List<GameObject>();
    
    public GameObjectPool(GameObject prefab, int initialSize = 10, Transform parent = null)
    {
        this.prefab = prefab;
        this.initialSize = initialSize;
        this.parent = parent;
        
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewObject();
        }
    }
    
    public GameObject Get()
    {
        GameObject obj;
        
        if (availableObjects.Count > 0)
        {
            obj = availableObjects.Dequeue();
        }
        else
        {
            obj = CreateNewObject();
        }
        
        obj.SetActive(true);
        return obj;
    }
    
    public void Return(GameObject obj)
    {
        if (obj == null) return;
        
        obj.SetActive(false);
        availableObjects.Enqueue(obj);
    }
    
    private GameObject CreateNewObject()
    {
        GameObject obj = Object.Instantiate(prefab, parent);
        obj.SetActive(false);
        allObjects.Add(obj);
        availableObjects.Enqueue(obj);
        return obj;
    }
    
    public void Clear()
    {
        foreach (GameObject obj in allObjects)
        {
            if (obj != null)
            {
                Object.Destroy(obj);
            }
        }
        
        availableObjects.Clear();
        allObjects.Clear();
    }
    
    public int AvailableCount => availableObjects.Count;
    public int TotalCount => allObjects.Count;
}

