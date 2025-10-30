using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a damage event in the combat system
/// </summary>
public class DamageEvent
{
    public Entity target;
    public Entity source;
    public int damage;
    public DamageType damageType;
    public WeaponType weaponType;
    public bool isCritical;
    public float timestamp;
    public Vector3 hitPosition;
    public Vector3 hitNormal;
}

/// <summary>
/// Central combat system managing all damage and healing events.
/// Processes damage queue and applies effects to entities.
/// </summary>
public class CombatSystem : GameSystem
{
    public static CombatSystem Instance { get; private set; }
    
    [Header("Combat Settings")]
    [SerializeField] private bool showDamageNumbers = true;
    [SerializeField] private float damageNumberDuration = 2f;
    [SerializeField] private bool enableKnockback = true;
    [SerializeField] private float knockbackForce = 5f;
    
    [Header("Hit Effects")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject criticalHitEffectPrefab;
    
    // Damage processing queue
    private Queue<DamageEvent> damageQueue = new Queue<DamageEvent>();
    private List<DamageEvent> processedEvents = new List<DamageEvent>();
    
    // Statistics tracking
    private int totalDamageDealt = 0;
    private int totalCriticalHits = 0;
    private int totalHealingDone = 0;
    
    public int TotalDamageDealt => totalDamageDealt;
    public int TotalCriticalHits => totalCriticalHits;
    public int TotalHealingDone => totalHealingDone;
    
    protected override void Awake()
    {
        base.Awake();
        
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    protected override void Update()
    {
        base.Update();
        ProcessDamageQueue();
    }
    
    /// <summary>
    /// Queue damage to be applied
    /// </summary>
    public void QueueDamage(Entity target, Entity source, int damage, DamageType damageType, 
                           WeaponType weaponType, bool isCritical, Vector3 hitPosition = default, 
                           Vector3 hitNormal = default)
    {
        if (target == null)
        {
            Debug.LogWarning("CombatSystem: Attempted to queue damage to null target");
            return;
        }
        
        DamageEvent damageEvent = new DamageEvent
        {
            target = target,
            source = source,
            damage = damage,
            damageType = damageType,
            weaponType = weaponType,
            isCritical = isCritical,
            timestamp = Time.time,
            hitPosition = hitPosition,
            hitNormal = hitNormal
        };
        
        damageQueue.Enqueue(damageEvent);
    }
    
    /// <summary>
    /// Queue healing to be applied
    /// </summary>
    public void QueueHealing(Entity target, Entity source, int healAmount, WeaponType weaponType)
    {
        if (target == null) return;
        
        // Healing is represented as negative damage
        DamageEvent healEvent = new DamageEvent
        {
            target = target,
            source = source,
            damage = -healAmount, // Negative for healing
            damageType = DamageType.Healing,
            weaponType = weaponType,
            isCritical = false,
            timestamp = Time.time,
            hitPosition = target.transform.position
        };
        
        damageQueue.Enqueue(healEvent);
    }
    
    /// <summary>
    /// Process all queued damage events
    /// </summary>
    private void ProcessDamageQueue()
    {
        while (damageQueue.Count > 0)
        {
            DamageEvent damageEvent = damageQueue.Dequeue();
            
            if (damageEvent.damageType == DamageType.Healing)
            {
                ApplyHealing(damageEvent);
            }
            else
            {
                ApplyDamage(damageEvent);
            }
            
            processedEvents.Add(damageEvent);
        }
        
        // Clear processed events after a frame
        if (processedEvents.Count > 100)
        {
            processedEvents.RemoveRange(0, 50);
        }
    }
    
    /// <summary>
    /// Apply damage to target entity
    /// </summary>
    private void ApplyDamage(DamageEvent damageEvent)
    {
        HealthComponent healthComponent = damageEvent.target.GetEntityComponent<HealthComponent>();
        
        if (healthComponent == null || healthComponent.IsDead)
            return;
        
        // Apply damage
        healthComponent.TakeDamage(damageEvent.damage);
        
        // Update statistics
        totalDamageDealt += damageEvent.damage;
        if (damageEvent.isCritical)
            totalCriticalHits++;
        
        // Show damage numbers
        if (showDamageNumbers && DamageNumberManager.Instance != null)
        {
            DamageNumberManager.Instance.ShowDamage(
                damageEvent.damage,
                damageEvent.hitPosition != Vector3.zero ? damageEvent.hitPosition : damageEvent.target.transform.position,
                damageEvent.isCritical
            );
        }
        
        // Spawn hit effect
        SpawnHitEffect(damageEvent);
        
        // Apply knockback
        if (enableKnockback && damageEvent.source != null)
        {
            ApplyKnockback(damageEvent);
        }
        
        // Log for debugging
        Debug.Log($"{damageEvent.source?.EntityName ?? "Unknown"} dealt {damageEvent.damage} " +
                 $"{(damageEvent.isCritical ? "CRITICAL " : "")}damage to {damageEvent.target.EntityName}");
    }
    
    /// <summary>
    /// Apply healing to target entity
    /// </summary>
    private void ApplyHealing(DamageEvent healEvent)
    {
        HealthComponent healthComponent = healEvent.target.GetEntityComponent<HealthComponent>();
        
        if (healthComponent == null || healthComponent.IsDead)
            return;
        
        int healAmount = -healEvent.damage; // Convert back to positive
        healthComponent.Heal(healAmount);
        
        // Update statistics
        totalHealingDone += healAmount;
        
        // Show healing numbers
        if (showDamageNumbers && DamageNumberManager.Instance != null)
        {
            DamageNumberManager.Instance.ShowHealing(
                healAmount,
                healEvent.target.transform.position
            );
        }
        
        Debug.Log($"{healEvent.source?.EntityName ?? "Unknown"} healed {healEvent.target.EntityName} for {healAmount}");
    }
    
    /// <summary>
    /// Spawn visual hit effect
    /// </summary>
    private void SpawnHitEffect(DamageEvent damageEvent)
    {
        GameObject effectPrefab = damageEvent.isCritical ? criticalHitEffectPrefab : hitEffectPrefab;
        
        if (effectPrefab != null)
        {
            Vector3 spawnPosition = damageEvent.hitPosition != Vector3.zero ? 
                                   damageEvent.hitPosition : 
                                   damageEvent.target.transform.position;
            
            GameObject effect = Instantiate(effectPrefab, spawnPosition, Quaternion.identity);
            Destroy(effect, 2f); // Auto-destroy after 2 seconds
        }
    }
    
    /// <summary>
    /// Apply knockback to target
    /// </summary>
    private void ApplyKnockback(DamageEvent damageEvent)
    {
        MovementComponent movementComponent = damageEvent.target.GetEntityComponent<MovementComponent>();
        
        if (movementComponent != null && damageEvent.source != null)
        {
            Vector3 knockbackDirection = (damageEvent.target.transform.position - damageEvent.source.transform.position).normalized;
            knockbackDirection.y = 0.5f; // Add slight upward force
            
            float finalKnockbackForce = knockbackForce;
            if (damageEvent.isCritical)
                finalKnockbackForce *= 1.5f;
            
            movementComponent.ApplyKnockback(knockbackDirection * finalKnockbackForce);
        }
    }
    
    /// <summary>
    /// Get recent damage events for a specific entity
    /// </summary>
    public List<DamageEvent> GetRecentDamageEvents(Entity entity, float timeWindow = 5f)
    {
        List<DamageEvent> recentEvents = new List<DamageEvent>();
        float currentTime = Time.time;
        
        foreach (DamageEvent evt in processedEvents)
        {
            if (evt.target == entity && (currentTime - evt.timestamp) <= timeWindow)
            {
                recentEvents.Add(evt);
            }
        }
        
        return recentEvents;
    }
    
    /// <summary>
    /// Reset combat statistics
    /// </summary>
    public void ResetStatistics()
    {
        totalDamageDealt = 0;
        totalCriticalHits = 0;
        totalHealingDone = 0;
        processedEvents.Clear();
    }
    
    /// <summary>
    /// Enable/disable damage numbers
    /// </summary>
    public void SetDamageNumbersEnabled(bool enabled)
    {
        showDamageNumbers = enabled;
    }
}

