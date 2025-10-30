using UnityEngine;

/// <summary>
/// Archer's Bow and Arrow weapon
/// Long-range projectile combat with precision
/// </summary>
public class BowWeapon : BaseWeapon
{
    [Header("Bow Settings")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private float arrowSpeed = 30f;
    [SerializeField] private float maxChargeTime = 2f;
    [SerializeField] private float minChargeMultiplier = 0.5f; // 50% damage at minimum
    [SerializeField] private float maxChargeMultiplier = 2.0f; // 200% damage at full charge
    
    [Header("Abilities")]
    [SerializeField] private int multiShotArrowCount = 3;
    [SerializeField] private float multiShotSpreadAngle = 15f;
    [SerializeField] private float multiShotCooldown = 10f;
    
    [SerializeField] private float rapidFireDuration = 5f;
    [SerializeField] private float rapidFireRateMultiplier = 3f;
    [SerializeField] private float rapidFireCooldown = 15f;
    
    // State
    private bool isChargingArrow = false;
    private float chargeStartTime = 0f;
    private bool isRapidFireActive = false;
    private float rapidFireEndTime = 0f;
    
    protected override void Awake()
    {
        base.Awake();
        weaponType = WeaponType.Bow;
        weaponName = "Archer's Bow";
    }
    
    protected override void InitializeAbilityCooldowns()
    {
        abilityCooldowns[0] = new AbilityCooldown
        {
            abilityName = "Multi-Shot",
            cooldownTime = multiShotCooldown,
            remainingTime = 0f
        };
        
        abilityCooldowns[1] = new AbilityCooldown
        {
            abilityName = "Arrow Rain",
            cooldownTime = 12f,
            remainingTime = 0f
        };
        
        abilityCooldowns[2] = new AbilityCooldown
        {
            abilityName = "Rapid Fire",
            cooldownTime = rapidFireCooldown,
            remainingTime = 0f
        };
    }
    
    public override void PerformAttack()
    {
        if (!CanFire()) return;
        
        if (!isChargingArrow)
        {
            // Start charging arrow
            StartCharging();
        }
        else
        {
            // Release arrow
            ReleaseArrow();
        }
    }
    
    private void StartCharging()
    {
        isChargingArrow = true;
        chargeStartTime = Time.time;
        
        PlayAnimation("BowDrawStart");
        SetAnimationBool("IsDrawing", true);
        
        Debug.Log("Started charging arrow...");
    }
    
    private void ReleaseArrow()
    {
        if (!isChargingArrow) return;
        
        // Calculate charge percentage
        float chargeTime = Time.time - chargeStartTime;
        float chargePercent = Mathf.Clamp01(chargeTime / maxChargeTime);
        float damageMultiplier = Mathf.Lerp(minChargeMultiplier, maxChargeMultiplier, chargePercent);
        
        // Fire arrow
        FireArrow(transform.forward, damageMultiplier);
        
        // Reset charge state
        isChargingArrow = false;
        SetAnimationBool("IsDrawing", false);
        PlayAnimation("BowRelease");
        
        lastFireTime = Time.time;
        
        Debug.Log($"Released arrow with {chargePercent * 100}% charge ({damageMultiplier}x damage)");
    }
    
    private void FireArrow(Vector3 direction, float damageMultiplier = 1f)
    {
        if (arrowPrefab == null || arrowSpawnPoint == null) return;
        
        // Spawn arrow
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.LookRotation(direction));
        
        // Set arrow velocity
        Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();
        if (arrowRb != null)
        {
            arrowRb.velocity = direction * arrowSpeed;
        }
        
        // Set arrow damage
        ArrowProjectile arrowScript = arrow.GetComponent<ArrowProjectile>();
        if (arrowScript != null)
        {
            int finalDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);
            arrowScript.Initialize(ownerEntity, finalDamage, weaponType);
        }
        
        // Play sound
        PlaySound(attackSound);
        
        // Destroy arrow after 5 seconds
        Destroy(arrow, 5f);
    }
    
    public override void PerformAbility(string abilityKey)
    {
        switch (abilityKey)
        {
            case "Q":
                MultiShot();
                break;
            case "E":
                ArrowRain();
                break;
            case "R":
                ActivateRapidFire();
                break;
        }
    }
    
    /// <summary>
    /// Multi-Shot - fire multiple arrows at once
    /// </summary>
    private void MultiShot()
    {
        if (!IsAbilityReady(0)) return;
        
        PlayAnimation("MultiShot");
        PlaySound(abilitySounds[0]);
        
        // Calculate spread angles
        float startAngle = -multiShotSpreadAngle * (multiShotArrowCount - 1) / 2f;
        
        for (int i = 0; i < multiShotArrowCount; i++)
        {
            float angle = startAngle + (multiShotSpreadAngle * i);
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            
            FireArrow(direction, 0.7f); // Reduced damage per arrow
        }
        
        Debug.Log($"Fired {multiShotArrowCount} arrows!");
        StartCooldown(0);
    }
    
    /// <summary>
    /// Arrow Rain - AOE attack from above
    /// </summary>
    private void ArrowRain()
    {
        if (!IsAbilityReady(1)) return;
        
        PlayAnimation("ArrowRain");
        PlaySound(abilitySounds[1]);
        
        // TODO: Implement arrow rain - spawn arrows that fall from sky in target area
        Debug.Log("Arrow Rain!");
        
        StartCooldown(1);
    }
    
    /// <summary>
    /// Rapid Fire - increase fire rate temporarily
    /// </summary>
    private void ActivateRapidFire()
    {
        if (!IsAbilityReady(2)) return;
        
        isRapidFireActive = true;
        rapidFireEndTime = Time.time + rapidFireDuration;
        
        PlayAnimation("RapidFire");
        PlaySound(abilitySounds[2]);
        
        Debug.Log($"Rapid Fire activated for {rapidFireDuration}s!");
        StartCooldown(2);
    }
    
    protected override bool CanFire()
    {
        float effectiveFireRate = isRapidFireActive ? fireRate * rapidFireRateMultiplier : fireRate;
        return Time.time - lastFireTime >= (1f / effectiveFireRate);
    }
    
    public override void UpdateWeapon(float deltaTime)
    {
        base.UpdateWeapon(deltaTime);
        
        // Update charge progress
        if (isChargingArrow)
        {
            float chargeTime = Time.time - chargeStartTime;
            chargeProgress = Mathf.Clamp01(chargeTime / maxChargeTime);
            
            // Update animator
            if (animator != null)
            {
                animator.SetFloat("ChargeProgress", chargeProgress);
            }
        }
        
        // Check if rapid fire expired
        if (isRapidFireActive && Time.time >= rapidFireEndTime)
        {
            isRapidFireActive = false;
            Debug.Log("Rapid Fire ended");
        }
    }
}

/// <summary>
/// Arrow projectile script
/// </summary>
public class ArrowProjectile : MonoBehaviour
{
    private Entity owner;
    private int damage;
    private WeaponType weaponType;
    private bool hasHit = false;
    
    public void Initialize(Entity ownerEntity, int arrowDamage, WeaponType type)
    {
        owner = ownerEntity;
        damage = arrowDamage;
        weaponType = type;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;
        
        hasHit = true;
        
        // Check if hit an entity
        Entity target = collision.gameObject.GetComponent<Entity>();
        if (target != null && target != owner)
        {
            // Apply damage
            DamageResult result = DamageCalculator.CalculateDamage(damage, weaponType);
            
            if (CombatSystem.Instance != null)
            {
                CombatSystem.Instance.QueueDamage(
                    target,
                    owner,
                    result.damage,
                    result.damageType,
                    weaponType,
                    result.isCritical,
                    collision.contacts[0].point,
                    collision.contacts[0].normal
                );
            }
        }
        
        // Stick arrow in surface
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
        
        // Destroy after delay
        Destroy(gameObject, 3f);
    }
}

