using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Entity class for ECS architecture.
/// Manages component registration and provides type-safe component access.
/// </summary>
public class Entity : MonoBehaviour
{
    // Component storage using Type as key
    private Dictionary<Type, Component> componentCache = new Dictionary<Type, Component>();
    
    // Entity metadata
    [SerializeField] private string entityId;
    [SerializeField] protected string entityName;
    [SerializeField] private bool isActive = true;
    
    public string EntityId => entityId;
    public string EntityName => entityName;
    public bool IsActive => isActive;
    
    protected virtual void Awake()
    {
        // Generate unique ID if not set
        if (string.IsNullOrEmpty(entityId))
        {
            entityId = System.Guid.NewGuid().ToString();
        }
        
        // Cache all components on this entity
        CacheComponents();
    }
    
    /// <summary>
    /// Cache all components attached to this GameObject
    /// </summary>
    private void CacheComponents()
    {
        Component[] components = GetComponents<Component>();
        foreach (Component component in components)
        {
            if (component != null && component != this)
            {
                componentCache[component.GetType()] = component;
            }
        }
    }
    
    /// <summary>
    /// Add a component to this entity and cache it
    /// </summary>
    public T AddEntityComponent<T>() where T : Component
    {
        T component = gameObject.AddComponent<T>();
        componentCache[typeof(T)] = component;
        return component;
    }
    
    /// <summary>
    /// Get a component from cache or GameObject
    /// </summary>
    public T GetEntityComponent<T>() where T : Component
    {
        Type type = typeof(T);
        
        if (componentCache.TryGetValue(type, out Component component))
        {
            return component as T;
        }
        
        // If not in cache, try to find it
        T foundComponent = GetComponent<T>();
        if (foundComponent != null)
        {
            componentCache[type] = foundComponent;
        }
        
        return foundComponent;
    }
    
    /// <summary>
    /// Check if entity has a specific component
    /// </summary>
    public bool HasEntityComponent<T>() where T : Component
    {
        return componentCache.ContainsKey(typeof(T)) || GetComponent<T>() != null;
    }
    
    /// <summary>
    /// Remove a component from this entity
    /// </summary>
    public void RemoveEntityComponent<T>() where T : Component
    {
        Type type = typeof(T);
        
        if (componentCache.ContainsKey(type))
        {
            Component component = componentCache[type];
            componentCache.Remove(type);
            Destroy(component);
        }
    }
    
    /// <summary>
    /// Set entity active/inactive state
    /// </summary>
    public void SetEntityActive(bool active)
    {
        isActive = active;
        gameObject.SetActive(active);
    }
    
    /// <summary>
    /// Destroy this entity
    /// </summary>
    public virtual void DestroyEntity()
    {
        componentCache.Clear();
        Destroy(gameObject);
    }
    
    protected virtual void OnDestroy()
    {
        componentCache.Clear();
    }
}

