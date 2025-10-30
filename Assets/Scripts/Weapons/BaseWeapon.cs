using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Weapon subclass types for specialization
/// </summary>
public enum WeaponSubclass
{
    Base,
    Subclass1,
    Subclass2,
    Subclass3
}

/// <summary>
/// Ability cooldown state
/// </summary>
[System.Serializable]
public class AbilityCooldown
{
    public string abilityName;
    public float cooldownTime;
    public float remainingTime;
    
    public bool IsReady => remainingTime <= 0f;
    public float Progress => 1f - (remainingTime / cooldownTime);
}

/// <summary>
/// Base weapon class for all weapons in the game.
/// Handles attacks, abilities, animations, and audio.
/// </summary>
public abstract class BaseWeapon : MonoBehaviour
{
    [Header("Weapon Identity")]
    [SerializeField] protected WeaponType weaponType;
    [SerializeField] protected WeaponSubclass subclass = WeaponSubclass.Base;
    [SerializeField] protected string weaponName = "Base Weapon";
    
    [Header("Damage Settings")]
    [SerializeField] protected int baseDamage = 10;
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float fireRate = 1f; // Attacks per second
    
    [Header("Components")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected AudioSource audioSource;
    
    [Header("Audio Clips")]
    [SerializeField] protected AudioClip attackSound;
    [SerializeField] protected AudioClip[] abilitySounds;
    
    [Header("Visual Effects")]
    [SerializeField] protected GameObject attackEffectPrefab;
    [SerializeField] protected Transform attackPoint;
    
    // Weapon state
    protected Entity ownerEntity;
    protected bool isCharging = false;
    protected float chargeProgress = 0f;
    protected float lastFireTime = 0f;
    protected bool isEquipped = false;
    
    // Ability cooldowns
    protected AbilityCooldown[] abilityCooldowns = new AbilityCooldown[4];
    
    // Events
    public UnityEvent<int, bool> OnAttackPerformed; // damage, isCritical
    public UnityEvent<string> OnAbilityUsed; // abilityName
    
    // Properties
    public WeaponType WeaponType => weaponType;
    public WeaponSubclass Subclass => subclass;
    public string WeaponName => weaponName;
    public int BaseDamage => baseDamage;
    public bool IsCharging => isCharging;
    public float ChargeProgress => chargeProgress;
    public bool IsEquipped => isEquipped;
    
    protected virtual void Awake()
    {
        InitializeAbilityCooldowns();
        
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    
    protected virtual void Update()
    {
        UpdateAbilityCooldowns(Time.deltaTime);
        UpdateWeapon(Time.deltaTime);
    }
    
    /// <summary>
    /// Initialize ability cooldown trackers
    /// </summary>
    protected virtual void InitializeAbilityCooldowns()
    {
        // Override in derived classes to set specific cooldowns
    }
    
    /// <summary>
    /// Set the owner of this weapon
    /// </summary>
    public virtual void SetOwner(Entity owner)
    {
        ownerEntity = owner;
    }
    
    /// <summary>
    /// Equip this weapon
    /// </summary>
    public virtual void Equip()
    {
        isEquipped = true;
        gameObject.SetActive(true);
        OnEquipped();
    }
    
    /// <summary>
    /// Unequip this weapon
    /// </summary>
    public virtual void Unequip()
    {
        isEquipped = false;
        gameObject.SetActive(false);
        OnUnequipped();
    }
    
    /// <summary>
    /// Called when weapon is equipped
    /// </summary>
    protected virtual void OnEquipped()
    {
        // Override in derived classes
    }
    
    /// <summary>
    /// Called when weapon is unequipped
    /// </summary>
    protected virtual void OnUnequipped()
    {
        // Override in derived classes
    }
    
    /// <summary>
    /// Perform primary attack (left-click) - usually a single cast/swing
    /// </summary>
    public virtual void PerformPrimaryAttack()
    {
        // Default implementation - override in derived classes
        PerformAttack();
    }
    
    /// <summary>
    /// Perform secondary attack (right-click held) - usually continuous
    /// </summary>
    public abstract void PerformAttack();
    
    /// <summary>
    /// Perform ability
    /// </summary>
    public abstract void PerformAbility(string abilityKey);
    
    /// <summary>
    /// Update weapon state (override in derived classes for charging, etc.)
    /// </summary>
    public virtual void UpdateWeapon(float deltaTime)
    {
        // Override in derived classes
    }
    
    /// <summary>
    /// Check if weapon can fire based on fire rate
    /// </summary>
    protected virtual bool CanFire()
    {
        return Time.time - lastFireTime >= (1f / fireRate);
    }
    
    /// <summary>
    /// Play weapon sound
    /// </summary>
    protected void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
    
    /// <summary>
    /// Play animation
    /// </summary>
    protected void PlayAnimation(string triggerName)
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }
    }
    
    /// <summary>
    /// Set animation parameter
    /// </summary>
    protected void SetAnimationBool(string paramName, bool value)
    {
        if (animator != null)
        {
            animator.SetBool(paramName, value);
        }
    }
    
    /// <summary>
    /// Spawn attack effect
    /// </summary>
    protected GameObject SpawnAttackEffect(Vector3 position, Quaternion rotation)
    {
        if (attackEffectPrefab != null)
        {
            GameObject effect = Instantiate(attackEffectPrefab, position, rotation);
            Destroy(effect, 2f);
            return effect;
        }
        return null;
    }
    
    /// <summary>
    /// Update ability cooldowns
    /// </summary>
    protected void UpdateAbilityCooldowns(float deltaTime)
    {
        foreach (var cooldown in abilityCooldowns)
        {
            if (cooldown != null && cooldown.remainingTime > 0f)
            {
                cooldown.remainingTime -= deltaTime;
                cooldown.remainingTime = Mathf.Max(0f, cooldown.remainingTime);
            }
        }
    }
    
    /// <summary>
    /// Start ability cooldown
    /// </summary>
    protected void StartCooldown(int abilityIndex)
    {
        if (abilityIndex >= 0 && abilityIndex < abilityCooldowns.Length && abilityCooldowns[abilityIndex] != null)
        {
            abilityCooldowns[abilityIndex].remainingTime = abilityCooldowns[abilityIndex].cooldownTime;
        }
    }
    
    /// <summary>
    /// Check if ability is ready
    /// </summary>
    protected bool IsAbilityReady(int abilityIndex)
    {
        if (abilityIndex >= 0 && abilityIndex < abilityCooldowns.Length && abilityCooldowns[abilityIndex] != null)
        {
            return abilityCooldowns[abilityIndex].IsReady;
        }
        return false;
    }
    
    /// <summary>
    /// Get ability cooldown
    /// </summary>
    public AbilityCooldown GetAbilityCooldown(int abilityIndex)
    {
        if (abilityIndex >= 0 && abilityIndex < abilityCooldowns.Length)
        {
            return abilityCooldowns[abilityIndex];
        }
        return null;
    }
    
    /// <summary>
    /// Deal damage to target using damage calculator
    /// </summary>
    protected void DealDamageToTarget(Entity target, Vector3 hitPosition = default, Vector3 hitNormal = default)
    {
        if (target == null || CombatSystem.Instance == null)
            return;
        
        // Calculate damage with crits
        DamageResult result = DamageCalculator.CalculateDamage(baseDamage, weaponType);
        
        // Queue damage in combat system
        CombatSystem.Instance.QueueDamage(
            target,
            ownerEntity,
            result.damage,
            result.damageType,
            weaponType,
            result.isCritical,
            hitPosition,
            hitNormal
        );
        
        // Invoke event
        OnAttackPerformed?.Invoke(result.damage, result.isCritical);
    }
}

