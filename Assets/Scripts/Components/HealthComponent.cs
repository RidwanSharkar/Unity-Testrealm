using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Health component for entities.
/// Manages health, damage, healing, and death events.
/// </summary>
public class HealthComponent : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool isInvulnerable = false;
    [SerializeField] private bool isDead = false;
    
    [Header("Shield Settings")]
    [SerializeField] private int currentShield = 0;
    [SerializeField] private int maxShield = 0;
    
    [Header("Regeneration")]
    [SerializeField] private bool canRegenerate = false;
    [SerializeField] private float regenerationRate = 1f; // HP per second
    [SerializeField] private float regenerationDelay = 3f; // Delay after taking damage
    private float timeSinceLastDamage = 0f;
    
    // Events
    public UnityEvent<int, int> OnHealthChanged; // current, max
    public UnityEvent<int> OnDamageTaken; // damage amount
    public UnityEvent<int> OnHealed; // heal amount
    public UnityEvent OnDeath;
    public UnityEvent OnRevived;
    
    // Properties
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int CurrentShield => currentShield;
    public int MaxShield => maxShield;
    public bool IsDead => isDead;
    public bool IsInvulnerable => isInvulnerable;
    public float HealthPercentage => maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
    
    void Awake()
    {
        currentHealth = maxHealth;
    }
    
    void Update()
    {
        if (canRegenerate && !isDead && currentHealth < maxHealth)
        {
            timeSinceLastDamage += Time.deltaTime;
            
            if (timeSinceLastDamage >= regenerationDelay)
            {
                Heal(Mathf.CeilToInt(regenerationRate * Time.deltaTime));
            }
        }
    }
    
    /// <summary>
    /// Apply damage to this entity
    /// </summary>
    public void TakeDamage(int damageAmount)
    {
        if (isDead || isInvulnerable || damageAmount <= 0)
            return;
        
        timeSinceLastDamage = 0f; // Reset regeneration timer
        
        // Apply damage to shield first
        if (currentShield > 0)
        {
            int shieldDamage = Mathf.Min(currentShield, damageAmount);
            currentShield -= shieldDamage;
            damageAmount -= shieldDamage;
            
            if (damageAmount <= 0)
            {
                OnDamageTaken?.Invoke(shieldDamage);
                return; // Damage absorbed by shield
            }
        }
        
        // Apply remaining damage to health
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnDamageTaken?.Invoke(damageAmount);
        
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }
    
    /// <summary>
    /// Heal this entity
    /// </summary>
    public void Heal(int healAmount)
    {
        if (isDead || healAmount <= 0)
            return;
        
        int oldHealth = currentHealth;
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        int actualHealed = currentHealth - oldHealth;
        
        if (actualHealed > 0)
        {
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnHealed?.Invoke(actualHealed);
        }
    }
    
    /// <summary>
    /// Add shield points
    /// </summary>
    public void AddShield(int shieldAmount)
    {
        currentShield += shieldAmount;
        currentShield = Mathf.Min(currentShield, maxShield);
    }
    
    /// <summary>
    /// Set maximum shield capacity
    /// </summary>
    public void SetMaxShield(int newMaxShield)
    {
        maxShield = newMaxShield;
        currentShield = Mathf.Min(currentShield, maxShield);
    }
    
    /// <summary>
    /// Set maximum health (scales current health proportionally)
    /// </summary>
    public void SetMaxHealth(int newMaxHealth, bool scaleCurrentHealth = true)
    {
        if (scaleCurrentHealth)
        {
            float healthPercentage = HealthPercentage;
            maxHealth = newMaxHealth;
            currentHealth = Mathf.RoundToInt(maxHealth * healthPercentage);
        }
        else
        {
            maxHealth = newMaxHealth;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    /// <summary>
    /// Increase max health permanently (e.g., from runes)
    /// </summary>
    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount; // Also increase current health
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    /// <summary>
    /// Handle entity death
    /// </summary>
    private void Die()
    {
        isDead = true;
        currentHealth = 0;
        OnDeath?.Invoke();
        Debug.Log($"{gameObject.name} has died");
    }
    
    /// <summary>
    /// Revive this entity
    /// </summary>
    public void Revive(int reviveHealth = -1)
    {
        if (!isDead) return;
        
        isDead = false;
        currentHealth = reviveHealth > 0 ? reviveHealth : maxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnRevived?.Invoke();
        
        Debug.Log($"{gameObject.name} has been revived");
    }
    
    /// <summary>
    /// Set invulnerability state
    /// </summary>
    public void SetInvulnerable(bool invulnerable)
    {
        isInvulnerable = invulnerable;
    }
    
    /// <summary>
    /// Instantly kill this entity
    /// </summary>
    public void InstantKill()
    {
        currentHealth = 0;
        Die();
    }
    
    /// <summary>
    /// Restore to full health
    /// </summary>
    public void RestoreToFull()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}

