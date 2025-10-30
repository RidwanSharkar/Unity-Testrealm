using UnityEngine;
using System.Collections;

/// <summary>
/// Mage's Staff weapon
/// Magic spellcasting with various elemental abilities
/// </summary>
public class StaffWeapon : BaseWeapon
{
    [Header("Staff Settings")]
    [SerializeField] private GameObject spellProjectilePrefab;
    [SerializeField] private Transform spellCastPoint;
    [SerializeField] private float spellSpeed = 20f;
    [SerializeField] private GameObject staffGlowEffect;
    
    [Header("Primary Attack - Fireball")]
    [SerializeField] private GameObject primaryFireballPrefab;
    [SerializeField] private int primaryFireballDamage = 15;
    [SerializeField] private float primaryAttackCastTime = 0.5f; // Animation time before fireball spawns
    [SerializeField] private float primaryAttackCooldown = 0.8f; // Time between casts
    [SerializeField] private GameObject castingEffect; // VFX during casting
    private bool isCasting = false;
    private float lastPrimaryAttackTime = 0f;
    
    [Header("Spell Types")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private GameObject iceNovaPrefab;
    [SerializeField] private GameObject lightningStrikePrefab;
    
    [Header("Mana System (Optional)")]
    [SerializeField] private bool useMana = true;
    [SerializeField] private int maxMana = 100;
    [SerializeField] private int currentMana = 100;
    [SerializeField] private int basicSpellManaCost = 10;
    [SerializeField] private float manaRegenRate = 5f; // Mana per second
    
    [Header("Ability Settings")]
    [SerializeField] private float fireballCooldown = 8f;
    [SerializeField] private int fireballDamage = 50;
    [SerializeField] private float fireballExplosionRadius = 3f;
    
    [SerializeField] private float iceNovaCooldown = 12f;
    [SerializeField] private int iceNovaDamage = 30;
    [SerializeField] private float iceNovaRadius = 5f;
    [SerializeField] private float iceNovaSlowDuration = 3f;
    
    [SerializeField] private float lightningStrikeCooldown = 15f;
    [SerializeField] private int lightningStrikeDamage = 80;
    [SerializeField] private float lightningStrikeRange = 20f;
    
    protected override void Awake()
    {
        base.Awake();
        weaponType = WeaponType.Magic;
        weaponName = "Mage's Staff";
        currentMana = maxMana;
    }
    
    protected override void InitializeAbilityCooldowns()
    {
        abilityCooldowns[0] = new AbilityCooldown
        {
            abilityName = "Fireball",
            cooldownTime = fireballCooldown,
            remainingTime = 0f
        };
        
        abilityCooldowns[1] = new AbilityCooldown
        {
            abilityName = "Ice Nova",
            cooldownTime = iceNovaCooldown,
            remainingTime = 0f
        };
        
        abilityCooldowns[2] = new AbilityCooldown
        {
            abilityName = "Lightning Strike",
            cooldownTime = lightningStrikeCooldown,
            remainingTime = 0f
        };
    }
    
    /// <summary>
    /// Primary attack (Left Click) - Cast fireball with animation
    /// </summary>
    public override void PerformPrimaryAttack()
    {
        // Check if we can cast (cooldown and not already casting)
        if (isCasting || Time.time - lastPrimaryAttackTime < primaryAttackCooldown)
            return;
        
        // Check mana
        if (useMana && currentMana < basicSpellManaCost)
        {
            Debug.Log("Not enough mana for fireball!");
            return;
        }
        
        // Start casting coroutine
        StartCoroutine(CastPrimaryFireball());
        
        // Consume mana
        if (useMana)
        {
            currentMana -= basicSpellManaCost;
        }
        
        lastPrimaryAttackTime = Time.time;
    }
    
    /// <summary>
    /// Coroutine for casting fireball with proper timing
    /// </summary>
    private IEnumerator CastPrimaryFireball()
    {
        isCasting = true;
        
        // Play casting animation
        PlayAnimation("Cast");
        PlaySound(attackSound);
        
        // Spawn casting effect at staff tip
        GameObject castEffect = null;
        if (castingEffect != null && spellCastPoint != null)
        {
            castEffect = Instantiate(castingEffect, spellCastPoint.position, spellCastPoint.rotation, spellCastPoint);
        }
        
        // Wait for cast time (animation duration)
        yield return new WaitForSeconds(primaryAttackCastTime);
        
        // Spawn fireball projectile
        SpawnPrimaryFireball();
        
        // Destroy casting effect
        if (castEffect != null)
        {
            Destroy(castEffect);
        }
        
        isCasting = false;
    }
    
    /// <summary>
    /// Spawn the fireball projectile
    /// </summary>
    private void SpawnPrimaryFireball()
    {
        if (primaryFireballPrefab == null || spellCastPoint == null)
        {
            Debug.LogWarning("Primary fireball prefab or cast point not assigned!");
            return;
        }
        
        // Get shoot direction (forward from cast point or camera)
        Vector3 shootDirection = spellCastPoint.forward;
        
        // If owner has a camera, use camera forward for better aiming
        if (ownerEntity != null)
        {
            Camera playerCamera = Camera.main;
            if (playerCamera != null)
            {
                // Shoot towards where the player is looking
                shootDirection = playerCamera.transform.forward;
            }
        }
        
        // Spawn fireball at cast point
        GameObject fireballObj = Instantiate(primaryFireballPrefab, spellCastPoint.position, Quaternion.identity);
        
        // Initialize projectile
        MageFireballProjectile fireball = fireballObj.GetComponent<MageFireballProjectile>();
        if (fireball != null)
        {
            fireball.Initialize(ownerEntity, primaryFireballDamage, weaponType, shootDirection);
        }
        else
        {
            // Fallback to base projectile
            Projectile projectile = fireballObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Initialize(ownerEntity, primaryFireballDamage, weaponType, shootDirection);
            }
        }
        
        Debug.Log($"Launched fireball! ({primaryFireballDamage} damage)");
    }
    
    /// <summary>
    /// Secondary attack (Right Click held) - Continuous spell beam
    /// </summary>
    public override void PerformAttack()
    {
        if (!CanFire()) return;
        
        // Check mana
        if (useMana && currentMana < basicSpellManaCost)
        {
            Debug.Log("Not enough mana!");
            return;
        }
        
        // Cast basic spell projectile
        CastBasicSpell();
        
        // Consume mana
        if (useMana)
        {
            currentMana -= basicSpellManaCost;
        }
        
        lastFireTime = Time.time;
    }
    
    private void CastBasicSpell()
    {
        PlayAnimation("SpellCast");
        PlaySound(attackSound);
        
        if (spellProjectilePrefab == null || spellCastPoint == null) return;
        
        // Spawn spell projectile
        GameObject spell = Instantiate(spellProjectilePrefab, spellCastPoint.position, spellCastPoint.rotation);
        
        // Set velocity
        Rigidbody spellRb = spell.GetComponent<Rigidbody>();
        if (spellRb != null)
        {
            spellRb.velocity = transform.forward * spellSpeed;
        }
        
        // Set damage
        SpellProjectile spellScript = spell.GetComponent<SpellProjectile>();
        if (spellScript != null)
        {
            spellScript.Initialize(ownerEntity, baseDamage, weaponType);
        }
        
        // Destroy after 5 seconds
        Destroy(spell, 5f);
        
        Debug.Log("Cast basic spell!");
    }
    
    public override void PerformAbility(string abilityKey)
    {
        switch (abilityKey)
        {
            case "Q":
                CastFireball();
                break;
            case "E":
                CastIceNova();
                break;
            case "R":
                CastLightningStrike();
                break;
        }
    }
    
    /// <summary>
    /// Fireball - explosive projectile
    /// </summary>
    private void CastFireball()
    {
        if (!IsAbilityReady(0)) return;
        
        PlayAnimation("Fireball");
        PlaySound(abilitySounds[0]);
        
        if (fireballPrefab != null && spellCastPoint != null)
        {
            // Spawn fireball
            GameObject fireball = Instantiate(fireballPrefab, spellCastPoint.position, spellCastPoint.rotation);
            
            // Set velocity
            Rigidbody fireballRb = fireball.GetComponent<Rigidbody>();
            if (fireballRb != null)
            {
                fireballRb.velocity = transform.forward * spellSpeed;
            }
            
            // Set damage and explosion
            FireballProjectile fireballScript = fireball.GetComponent<FireballProjectile>();
            if (fireballScript != null)
            {
                fireballScript.Initialize(ownerEntity, fireballDamage, fireballExplosionRadius, weaponType);
            }
            
            Destroy(fireball, 5f);
        }
        
        Debug.Log($"Cast Fireball! ({fireballDamage} damage, {fireballExplosionRadius}m radius)");
        StartCooldown(0);
    }
    
    /// <summary>
    /// Ice Nova - freeze enemies around caster
    /// </summary>
    private void CastIceNova()
    {
        if (!IsAbilityReady(1)) return;
        
        PlayAnimation("IceNova");
        PlaySound(abilitySounds[1]);
        
        // Create ice nova effect
        if (iceNovaPrefab != null)
        {
            GameObject iceNova = Instantiate(iceNovaPrefab, transform.position, Quaternion.identity);
            Destroy(iceNova, 2f);
        }
        
        // Damage and slow enemies in radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, iceNovaRadius);
        
        foreach (Collider hit in hitColliders)
        {
            Entity target = hit.GetComponent<Entity>();
            if (target != null && target != ownerEntity)
            {
                // Apply damage
                DamageResult result = DamageCalculator.CalculateDamage(iceNovaDamage, weaponType);
                
                if (CombatSystem.Instance != null)
                {
                    CombatSystem.Instance.QueueDamage(
                        target,
                        ownerEntity,
                        result.damage,
                        result.damageType,
                        weaponType,
                        result.isCritical,
                        hit.transform.position,
                        Vector3.up
                    );
                }
                
                // TODO: Apply slow effect
                Debug.Log($"Ice Nova froze enemy for {iceNovaSlowDuration}s");
            }
        }
        
        Debug.Log($"Cast Ice Nova! ({iceNovaDamage} damage, {iceNovaRadius}m radius)");
        StartCooldown(1);
    }
    
    /// <summary>
    /// Lightning Strike - high damage single target
    /// </summary>
    private void CastLightningStrike()
    {
        if (!IsAbilityReady(2)) return;
        
        PlayAnimation("LightningStrike");
        PlaySound(abilitySounds[2]);
        
        // Raycast to find target
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, lightningStrikeRange))
        {
            Entity target = hit.collider.GetComponent<Entity>();
            if (target != null && target != ownerEntity)
            {
                // Spawn lightning effect
                if (lightningStrikePrefab != null)
                {
                    GameObject lightning = Instantiate(lightningStrikePrefab, hit.point, Quaternion.identity);
                    Destroy(lightning, 1f);
                }
                
                // Apply massive damage
                DamageResult result = DamageCalculator.CalculateDamage(lightningStrikeDamage, weaponType);
                
                if (CombatSystem.Instance != null)
                {
                    CombatSystem.Instance.QueueDamage(
                        target,
                        ownerEntity,
                        result.damage,
                        result.damageType,
                        weaponType,
                        result.isCritical,
                        hit.point,
                        hit.normal
                    );
                }
                
                Debug.Log($"Lightning Strike hit! ({result.damage} damage)");
            }
        }
        
        StartCooldown(2);
    }
    
    public override void UpdateWeapon(float deltaTime)
    {
        base.UpdateWeapon(deltaTime);
        
        // Regenerate mana
        if (useMana && currentMana < maxMana)
        {
            currentMana += Mathf.RoundToInt(manaRegenRate * deltaTime);
            currentMana = Mathf.Min(currentMana, maxMana);
        }
        
        // Update staff glow based on mana
        if (staffGlowEffect != null)
        {
            float glowIntensity = (float)currentMana / maxMana;
            // TODO: Update glow effect intensity
        }
    }
    
    // Mana properties
    public int CurrentMana => currentMana;
    public int MaxMana => maxMana;
    public float ManaPercent => (float)currentMana / maxMana;
}

/// <summary>
/// Basic spell projectile
/// </summary>
public class SpellProjectile : MonoBehaviour
{
    private Entity owner;
    private int damage;
    private WeaponType weaponType;
    private bool hasHit = false;
    
    public void Initialize(Entity ownerEntity, int spellDamage, WeaponType type)
    {
        owner = ownerEntity;
        damage = spellDamage;
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
        
        // Destroy spell
        Destroy(gameObject);
    }
}

/// <summary>
/// Fireball projectile with explosion
/// </summary>
public class FireballProjectile : MonoBehaviour
{
    private Entity owner;
    private int damage;
    private float explosionRadius;
    private WeaponType weaponType;
    private bool hasExploded = false;
    
    public void Initialize(Entity ownerEntity, int fireballDamage, float radius, WeaponType type)
    {
        owner = ownerEntity;
        damage = fireballDamage;
        explosionRadius = radius;
        weaponType = type;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (hasExploded) return;
        
        hasExploded = true;
        Explode(collision.contacts[0].point);
    }
    
    private void Explode(Vector3 explosionPoint)
    {
        // TODO: Spawn explosion effect
        
        // Damage all entities in radius
        Collider[] hitColliders = Physics.OverlapSphere(explosionPoint, explosionRadius);
        
        foreach (Collider hit in hitColliders)
        {
            Entity target = hit.GetComponent<Entity>();
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
                        hit.transform.position,
                        Vector3.up
                    );
                }
            }
        }
        
        // Destroy fireball
        Destroy(gameObject);
    }
}

