using UnityEngine;

/// <summary>
/// Character class types for different playstyles
/// </summary>
public enum CharacterClassType
{
    Knight,   // Shield and Sword - Tank/Melee
    Archer,   // Bow and Arrow - Ranged DPS
    Mage      // Spellcaster - Magic DPS/Support
}

/// <summary>
/// Character class stats and abilities
/// Each class has unique stats, weapons, and playstyle
/// </summary>
public class CharacterClass : MonoBehaviour
{
    [Header("Class Identity")]
    [SerializeField] private CharacterClassType classType = CharacterClassType.Knight;
    [SerializeField] private string className = "Knight";
    [SerializeField] private string classDescription = "A heavily armored warrior with sword and shield";
    
    [Header("Base Stats")]
    [SerializeField] private int baseHealth = 100;
    [SerializeField] private int baseArmor = 10;
    [SerializeField] private float baseMovementSpeed = 5f;
    [SerializeField] private float baseDamage = 15f;
    
    [Header("Class Modifiers")]
    [SerializeField] private float healthModifier = 1.0f;      // Knight: 1.5x, Archer: 0.8x, Mage: 0.7x
    [SerializeField] private float armorModifier = 1.0f;       // Knight: 1.5x, Archer: 1.0x, Mage: 0.5x
    [SerializeField] private float speedModifier = 1.0f;       // Knight: 0.9x, Archer: 1.2x, Mage: 1.0x
    [SerializeField] private float damageModifier = 1.0f;      // Knight: 1.2x, Archer: 1.0x, Mage: 1.5x
    
    [Header("Class Weapons")]
    [SerializeField] private BaseWeapon[] classWeapons;        // Weapons available to this class
    [SerializeField] private GameObject weaponVisualPrefab;     // Visual weapon model (sword, bow, staff)
    
    [Header("Class Abilities")]
    [SerializeField] private string ability1Name = "Shield Bash";
    [SerializeField] private string ability2Name = "Charge";
    [SerializeField] private string ability3Name = "Defensive Stance";
    [SerializeField] private float ability1Cooldown = 8f;
    [SerializeField] private float ability2Cooldown = 12f;
    [SerializeField] private float ability3Cooldown = 20f;
    
    [Header("Visual Settings")]
    [SerializeField] private GameObject characterModel;         // The character mesh/model
    [SerializeField] private RuntimeAnimatorController animatorController;
    [SerializeField] private Avatar characterAvatar;
    
    [Header("Audio")]
    [SerializeField] private AudioClip[] classVoiceLines;       // Character voice lines
    [SerializeField] private AudioClip[] abilityEffectSounds;   // Ability sound effects
    
    // Components
    private Animator animator;
    private HealthComponent healthComponent;
    private MovementComponent movementComponent;
    private PlayerController playerController;
    
    // Properties
    public CharacterClassType ClassType => classType;
    public string ClassName => className;
    public string ClassDescription => classDescription;
    public int FinalHealth => Mathf.RoundToInt(baseHealth * healthModifier);
    public int FinalArmor => Mathf.RoundToInt(baseArmor * armorModifier);
    public float FinalMovementSpeed => baseMovementSpeed * speedModifier;
    public float FinalDamage => baseDamage * damageModifier;
    
    void Awake()
    {
        // Get components
        animator = GetComponentInChildren<Animator>();
        healthComponent = GetComponent<HealthComponent>();
        movementComponent = GetComponent<MovementComponent>();
        playerController = GetComponent<PlayerController>();
        
        // Apply class-specific settings on awake
        ApplyClassModifiers();
    }
    
    void Start()
    {
        // Setup visual model and animations
        SetupCharacterVisuals();
        
        // Setup weapons
        SetupClassWeapons();
    }
    
    /// <summary>
    /// Apply class-specific stat modifiers
    /// </summary>
    private void ApplyClassModifiers()
    {
        // Apply health modifier
        if (healthComponent != null)
        {
            int modifiedMaxHealth = FinalHealth;
            healthComponent.SetMaxHealth(modifiedMaxHealth);
        }
        
        // Apply movement speed modifier (if MovementComponent supports it)
        // This would be handled by MovementComponent reading from this class
        
        Debug.Log($"Applied {className} class modifiers - HP: {FinalHealth}, Armor: {FinalArmor}, Speed: {FinalMovementSpeed}");
    }
    
    /// <summary>
    /// Setup character visual model and animations
    /// </summary>
    private void SetupCharacterVisuals()
    {
        if (animator != null && animatorController != null)
        {
            animator.runtimeAnimatorController = animatorController;
        }
        
        if (animator != null && characterAvatar != null)
        {
            animator.avatar = characterAvatar;
        }
        
        Debug.Log($"{className} visuals setup complete");
    }
    
    /// <summary>
    /// Setup weapons available to this class
    /// </summary>
    private void SetupClassWeapons()
    {
        if (classWeapons == null || classWeapons.Length == 0)
        {
            Debug.LogWarning($"{className} has no weapons assigned!");
            return;
        }
        
        // Instantiate weapon visual if provided
        if (weaponVisualPrefab != null && animator != null)
        {
            // Find weapon socket/attachment point (usually hand bone)
            Transform weaponSocket = FindWeaponSocket();
            
            if (weaponSocket != null)
            {
                GameObject weaponInstance = Instantiate(weaponVisualPrefab, weaponSocket);
                weaponInstance.transform.localPosition = Vector3.zero;
                weaponInstance.transform.localRotation = Quaternion.identity;
                
                Debug.Log($"{className} weapon visual attached to {weaponSocket.name}");
            }
        }
    }
    
    /// <summary>
    /// Find the weapon socket bone in the character rig
    /// Typically the right hand bone for weapons
    /// </summary>
    private Transform FindWeaponSocket()
    {
        if (animator == null) return null;
        
        // Try to find common weapon socket names
        string[] socketNames = new string[]
        {
            "RightHand",
            "mixamorig:RightHand",
            "Right_Hand",
            "RightHandIndex1",
            "mixamorig:RightHandIndex1"
        };
        
        foreach (string socketName in socketNames)
        {
            Transform socket = FindChildRecursive(animator.transform, socketName);
            if (socket != null)
            {
                return socket;
            }
        }
        
        Debug.LogWarning($"Could not find weapon socket for {className}. Searched for: {string.Join(", ", socketNames)}");
        return null;
    }
    
    /// <summary>
    /// Recursively search for a child transform by name
    /// </summary>
    private Transform FindChildRecursive(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;
            
            Transform result = FindChildRecursive(child, childName);
            if (result != null)
                return result;
        }
        return null;
    }
    
    /// <summary>
    /// Perform class-specific ability
    /// </summary>
    public void PerformAbility(int abilityIndex)
    {
        switch (classType)
        {
            case CharacterClassType.Knight:
                PerformKnightAbility(abilityIndex);
                break;
            case CharacterClassType.Archer:
                PerformArcherAbility(abilityIndex);
                break;
            case CharacterClassType.Mage:
                PerformMageAbility(abilityIndex);
                break;
        }
    }
    
    #region Knight Abilities
    
    private void PerformKnightAbility(int abilityIndex)
    {
        switch (abilityIndex)
        {
            case 1: // Shield Bash
                ShieldBash();
                break;
            case 2: // Charge
                Charge();
                break;
            case 3: // Defensive Stance
                DefensiveStance();
                break;
        }
    }
    
    private void ShieldBash()
    {
        Debug.Log("Knight: Shield Bash!");
        // TODO: Implement shield bash - stun nearby enemies
        if (animator != null)
        {
            animator.SetTrigger("ShieldBash");
        }
    }
    
    private void Charge()
    {
        Debug.Log("Knight: Charge!");
        // TODO: Implement charge - dash forward and knockback enemies
        if (animator != null)
        {
            animator.SetTrigger("Charge");
        }
    }
    
    private void DefensiveStance()
    {
        Debug.Log("Knight: Defensive Stance!");
        // TODO: Implement defensive stance - reduce damage taken
        if (animator != null)
        {
            animator.SetTrigger("DefensiveStance");
        }
    }
    
    #endregion
    
    #region Archer Abilities
    
    private void PerformArcherAbility(int abilityIndex)
    {
        switch (abilityIndex)
        {
            case 1: // Multi-Shot
                MultiShot();
                break;
            case 2: // Arrow Rain
                ArrowRain();
                break;
            case 3: // Rapid Fire
                RapidFire();
                break;
        }
    }
    
    private void MultiShot()
    {
        Debug.Log("Archer: Multi-Shot!");
        // TODO: Implement multi-shot - fire multiple arrows at once
        if (animator != null)
        {
            animator.SetTrigger("MultiShot");
        }
    }
    
    private void ArrowRain()
    {
        Debug.Log("Archer: Arrow Rain!");
        // TODO: Implement arrow rain - AOE attack from above
        if (animator != null)
        {
            animator.SetTrigger("ArrowRain");
        }
    }
    
    private void RapidFire()
    {
        Debug.Log("Archer: Rapid Fire!");
        // TODO: Implement rapid fire - shoot arrows quickly
        if (animator != null)
        {
            animator.SetTrigger("RapidFire");
        }
    }
    
    #endregion
    
    #region Mage Abilities
    
    private void PerformMageAbility(int abilityIndex)
    {
        switch (abilityIndex)
        {
            case 1: // Fireball
                Fireball();
                break;
            case 2: // Ice Nova
                IceNova();
                break;
            case 3: // Lightning Strike
                LightningStrike();
                break;
        }
    }
    
    private void Fireball()
    {
        Debug.Log("Mage: Fireball!");
        // TODO: Implement fireball - projectile spell
        if (animator != null)
        {
            animator.SetTrigger("Fireball");
        }
    }
    
    private void IceNova()
    {
        Debug.Log("Mage: Ice Nova!");
        // TODO: Implement ice nova - freeze nearby enemies
        if (animator != null)
        {
            animator.SetTrigger("IceNova");
        }
    }
    
    private void LightningStrike()
    {
        Debug.Log("Mage: Lightning Strike!");
        // TODO: Implement lightning strike - high damage single target
        if (animator != null)
        {
            animator.SetTrigger("LightningStrike");
        }
    }
    
    #endregion
    
    /// <summary>
    /// Get class-specific weapon damage modifier
    /// </summary>
    public float GetWeaponDamageMultiplier()
    {
        return damageModifier;
    }
    
    /// <summary>
    /// Get class-specific ability info
    /// </summary>
    public string GetAbilityName(int index)
    {
        switch (index)
        {
            case 1: return ability1Name;
            case 2: return ability2Name;
            case 3: return ability3Name;
            default: return "Unknown";
        }
    }
    
    public float GetAbilityCooldown(int index)
    {
        switch (index)
        {
            case 1: return ability1Cooldown;
            case 2: return ability2Cooldown;
            case 3: return ability3Cooldown;
            default: return 0f;
        }
    }
}

