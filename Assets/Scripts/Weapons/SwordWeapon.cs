using UnityEngine;

/// <summary>
/// Sword weapon - Tank class primary weapon.
/// Features: 3-hit combo, charge ability, deflect, and area attacks.
/// </summary>
public class SwordWeapon : BaseWeapon
{
    [Header("Sword-Specific Settings")]
    [SerializeField] private float comboResetTime = 1f;
    [SerializeField] private float comboMultiplier1 = 1f;
    [SerializeField] private float comboMultiplier2 = 1.2f;
    [SerializeField] private float comboMultiplier3 = 1.5f;
    
    [Header("Combo Audio")]
    [SerializeField] private AudioClip swingSound1;
    [SerializeField] private AudioClip swingSound2;
    [SerializeField] private AudioClip swingSound3;
    
    [Header("Ability Settings")]
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeDuration = 0.5f;
    [SerializeField] private int chargeDamage = 20;
    [SerializeField] private float deflectDuration = 0.3f;
    [SerializeField] private int colossusStrikeDamage = 50;
    [SerializeField] private float colossusStrikeRadius = 3f;
    
    [Header("Ability VFX")]
    [SerializeField] private GameObject chargeEffect;
    [SerializeField] private GameObject deflectEffect;
    [SerializeField] private GameObject colossusEffect;
    [SerializeField] private GameObject windShearEffect;
    
    // Combo state
    private int currentComboStep = 1;
    private float lastAttackTime = 0f;
    
    // Ability states
    private bool isCharging = false;
    private bool isDeflecting = false;
    private float deflectEndTime = 0f;
    
    protected override void Awake()
    {
        base.Awake();
        weaponName = "Sword";
        baseDamage = 15;
        attackRange = 2.5f;
        fireRate = 1.21f; // 0.825s between attacks
    }
    
    protected override void InitializeAbilityCooldowns()
    {
        abilityCooldowns[0] = new AbilityCooldown { abilityName = "Charge", cooldownTime = 6f };
        abilityCooldowns[1] = new AbilityCooldown { abilityName = "Deflect", cooldownTime = 8f };
        abilityCooldowns[2] = new AbilityCooldown { abilityName = "Colossus Strike", cooldownTime = 12f };
        abilityCooldowns[3] = new AbilityCooldown { abilityName = "Wind Shear", cooldownTime = 10f };
    }
    
    public override void UpdateWeapon(float deltaTime)
    {
        // Reset combo if too much time has passed
        if (Time.time - lastAttackTime > comboResetTime)
        {
            currentComboStep = 1;
        }
        
        // Update deflect state
        if (isDeflecting && Time.time >= deflectEndTime)
        {
            isDeflecting = false;
            if (deflectEffect != null)
                deflectEffect.SetActive(false);
        }
    }
    
    public override void PerformAttack()
    {
        if (!isEquipped || !CanFire())
            return;
        
        // Play combo animation
        PlayAnimation($"SwordAttack{currentComboStep}");
        
        // Play combo sound
        AudioClip comboSound = currentComboStep switch
        {
            1 => swingSound1,
            2 => swingSound2,
            3 => swingSound3,
            _ => attackSound
        };
        PlaySound(comboSound);
        
        // Perform melee hit detection
        PerformMeleeSwing();
        
        // Update combo step
        lastAttackTime = Time.time;
        lastFireTime = Time.time;
        currentComboStep = currentComboStep >= 3 ? 1 : currentComboStep + 1;
    }
    
    /// <summary>
    /// Perform melee hit detection in front of player
    /// </summary>
    private void PerformMeleeSwing()
    {
        if (attackPoint == null)
            attackPoint = transform;
        
        // Get combo damage multiplier
        float comboMult = currentComboStep switch
        {
            1 => comboMultiplier1,
            2 => comboMultiplier2,
            3 => comboMultiplier3,
            _ => 1f
        };
        
        int actualDamage = Mathf.RoundToInt(baseDamage * comboMult);
        
        // Detect enemies in attack range
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRange);
        
        foreach (Collider hit in hits)
        {
            Entity targetEntity = hit.GetComponent<Entity>();
            
            if (targetEntity != null && targetEntity != ownerEntity)
            {
                // Check if target is in front of player
                Vector3 directionToTarget = (hit.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, directionToTarget);
                
                if (angle < 90f) // 180 degree arc in front
                {
                    // Temporarily override base damage for combo
                    int originalDamage = baseDamage;
                    baseDamage = actualDamage;
                    
                    DealDamageToTarget(targetEntity, hit.ClosestPoint(attackPoint.position));
                    
                    baseDamage = originalDamage;
                    
                    // Spawn hit effect
                    SpawnAttackEffect(hit.ClosestPoint(attackPoint.position), Quaternion.identity);
                }
            }
        }
    }
    
    public override void PerformAbility(string abilityKey)
    {
        switch (abilityKey.ToUpper())
        {
            case "Q":
                if (IsAbilityReady(0)) PerformCharge();
                break;
            case "E":
                if (IsAbilityReady(1)) PerformDeflect();
                break;
            case "R":
                if (IsAbilityReady(2)) PerformColossusStrike();
                break;
            case "F":
                if (IsAbilityReady(3)) PerformWindShear();
                break;
        }
    }
    
    /// <summary>
    /// Q - Charge: Dash forward and deal damage
    /// </summary>
    private void PerformCharge()
    {
        StartCooldown(0);
        OnAbilityUsed?.Invoke("Charge");
        
        PlayAnimation("SwordCharge");
        
        if (chargeEffect != null)
        {
            GameObject effect = Instantiate(chargeEffect, transform.position, transform.rotation);
            Destroy(effect, chargeDuration);
        }
        
        // Dash forward
        if (ownerEntity != null)
        {
            Vector3 chargeDirection = transform.forward;
            Vector3 targetPosition = transform.position + chargeDirection * (chargeSpeed * chargeDuration);
            
            MovementComponent movement = ownerEntity.GetEntityComponent<MovementComponent>();
            if (movement != null)
            {
                movement.ApplyKnockback(chargeDirection * chargeSpeed);
            }
        }
        
        // Damage enemies in path
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 1f, transform.forward, chargeSpeed * chargeDuration);
        
        foreach (RaycastHit hit in hits)
        {
            Entity targetEntity = hit.collider.GetComponent<Entity>();
            if (targetEntity != null && targetEntity != ownerEntity)
            {
                int originalDamage = baseDamage;
                baseDamage = chargeDamage;
                DealDamageToTarget(targetEntity, hit.point);
                baseDamage = originalDamage;
            }
        }
        
        Debug.Log("Sword: Charge ability used!");
    }
    
    /// <summary>
    /// E - Deflect: Block and reflect damage
    /// </summary>
    private void PerformDeflect()
    {
        StartCooldown(1);
        OnAbilityUsed?.Invoke("Deflect");
        
        PlayAnimation("SwordDeflect");
        
        isDeflecting = true;
        deflectEndTime = Time.time + deflectDuration;
        
        if (deflectEffect != null)
        {
            deflectEffect.SetActive(true);
        }
        
        // Make player briefly invulnerable
        if (ownerEntity != null)
        {
            HealthComponent health = ownerEntity.GetEntityComponent<HealthComponent>();
            if (health != null)
            {
                health.SetInvulnerable(true);
                Invoke(nameof(EndDeflect), deflectDuration);
            }
        }
        
        Debug.Log("Sword: Deflect ability used!");
    }
    
    private void EndDeflect()
    {
        if (ownerEntity != null)
        {
            HealthComponent health = ownerEntity.GetEntityComponent<HealthComponent>();
            if (health != null)
            {
                health.SetInvulnerable(false);
            }
        }
    }
    
    /// <summary>
    /// R - Colossus Strike: Massive AOE damage
    /// </summary>
    private void PerformColossusStrike()
    {
        StartCooldown(2);
        OnAbilityUsed?.Invoke("Colossus Strike");
        
        PlayAnimation("SwordColossus");
        
        if (colossusEffect != null)
        {
            GameObject effect = Instantiate(colossusEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
        
        // Deal AOE damage
        Collider[] hits = Physics.OverlapSphere(transform.position, colossusStrikeRadius);
        
        foreach (Collider hit in hits)
        {
            Entity targetEntity = hit.GetComponent<Entity>();
            if (targetEntity != null && targetEntity != ownerEntity)
            {
                int originalDamage = baseDamage;
                baseDamage = colossusStrikeDamage;
                DealDamageToTarget(targetEntity, hit.ClosestPoint(transform.position));
                baseDamage = originalDamage;
            }
        }
        
        Debug.Log("Sword: Colossus Strike ability used!");
    }
    
    /// <summary>
    /// F - Wind Shear: Ranged slash projectile
    /// </summary>
    private void PerformWindShear()
    {
        StartCooldown(3);
        OnAbilityUsed?.Invoke("Wind Shear");
        
        PlayAnimation("SwordWindShear");
        
        if (windShearEffect != null)
        {
            GameObject projectile = Instantiate(windShearEffect, attackPoint.position, transform.rotation);
            
            // Add projectile component (you'll need to create this)
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = transform.forward * 20f;
            }
            
            Destroy(projectile, 3f);
        }
        
        Debug.Log("Sword: Wind Shear ability used!");
    }
    
    /// <summary>
    /// Check if currently deflecting
    /// </summary>
    public bool IsDeflecting()
    {
        return isDeflecting;
    }
}

