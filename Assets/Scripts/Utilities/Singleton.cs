using UnityEngine;

/// <summary>
/// Generic singleton pattern for MonoBehaviour classes.
/// Ensures only one instance exists and persists across scenes.
/// </summary>
/// <typeparam name="T">The type of the singleton class</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static object lockObject = new object();
    private static bool applicationIsQuitting = false;
    
    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed. Returning null.");
                return null;
            }
            
            lock (lockObject)
            {
                if (instance == null)
                {
                    // Find existing instance
                    instance = FindObjectOfType<T>();
                    
                    // Create new instance if none exists
                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        instance = singletonObject.AddComponent<T>();
                        singletonObject.name = $"{typeof(T).Name} (Singleton)";
                        
                        DontDestroyOnLoad(singletonObject);
                        
                        Debug.Log($"[Singleton] Created singleton instance of {typeof(T)}");
                    }
                }
                
                return instance;
            }
        }
    }
    
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Debug.LogWarning($"[Singleton] Duplicate instance of {typeof(T)} detected. Destroying.");
            Destroy(gameObject);
        }
    }
    
    protected virtual void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }
    
    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            applicationIsQuitting = true;
        }
    }
}

/// <summary>
/// Persistent singleton that survives scene changes.
/// </summary>
public class PersistentSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                
                if (instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    instance = go.AddComponent<T>();
                }
            }
            return instance;
        }
    }
    
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}

