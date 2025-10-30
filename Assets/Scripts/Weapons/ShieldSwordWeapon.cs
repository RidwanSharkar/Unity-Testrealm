using UnityEngine;

/// <summary>
/// Knight's Shield and Sword weapon
/// Close-range melee combat with defensive capabilities
/// </summary>
public class ShieldSwordWeapon : BaseWeapon
{
    [Header("Shield & Sword Settings")]
    [SerializeField] private GameObject shieldObject;
    [SerializeField] private GameObject swordObject;
    [SerializeField] private float blockDamageReduction = 0.5f; // 50% damage reduction when blocking
    [SerializeField] private float comboWindow = 0.5f; // Time window for combo attacks
    
    [Header("Attack Chain")]
    [SerializeField] private int[] comboBaseDamage = { 15, 20, 30 }; // 3-hit combo
    [SerializeField] private float[] comboFireRates = { 1.2f, 1.5f, 1.0f };
    
    [Header("Shield Abilities")]
    [SerializeField] private float shieldBashDamage = 25f;
    [SerializeField] private float shieldBashCooldown = 8f;
    [SerializeField] private float shieldBashStunDuration = 2f;
    
    // State
    private int currentComboIndex = 0;
    private float lastAttackTime = 0f;
    private bool isBlocking = false;
    
    protected override void Awake()
    {
        base.Awake();
        weaponType = WeaponType.Sword;
        weaponName = "Knight's Sword & Shield";
    }
    
    protected override void InitializeAbilityCooldowns()
    {
        abilityCooldowns[0] = new AbilityCooldown
        {
            abilityName = "Shield Bash",
            cooldownTime = shieldBashCooldown,
            remainingTime = 0f
        };
        
        abilityCooldowns[1] = new AbilityCooldown
        {
            abilityName = "Block",
            cooldownTime = 0f,
            remainingTime = 0f
        };
        
        abilityCooldowns[2] = new AbilityCooldown
        {
            abilityName = "Shield Charge",
            cooldownTime = 12f,
            remainingTime = 0f
        };
    }
    
    public override void PerformAttack()
    {
        if (isBlocking) return; // Can't attack while blocking
        if (!CanFire()) return;
        
        // Check if we're in combo window
        float timeSinceLastAttack = Time.time - lastAttackTime;
        if (timeSinceLastAttack > comboWindow)
        {
            // Reset combo if window expired
            currentComboIndex = 0;
        }
        
        // Perform current combo attack
        PerformComboAttack(currentComboIndex);
        
        // Update combo index
        currentComboIndex = (currentComboIndex + 1) % comboBaseDamage.Length;
        lastAttackTime = Time.time;
        lastFireTime = Time.time;
    }
    
    private void PerformComboAttack(int comboIndex)
    {
        // Set damage based on combo
        int comboDamage = comboBaseDamage[comboIndex];
        
        // Play animation
        PlayAnimation($"Attack{comboIndex + 1}");
        
        // Play sound
        PlaySound(attackSound);
        
        // Detect enemies in range
        Collider[] hitColliders = Physics.OverlapSphere(attackPoint.position, attackRange);
        
        foreach (Collider hit in hitColliders)
        {
            Entity target = hit.GetComponent<Entity>();
            if (target != null && target != ownerEntity)
            {
                // Calculate damage
                DamageResult result = DamageCalculator.CalculateDamage(comboDamage, weaponType);
                
                // Apply damage
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
                
                Debug.Log($"Sword combo hit {comboIndex + 1}: {result.damage} damage");
            }
        }
        
        // Spawn visual effect
        if (attackPoint != null)
        {
            SpawnAttackEffect(attackPoint.position, attackPoint.rotation);
        }
    }
    
    public override void PerformAbility(string abilityKey)
    {
        switch (abilityKey)
        {
            case "Q":
                ShieldBash();
                break;
            case "E":
                ToggleBlock();
                break;
            case "R":
                ShieldCharge();
                break;
        }
    }
    
    /// <summary>
    /// Shield Bash - stun enemies in front
    /// </summary>
    private void ShieldBash()
    {
        if (!IsAbilityReady(0)) return;
        
        PlayAnimation("ShieldBash");
        PlaySound(abilitySounds[0]);
        
        // Detect enemies in cone in front
        Collider[] hitColliders = Physics.OverlapSphere(attackPoint.position, attackRange * 1.5f);
        
        foreach (Collider hit in hitColliders)
        {
            // Check if enemy is in front
            Vector3 directionToTarget = (hit.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToTarget);
            
            if (angle < 60f) // 120-degree cone
            {
                Entity target = hit.GetComponent<Entity>();
                if (target != null && target != ownerEntity)
                {
                    // Apply damage and stun
                    DamageResult result = DamageCalculator.CalculateDamage(Mathf.RoundToInt(shieldBashDamage), weaponType);
                    
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
                    
                    // TODO: Apply stun effect
                    Debug.Log($"Shield Bash stunned enemy for {shieldBashStunDuration}s");
                }
            }
        }
        
        StartCooldown(0);
    }
    
    /// <summary>
    /// Toggle blocking stance
    /// </summary>
    private void ToggleBlock()
    {
        isBlocking = !isBlocking;
        
        if (isBlocking)
        {
            PlayAnimation("BlockStart");
            SetAnimationBool("IsBlocking", true);
            Debug.Log("Blocking - damage reduced by 50%");
        }
        else
        {
            SetAnimationBool("IsBlocking", false);
            Debug.Log("Stopped blocking");
        }
    }
    
    /// <summary>
    /// Shield Charge - dash forward with shield
    /// </summary>
    private void ShieldCharge()
    {
        if (!IsAbilityReady(2)) return;
        
        PlayAnimation("ShieldCharge");
        PlaySound(abilitySounds[2]);
        
        // TODO: Implement dash forward and knockback enemies
        Debug.Log("Shield Charge!");
        
        StartCooldown(2);
    }
    
    /// <summary>
    /// Apply block damage reduction
    /// </summary>
    public float GetBlockDamageReduction()
    {
        return isBlocking ? blockDamageReduction : 0f;
    }
    
    public override void UpdateWeapon(float deltaTime)
    {
        base.UpdateWeapon(deltaTime);
        
        // Keep blocking animation active
        if (isBlocking && animator != null)
        {
            animator.SetBool("IsBlocking", true);
        }
    }
}

