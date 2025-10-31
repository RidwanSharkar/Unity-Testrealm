using UnityEngine;
using System.Collections;

/// <summary>
/// Direct spell casting for Mage character without weapon system
/// Casts spells from hands/body, not from a weapon
/// </summary>
public class MageSpellCaster : MonoBehaviour
{
    [Header("Spell Casting Setup")]
    [SerializeField] private Animator animator;
    [SerializeField] private Entity ownerEntity;
    [SerializeField] private Transform leftHandCastPoint;  // Optional: cast from left hand
    [SerializeField] private Transform rightHandCastPoint; // Optional: cast from right hand
    [SerializeField] private Transform chestCastPoint;     // Optional: cast from chest/center
    
    [Header("Animation Fix")]
    [SerializeField] private bool preventVerticalMovement = true; // Prevents sinking/floating during animations
    private CharacterController characterController;
    private Vector3 positionBeforeCast;
    
    [Header("Primary Attack - Fireball")]
    [SerializeField] private GameObject primaryFireballPrefab;
    [SerializeField] private int primaryFireballDamage = 15;
    [SerializeField] private float primaryAttackCastTime = 0.5f; // Animation time before fireball spawns
    [SerializeField] private float primaryAttackCooldown = 0.8f; // Time between casts
    [SerializeField] private GameObject castingEffect; // VFX during casting
    
    [Header("Spiral Effect (Optional)")]
    [SerializeField] private bool useSpiralEffect = false; // Enable twin spiral projectiles
    [SerializeField] private GameObject spiralPairPrefab; // Prefab with SpiralProjectilePair component
    [SerializeField] private float spiralRadius = 0.5f;
    [SerializeField] private float spiralSpeed = 360f; // Rotation speed in degrees/second
    
    private bool isCasting = false;
    private float lastPrimaryAttackTime = 0f;
    
    [Header("Mana System")]
    [SerializeField] private bool useMana = true;
    [SerializeField] private int maxMana = 100;
    [SerializeField] private int currentMana = 100;
    [SerializeField] private int basicSpellManaCost = 10;
    [SerializeField] private float manaRegenRate = 5f; // Mana per second
    
    [Header("Spell Abilities")]
    [SerializeField] private GameObject fireballAbilityPrefab; // Q ability
    [SerializeField] private float fireballCooldown = 8f;
    [SerializeField] private int fireballDamage = 50;
    [SerializeField] private float fireballExplosionRadius = 3f;
    private float lastFireballTime = 0f;
    
    [SerializeField] private GameObject iceNovaPrefab; // E ability
    [SerializeField] private float iceNovaCooldown = 12f;
    [SerializeField] private int iceNovaDamage = 30;
    [SerializeField] private float iceNovaRadius = 5f;
    private float lastIceNovaTime = 0f;
    
    [SerializeField] private GameObject lightningStrikePrefab; // R ability
    [SerializeField] private float lightningStrikeCooldown = 15f;
    [SerializeField] private int lightningStrikeDamage = 80;
    [SerializeField] private float lightningStrikeRange = 20f;
    private float lastLightningStrikeTime = 0f;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip castSound;
    [SerializeField] private AudioClip fireballSound;
    [SerializeField] private AudioClip iceNovaSound;
    [SerializeField] private AudioClip lightningSound;
    
    private void Awake()
    {
        // Auto-find components if not assigned
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
            
            if (animator != null)
            {
                Debug.Log($"[MageSpellCaster] Found Animator: {animator.gameObject.name}");
                
                // CRITICAL: Disable Apply Root Motion to prevent sinking/floating
                if (animator.applyRootMotion)
                {
                    Debug.LogWarning("[MageSpellCaster] Apply Root Motion is enabled! Disabling it to prevent character sinking during animations.");
                    animator.applyRootMotion = false;
                }
            }
            else
            {
                Debug.LogError("[MageSpellCaster] ANIMATOR NOT FOUND! Casting animations will not play. Please assign the Animator component in the Inspector.");
            }
        }
        
        if (ownerEntity == null)
        {
            ownerEntity = GetComponent<Entity>();
            if (ownerEntity != null)
            {
                Debug.Log($"[MageSpellCaster] Found Entity: {ownerEntity.gameObject.name}");
            }
        }
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        // Get CharacterController for position locking
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogWarning("[MageSpellCaster] No CharacterController found. Vertical position lock may not work perfectly.");
        }
        
        // Initialize mana
        currentMana = maxMana;
        
        // Auto-create cast points if not assigned
        if (chestCastPoint == null)
        {
            GameObject castPoint = new GameObject("ChestCastPoint");
            castPoint.transform.SetParent(transform);
            castPoint.transform.localPosition = new Vector3(0, 1.5f, 0.5f); // Approximate chest height
            chestCastPoint = castPoint.transform;
            Debug.Log("[MageSpellCaster] Auto-created ChestCastPoint at chest height");
        }
    }
    
    private void Update()
    {
        // Regenerate mana
        if (useMana && currentMana < maxMana)
        {
            currentMana += Mathf.RoundToInt(manaRegenRate * Time.deltaTime);
            currentMana = Mathf.Min(currentMana, maxMana);
        }
    }
    
    /// <summary>
    /// Primary attack (Left Click) - Cast fireball with animation
    /// </summary>
    public void PerformPrimaryAttack()
    {
        Debug.Log($"[MageSpellCaster] PerformPrimaryAttack called! isCasting={isCasting}, cooldownRemaining={Mathf.Max(0, primaryAttackCooldown - (Time.time - lastPrimaryAttackTime))}, mana={currentMana}/{maxMana}");
        
        // Check if we can cast (cooldown and not already casting)
        if (isCasting)
        {
            Debug.LogWarning("[MageSpellCaster] Already casting! Cannot cast again.");
            return;
        }
        
        if (Time.time - lastPrimaryAttackTime < primaryAttackCooldown)
        {
            Debug.LogWarning($"[MageSpellCaster] On cooldown! Wait {(primaryAttackCooldown - (Time.time - lastPrimaryAttackTime)):F2} seconds.");
            return;
        }
        
        // Check mana
        if (useMana && currentMana < basicSpellManaCost)
        {
            Debug.LogWarning($"[MageSpellCaster] Not enough mana for fireball! Need {basicSpellManaCost}, have {currentMana}");
            return;
        }
        
        // Start casting coroutine
        Debug.Log("[MageSpellCaster] Starting fireball cast!");
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
        
        // Store position before cast to prevent sinking
        if (preventVerticalMovement)
        {
            positionBeforeCast = transform.position;
            Debug.Log($"[MageSpellCaster] Locked position at Y={positionBeforeCast.y} to prevent sinking");
        }
        
        // Play casting animation
        Debug.Log("[MageSpellCaster] Playing 'Cast' animation trigger");
        PlayAnimation("Cast");
        PlaySound(castSound);
        
        // Determine cast point (prefer hands, fallback to chest)
        Transform castPoint = GetBestCastPoint();
        
        // Spawn casting effect at cast point
        GameObject castEffect = null;
        if (castingEffect != null && castPoint != null)
        {
            castEffect = Instantiate(castingEffect, castPoint.position, castPoint.rotation, castPoint);
            Debug.Log("[MageSpellCaster] Spawned casting effect");
        }
        
        // Wait for cast time (animation duration) while maintaining vertical position
        float elapsedTime = 0f;
        while (elapsedTime < primaryAttackCastTime)
        {
            // Prevent sinking by locking Y position during animation
            if (preventVerticalMovement && characterController != null)
            {
                Vector3 currentPos = transform.position;
                if (Mathf.Abs(currentPos.y - positionBeforeCast.y) > 0.01f)
                {
                    // Restore Y position if it changed
                    transform.position = new Vector3(currentPos.x, positionBeforeCast.y, currentPos.z);
                }
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        Debug.Log($"[MageSpellCaster] Cast animation complete!");
        
        // Spawn fireball projectile
        Debug.Log("[MageSpellCaster] Spawning fireball now...");
        SpawnPrimaryFireball(castPoint);
        
        // Destroy casting effect
        if (castEffect != null)
        {
            Destroy(castEffect);
        }
        
        // Ensure final position is correct
        if (preventVerticalMovement)
        {
            Vector3 finalPos = transform.position;
            transform.position = new Vector3(finalPos.x, positionBeforeCast.y, finalPos.z);
            Debug.Log($"[MageSpellCaster] Reset position to Y={positionBeforeCast.y}");
        }
        
        isCasting = false;
        Debug.Log("[MageSpellCaster] Casting finished, ready for next cast");
    }
    
    /// <summary>
    /// Spawn the fireball projectile
    /// </summary>
    private void SpawnPrimaryFireball(Transform castPoint)
    {
        if (castPoint == null)
        {
            Debug.LogError("[MageSpellCaster] Cast Point is NULL! Using character position as fallback.");
            castPoint = transform;
        }
        
        // Get shoot direction (camera forward for better aiming)
        Vector3 shootDirection = castPoint.forward;
        
        Camera playerCamera = Camera.main;
        if (playerCamera != null)
        {
            // Shoot towards where the player is looking
            shootDirection = playerCamera.transform.forward;
            Debug.Log($"[MageSpellCaster] Aiming towards camera direction: {shootDirection}");
        }
        
        // Check if using spiral effect
        if (useSpiralEffect && spiralPairPrefab != null)
        {
            SpawnSpiralProjectile(castPoint, shootDirection);
        }
        else
        {
            SpawnSingleProjectile(castPoint, shootDirection);
        }
    }
    
    /// <summary>
    /// Spawn a single fireball projectile
    /// </summary>
    private void SpawnSingleProjectile(Transform castPoint, Vector3 shootDirection)
    {
        if (primaryFireballPrefab == null)
        {
            Debug.LogError("[MageSpellCaster] Primary fireball prefab is NOT ASSIGNED! Please assign it in the Inspector.");
            return;
        }
        
        // Spawn fireball at cast point
        Debug.Log($"[MageSpellCaster] Instantiating single fireball at position: {castPoint.position}");
        GameObject fireballObj = Instantiate(primaryFireballPrefab, castPoint.position, Quaternion.identity);
        
        // Initialize projectile
        MageFireballProjectile fireball = fireballObj.GetComponent<MageFireballProjectile>();
        if (fireball != null)
        {
            Debug.Log($"[MageSpellCaster] Initializing MageFireballProjectile with {primaryFireballDamage} damage");
            fireball.Initialize(ownerEntity, primaryFireballDamage, WeaponType.Magic, shootDirection);
        }
        else
        {
            // Fallback to base projectile
            Projectile projectile = fireballObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                Debug.Log($"[MageSpellCaster] Initializing base Projectile with {primaryFireballDamage} damage");
                projectile.Initialize(ownerEntity, primaryFireballDamage, WeaponType.Magic, shootDirection);
            }
            else
            {
                Debug.LogError("[MageSpellCaster] Fireball prefab has NO Projectile component! Please add MageFireballProjectile or Projectile component to the prefab.");
            }
        }
        
        Debug.Log($"<color=green>[MageSpellCaster] ✓ Fireball launched successfully! ({primaryFireballDamage} damage)</color>");
    }
    
    /// <summary>
    /// Spawn a spiral projectile pair
    /// </summary>
    private void SpawnSpiralProjectile(Transform castPoint, Vector3 shootDirection)
    {
        if (spiralPairPrefab == null)
        {
            Debug.LogError("[MageSpellCaster] Spiral Pair Prefab is NOT ASSIGNED! Falling back to single projectile.");
            SpawnSingleProjectile(castPoint, shootDirection);
            return;
        }
        
        // Spawn spiral pair at cast point
        Debug.Log($"[MageSpellCaster] Instantiating SPIRAL projectile pair at position: {castPoint.position}");
        GameObject spiralObj = Instantiate(spiralPairPrefab, castPoint.position, Quaternion.identity);
        
        // Initialize spiral pair
        SpiralProjectilePair spiral = spiralObj.GetComponent<SpiralProjectilePair>();
        if (spiral != null)
        {
            Debug.Log($"[MageSpellCaster] Initializing SpiralProjectilePair with {primaryFireballDamage} damage");
            spiral.Initialize(ownerEntity, primaryFireballDamage, WeaponType.Magic, shootDirection);
        }
        else
        {
            Debug.LogError("[MageSpellCaster] Spiral Pair Prefab has NO SpiralProjectilePair component! Please add it.");
            Destroy(spiralObj);
            return;
        }
        
        Debug.Log($"<color=cyan>[MageSpellCaster] ✓ Spiral projectiles launched! ({primaryFireballDamage} damage)</color>");
    }
    
    /// <summary>
    /// Cast Fireball ability (Q key)
    /// </summary>
    public void CastFireballAbility()
    {
        if (Time.time - lastFireballTime < fireballCooldown)
        {
            Debug.LogWarning($"[MageSpellCaster] Fireball on cooldown! {(fireballCooldown - (Time.time - lastFireballTime)):F1}s remaining");
            return;
        }
        
        Debug.Log("[MageSpellCaster] Casting Fireball ability!");
        PlayAnimation("Fireball");
        PlaySound(fireballSound);
        
        Transform castPoint = GetBestCastPoint();
        
        if (fireballAbilityPrefab != null && castPoint != null)
        {
            // Spawn explosive fireball
            GameObject fireball = Instantiate(fireballAbilityPrefab, castPoint.position, castPoint.rotation);
            
            // Set velocity towards camera direction
            Rigidbody fireballRb = fireball.GetComponent<Rigidbody>();
            if (fireballRb != null)
            {
                Vector3 direction = Camera.main != null ? Camera.main.transform.forward : transform.forward;
                fireballRb.velocity = direction * 20f;
            }
            
            // Set damage and explosion
            FireballProjectile fireballScript = fireball.GetComponent<FireballProjectile>();
            if (fireballScript != null)
            {
                fireballScript.Initialize(ownerEntity, fireballDamage, fireballExplosionRadius, WeaponType.Magic);
            }
            
            Destroy(fireball, 5f);
        }
        
        Debug.Log($"[MageSpellCaster] Cast Fireball! ({fireballDamage} damage, {fireballExplosionRadius}m radius)");
        lastFireballTime = Time.time;
    }
    
    /// <summary>
    /// Cast Ice Nova ability (E key)
    /// </summary>
    public void CastIceNova()
    {
        if (Time.time - lastIceNovaTime < iceNovaCooldown)
        {
            Debug.LogWarning($"[MageSpellCaster] Ice Nova on cooldown! {(iceNovaCooldown - (Time.time - lastIceNovaTime)):F1}s remaining");
            return;
        }
        
        Debug.Log("[MageSpellCaster] Casting Ice Nova!");
        PlayAnimation("IceNova");
        PlaySound(iceNovaSound);
        
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
                DamageResult result = DamageCalculator.CalculateDamage(iceNovaDamage, WeaponType.Magic);
                
                if (CombatSystem.Instance != null)
                {
                    CombatSystem.Instance.QueueDamage(
                        target,
                        ownerEntity,
                        result.damage,
                        result.damageType,
                        WeaponType.Magic,
                        result.isCritical,
                        hit.transform.position,
                        Vector3.up
                    );
                }
                
                Debug.Log($"[MageSpellCaster] Ice Nova hit {target.name}!");
            }
        }
        
        Debug.Log($"[MageSpellCaster] Cast Ice Nova! ({iceNovaDamage} damage, {iceNovaRadius}m radius)");
        lastIceNovaTime = Time.time;
    }
    
    /// <summary>
    /// Cast Lightning Strike ability (R key)
    /// </summary>
    public void CastLightningStrike()
    {
        if (Time.time - lastLightningStrikeTime < lightningStrikeCooldown)
        {
            Debug.LogWarning($"[MageSpellCaster] Lightning Strike on cooldown! {(lightningStrikeCooldown - (Time.time - lastLightningStrikeTime)):F1}s remaining");
            return;
        }
        
        Debug.Log("[MageSpellCaster] Casting Lightning Strike!");
        PlayAnimation("LightningStrike");
        PlaySound(lightningSound);
        
        // Raycast to find target
        Ray ray = new Ray(transform.position + Vector3.up, Camera.main != null ? Camera.main.transform.forward : transform.forward);
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
                DamageResult result = DamageCalculator.CalculateDamage(lightningStrikeDamage, WeaponType.Magic);
                
                if (CombatSystem.Instance != null)
                {
                    CombatSystem.Instance.QueueDamage(
                        target,
                        ownerEntity,
                        result.damage,
                        result.damageType,
                        WeaponType.Magic,
                        result.isCritical,
                        hit.point,
                        hit.normal
                    );
                }
                
                Debug.Log($"[MageSpellCaster] Lightning Strike hit {target.name}! ({result.damage} damage)");
            }
        }
        
        lastLightningStrikeTime = Time.time;
    }
    
    /// <summary>
    /// Get the best cast point based on what's available
    /// Priority: Right Hand > Left Hand > Chest > Character Center
    /// </summary>
    private Transform GetBestCastPoint()
    {
        if (rightHandCastPoint != null) return rightHandCastPoint;
        if (leftHandCastPoint != null) return leftHandCastPoint;
        if (chestCastPoint != null) return chestCastPoint;
        return transform; // Fallback to character center
    }
    
    /// <summary>
    /// Play animation trigger
    /// </summary>
    private void PlayAnimation(string triggerName)
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
            Debug.Log($"[MageSpellCaster] Triggered animation: {triggerName}");
        }
        else
        {
            Debug.LogWarning($"[MageSpellCaster] Cannot play animation '{triggerName}' - Animator is null!");
        }
    }
    
    /// <summary>
    /// Play sound effect
    /// </summary>
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    // Public properties for UI/debugging
    public int CurrentMana => currentMana;
    public int MaxMana => maxMana;
    public float ManaPercent => (float)currentMana / maxMana;
    public bool IsCasting => isCasting;
    public float PrimaryAttackCooldownRemaining => Mathf.Max(0, primaryAttackCooldown - (Time.time - lastPrimaryAttackTime));
    public float FireballCooldownRemaining => Mathf.Max(0, fireballCooldown - (Time.time - lastFireballTime));
    public float IceNovaCooldownRemaining => Mathf.Max(0, iceNovaCooldown - (Time.time - lastIceNovaTime));
    public float LightningStrikeCooldownRemaining => Mathf.Max(0, lightningStrikeCooldown - (Time.time - lastLightningStrikeTime));
}

