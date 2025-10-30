using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Scythe weapon - Healer class primary weapon.
/// Features: Healing attacks, AOE heals, life drain, and support abilities.
/// </summary>
public class ScytheWeapon : BaseWeapon
{
    [Header("Scythe-Specific Settings")]
    [SerializeField] private int healAmount = 10;
    [SerializeField] private float healRadius = 5f;
    [SerializeField] private float lifeDrainPercent = 0.3f; // 30% of damage dealt heals self
    
    [Header("Scythe Audio")]
    [SerializeField] private AudioClip scytheSwingSound;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioClip soulHarvestSound;
    
    [Header("Ability Settings")]
    [SerializeField] private int healingWaveHealAmount = 25;
    [SerializeField] private float healingWaveRadius = 8f;
    [SerializeField] private int soulHarvestDamage = 30;
    [SerializeField] private float soulHarvestRadius = 4f;
    [SerializeField] private int resurrectionHealthPercent = 50;
    [SerializeField] private float spiritLinkDuration = 10f;
    [SerializeField] private float spiritLinkDamageShare = 0.5f;
    
    [Header("Ability VFX")]
    [SerializeField] private GameObject healingWaveEffect;
    [SerializeField] private GameObject soulHarvestEffect;
    [SerializeField] private GameObject resurrectionEffect;
    [SerializeField] private GameObject spiritLinkEffect;
    
    // Scythe state
    private Entity linkedAlly = null;
    private float spiritLinkEndTime = 0f;
    
    protected override void Awake()
    {
        base.Awake();
        weaponName = "Scythe";
        baseDamage = 12;
        attackRange = 3f;
        fireRate = 1.1f; // Slightly faster than sword
    }
    
    protected override void InitializeAbilityCooldowns()
    {
        abilityCooldowns[0] = new AbilityCooldown { abilityName = "Healing Wave", cooldownTime = 8f };
        abilityCooldowns[1] = new AbilityCooldown { abilityName = "Soul Harvest", cooldownTime = 10f };
        abilityCooldowns[2] = new AbilityCooldown { abilityName = "Resurrection", cooldownTime = 60f };
        abilityCooldowns[3] = new AbilityCooldown { abilityName = "Spirit Link", cooldownTime = 15f };
    }
    
    public override void UpdateWeapon(float deltaTime)
    {
        // Update Spirit Link
        if (linkedAlly != null && Time.time >= spiritLinkEndTime)
        {
            linkedAlly = null;
            if (spiritLinkEffect != null)
                spiritLinkEffect.SetActive(false);
        }
    }
    
    public override void PerformAttack()
    {
        if (!isEquipped || !CanFire())
            return;
        
        PlayAnimation("ScytheAttack");
        PlaySound(scytheSwingSound);
        
        // Perform melee swing with life drain
        PerformScytheSwing();
        
        lastFireTime = Time.time;
    }
    
    /// <summary>
    /// Perform scythe melee attack with life drain
    /// </summary>
    private void PerformScytheSwing()
    {
        if (attackPoint == null)
            attackPoint = transform;
        
        // Detect enemies in attack range
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRange);
        int totalDamageDealt = 0;
        
        foreach (Collider hit in hits)
        {
            Entity targetEntity = hit.GetComponent<Entity>();
            
            if (targetEntity != null && targetEntity != ownerEntity)
            {
                // Check if target is in front of player
                Vector3 directionToTarget = (hit.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, directionToTarget);
                
                if (angle < 100f) // Wide arc for scythe
                {
                    // Calculate damage
                    DamageResult result = DamageCalculator.CalculateDamage(baseDamage, weaponType);
                    
                    // Queue damage
                    if (CombatSystem.Instance != null)
                    {
                        CombatSystem.Instance.QueueDamage(
                            targetEntity,
                            ownerEntity,
                            result.damage,
                            result.damageType,
                            weaponType,
                            result.isCritical,
                            hit.ClosestPoint(attackPoint.position)
                        );
                    }
                    
                    totalDamageDealt += result.damage;
                    
                    // Spawn hit effect
                    SpawnAttackEffect(hit.ClosestPoint(attackPoint.position), Quaternion.identity);
                }
            }
        }
        
        // Life drain: heal self based on damage dealt
        if (totalDamageDealt > 0 && ownerEntity != null)
        {
            int healAmount = Mathf.RoundToInt(totalDamageDealt * lifeDrainPercent);
            HealEntity(ownerEntity, healAmount);
        }
    }
    
    public override void PerformAbility(string abilityKey)
    {
        switch (abilityKey.ToUpper())
        {
            case "Q":
                if (IsAbilityReady(0)) PerformHealingWave();
                break;
            case "E":
                if (IsAbilityReady(1)) PerformSoulHarvest();
                break;
            case "R":
                if (IsAbilityReady(2)) PerformResurrection();
                break;
            case "F":
                if (IsAbilityReady(3)) PerformSpiritLink();
                break;
        }
    }
    
    /// <summary>
    /// Q - Healing Wave: Heal all nearby allies
    /// </summary>
    private void PerformHealingWave()
    {
        StartCooldown(0);
        OnAbilityUsed?.Invoke("Healing Wave");
        
        PlayAnimation("ScytheHealingWave");
        PlaySound(healSound);
        
        if (healingWaveEffect != null)
        {
            GameObject effect = Instantiate(healingWaveEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
        
        // Find all allies in radius
        Collider[] hits = Physics.OverlapSphere(transform.position, healingWaveRadius);
        
        foreach (Collider hit in hits)
        {
            Entity targetEntity = hit.GetComponent<Entity>();
            
            // Heal allies (you'd check team/faction here)
            if (targetEntity != null)
            {
                int actualHealAmount = DamageCalculator.CalculateHealing(healingWaveHealAmount, weaponType);
                HealEntity(targetEntity, actualHealAmount);
            }
        }
        
        Debug.Log("Scythe: Healing Wave ability used!");
    }
    
    /// <summary>
    /// E - Soul Harvest: Damage enemies and heal based on enemies hit
    /// </summary>
    private void PerformSoulHarvest()
    {
        StartCooldown(1);
        OnAbilityUsed?.Invoke("Soul Harvest");
        
        PlayAnimation("ScytheSoulHarvest");
        PlaySound(soulHarvestSound);
        
        if (soulHarvestEffect != null)
        {
            GameObject effect = Instantiate(soulHarvestEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
        
        // Damage all enemies in radius
        Collider[] hits = Physics.OverlapSphere(transform.position, soulHarvestRadius);
        int enemiesHit = 0;
        
        foreach (Collider hit in hits)
        {
            Entity targetEntity = hit.GetComponent<Entity>();
            
            if (targetEntity != null && targetEntity != ownerEntity)
            {
                // Deal damage
                if (CombatSystem.Instance != null)
                {
                    DamageResult result = DamageCalculator.CalculateDamage(soulHarvestDamage, weaponType);
                    CombatSystem.Instance.QueueDamage(
                        targetEntity,
                        ownerEntity,
                        result.damage,
                        DamageType.Magical,
                        weaponType,
                        result.isCritical,
                        hit.transform.position
                    );
                    
                    enemiesHit++;
                }
            }
        }
        
        // Heal based on enemies hit
        if (enemiesHit > 0 && ownerEntity != null)
        {
            int totalHealing = enemiesHit * 15;
            HealEntity(ownerEntity, totalHealing);
        }
        
        Debug.Log($"Scythe: Soul Harvest hit {enemiesHit} enemies!");
    }
    
    /// <summary>
    /// R - Resurrection: Revive a dead ally
    /// </summary>
    private void PerformResurrection()
    {
        StartCooldown(2);
        OnAbilityUsed?.Invoke("Resurrection");
        
        PlayAnimation("ScytheResurrection");
        
        // Find dead allies nearby (this would need a proper implementation)
        Collider[] hits = Physics.OverlapSphere(transform.position, healRadius);
        
        foreach (Collider hit in hits)
        {
            Entity targetEntity = hit.GetComponent<Entity>();
            
            if (targetEntity != null && targetEntity != ownerEntity)
            {
                HealthComponent healthComponent = targetEntity.GetEntityComponent<HealthComponent>();
                
                if (healthComponent != null && healthComponent.IsDead)
                {
                    // Calculate revive health
                    int reviveHealth = Mathf.RoundToInt(healthComponent.MaxHealth * (resurrectionHealthPercent / 100f));
                    healthComponent.Revive(reviveHealth);
                    
                    if (resurrectionEffect != null)
                    {
                        GameObject effect = Instantiate(resurrectionEffect, hit.transform.position, Quaternion.identity);
                        Destroy(effect, 3f);
                    }
                    
                    Debug.Log($"Scythe: Resurrected {targetEntity.EntityName}!");
                    break; // Only resurrect one ally
                }
            }
        }
        
        Debug.Log("Scythe: Resurrection ability used!");
    }
    
    /// <summary>
    /// F - Spirit Link: Link with an ally to share healing
    /// </summary>
    private void PerformSpiritLink()
    {
        StartCooldown(3);
        OnAbilityUsed?.Invoke("Spirit Link");
        
        PlayAnimation("ScytheSpiritLink");
        
        // Find closest ally
        Collider[] hits = Physics.OverlapSphere(transform.position, healRadius);
        float closestDistance = float.MaxValue;
        Entity closestAlly = null;
        
        foreach (Collider hit in hits)
        {
            Entity targetEntity = hit.GetComponent<Entity>();
            
            if (targetEntity != null && targetEntity != ownerEntity)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestAlly = targetEntity;
                }
            }
        }
        
        if (closestAlly != null)
        {
            linkedAlly = closestAlly;
            spiritLinkEndTime = Time.time + spiritLinkDuration;
            
            if (spiritLinkEffect != null)
            {
                GameObject effect = Instantiate(spiritLinkEffect, transform.position, Quaternion.identity);
                effect.transform.SetParent(transform);
            }
            
            Debug.Log($"Scythe: Spirit Link established with {closestAlly.EntityName}!");
        }
    }
    
    /// <summary>
    /// Heal an entity
    /// </summary>
    private void HealEntity(Entity target, int healAmount)
    {
        if (target == null || CombatSystem.Instance == null)
            return;
        
        HealthComponent healthComponent = target.GetEntityComponent<HealthComponent>();
        if (healthComponent != null && !healthComponent.IsDead)
        {
            // Apply scythe passive bonus
            int finalHealAmount = DamageCalculator.CalculateHealing(healAmount, weaponType);
            
            // Queue healing through combat system
            CombatSystem.Instance.QueueHealing(target, ownerEntity, finalHealAmount, weaponType);
            
            // If spirit linked, also heal linked ally
            if (linkedAlly != null && linkedAlly != target && Time.time < spiritLinkEndTime)
            {
                int linkHealAmount = Mathf.RoundToInt(finalHealAmount * 0.5f);
                HealthComponent linkedHealth = linkedAlly.GetEntityComponent<HealthComponent>();
                if (linkedHealth != null)
                {
                    linkedHealth.Heal(linkHealAmount);
                }
            }
        }
    }
    
    /// <summary>
    /// Get currently linked ally
    /// </summary>
    public Entity GetLinkedAlly()
    {
        return linkedAlly;
    }
    
    /// <summary>
    /// Check if spirit link is active
    /// </summary>
    public bool IsSpiritLinkActive()
    {
        return linkedAlly != null && Time.time < spiritLinkEndTime;
    }
}

