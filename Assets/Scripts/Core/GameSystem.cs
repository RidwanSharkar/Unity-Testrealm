using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all game systems in the ECS architecture.
/// Systems process entities that have specific components.
/// </summary>
public abstract class GameSystem : MonoBehaviour
{
    protected List<Entity> registeredEntities = new List<Entity>();
    protected bool isSystemActive = true;
    
    public bool IsSystemActive => isSystemActive;
    
    protected virtual void Awake()
    {
        InitializeSystem();
    }
    
    protected virtual void Start()
    {
        // Find and register existing entities in the scene
        FindAndRegisterExistingEntities();
    }
    
    /// <summary>
    /// Initialize the system (override in derived classes)
    /// </summary>
    protected virtual void InitializeSystem()
    {
        Debug.Log($"{GetType().Name} initialized");
    }
    
    /// <summary>
    /// Register an entity with this system
    /// </summary>
    public virtual void RegisterEntity(Entity entity)
    {
        if (entity == null)
        {
            Debug.LogWarning($"{GetType().Name}: Attempted to register null entity");
            return;
        }
        
        if (!registeredEntities.Contains(entity))
        {
            if (CanProcessEntity(entity))
            {
                registeredEntities.Add(entity);
                OnEntityRegistered(entity);
            }
        }
    }
    
    /// <summary>
    /// Unregister an entity from this system
    /// </summary>
    public virtual void UnregisterEntity(Entity entity)
    {
        if (registeredEntities.Contains(entity))
        {
            registeredEntities.Remove(entity);
            OnEntityUnregistered(entity);
        }
    }
    
    /// <summary>
    /// Check if this system can process a specific entity
    /// Override this to filter entities by required components
    /// </summary>
    protected virtual bool CanProcessEntity(Entity entity)
    {
        return true; // By default, process all entities
    }
    
    /// <summary>
    /// Called when an entity is registered
    /// </summary>
    protected virtual void OnEntityRegistered(Entity entity)
    {
        // Override in derived classes
    }
    
    /// <summary>
    /// Called when an entity is unregistered
    /// </summary>
    protected virtual void OnEntityUnregistered(Entity entity)
    {
        // Override in derived classes
    }
    
    /// <summary>
    /// Find and register all existing entities in the scene
    /// </summary>
    protected virtual void FindAndRegisterExistingEntities()
    {
        Entity[] entities = FindObjectsOfType<Entity>();
        foreach (Entity entity in entities)
        {
            RegisterEntity(entity);
        }
    }
    
    /// <summary>
    /// Update loop for the system
    /// </summary>
    protected virtual void Update()
    {
        if (!isSystemActive) return;
        
        ProcessEntities(Time.deltaTime);
    }
    
    /// <summary>
    /// Process all registered entities (override in derived classes)
    /// </summary>
    protected virtual void ProcessEntities(float deltaTime)
    {
        // Override in derived classes
    }
    
    /// <summary>
    /// Fixed update loop for physics-related systems
    /// </summary>
    protected virtual void FixedUpdate()
    {
        if (!isSystemActive) return;
        
        ProcessEntitiesFixed(Time.fixedDeltaTime);
    }
    
    /// <summary>
    /// Process entities in FixedUpdate (override in derived classes)
    /// </summary>
    protected virtual void ProcessEntitiesFixed(float fixedDeltaTime)
    {
        // Override in derived classes
    }
    
    /// <summary>
    /// Enable or disable this system
    /// </summary>
    public void SetSystemActive(bool active)
    {
        isSystemActive = active;
    }
    
    /// <summary>
    /// Clean up all registered entities
    /// </summary>
    public virtual void ClearAllEntities()
    {
        registeredEntities.Clear();
    }
    
    /// <summary>
    /// Get count of registered entities
    /// </summary>
    public int GetEntityCount()
    {
        return registeredEntities.Count;
    }
}

