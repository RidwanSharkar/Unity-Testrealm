using UnityEngine;

/// <summary>
/// Weapon types available in the game
/// </summary>
public enum WeaponType 
{ 
    Sword,      // Tank class - melee
    Scythe,     // Healer class - melee with healing
    Bow,        // Ranged
    Runeblade,  // Magic melee
    Sabres,     // Dual-wield melee
    Magic       // Mage staff - spellcasting
}

/// <summary>
/// Damage types for different attack categories
/// </summary>
public enum DamageType
{
    Physical,
    Magical,
    True,      // Ignores armor
    Healing    // Negative damage (healing)
}

/// <summary>
/// Static utility class for damage calculations.
/// Handles critical hits, weapon bonuses, and rune modifiers.
/// </summary>
public static class DamageCalculator
{
    // Base critical chance and damage
    private const float BASE_CRIT_CHANCE = 0.11f;
    private const float BASE_CRIT_MULTIPLIER = 2.0f;
    
    // Rune bonuses
    private const float CRIT_RUNE_BONUS = 0.03f;
    private const float CRIT_DAMAGE_RUNE_BONUS = 0.15f;
    
    // Weapon-specific bonuses
    private const float BOW_PASSIVE_CRIT_BONUS = 0.05f;
    private const float SWORD_PASSIVE_DAMAGE_BONUS = 1.15f;
    private const float SCYTHE_PASSIVE_HEALING_BONUS = 1.25f;
    
    /// <summary>
    /// Calculate damage with critical hit chance
    /// </summary>
    public static DamageResult CalculateDamage(int baseAmount, WeaponType weaponType, DamageType damageType = DamageType.Physical)
    {
        // Calculate critical chance
        float critChance = CalculateCriticalChance(weaponType);
        
        // Determine if critical hit
        bool isCritical = Random.value < critChance;
        
        // Calculate critical multiplier
        float critMultiplier = CalculateCriticalMultiplier();
        
        // Apply weapon-specific bonuses
        float weaponMultiplier = GetWeaponDamageMultiplier(weaponType);
        
        // Calculate final damage
        int finalDamage = baseAmount;
        finalDamage = Mathf.RoundToInt(finalDamage * weaponMultiplier);
        
        if (isCritical)
        {
            finalDamage = Mathf.RoundToInt(finalDamage * critMultiplier);
        }
        
        return new DamageResult
        {
            damage = finalDamage,
            isCritical = isCritical,
            damageType = damageType,
            weaponType = weaponType
        };
    }
    
    /// <summary>
    /// Calculate critical hit chance based on runes and weapon passives
    /// </summary>
    private static float CalculateCriticalChance(WeaponType weaponType)
    {
        float critChance = BASE_CRIT_CHANCE;
        
        // Add rune bonuses
        if (GameManager.Instance != null)
        {
            critChance += GameManager.Instance.CriticalRuneCount * CRIT_RUNE_BONUS;
            
            // Add weapon-specific passive bonuses
            if (weaponType == WeaponType.Bow && GameManager.Instance.IsBowPassiveUnlocked())
            {
                critChance += BOW_PASSIVE_CRIT_BONUS;
            }
        }
        
        return Mathf.Clamp01(critChance); // Cap at 100%
    }
    
    /// <summary>
    /// Calculate critical damage multiplier based on runes
    /// </summary>
    private static float CalculateCriticalMultiplier()
    {
        float critMultiplier = BASE_CRIT_MULTIPLIER;
        
        if (GameManager.Instance != null)
        {
            critMultiplier += GameManager.Instance.CritDamageRuneCount * CRIT_DAMAGE_RUNE_BONUS;
        }
        
        return critMultiplier;
    }
    
    /// <summary>
    /// Get weapon-specific damage multiplier
    /// </summary>
    private static float GetWeaponDamageMultiplier(WeaponType weaponType)
    {
        if (GameManager.Instance == null) return 1f;
        
        switch (weaponType)
        {
            case WeaponType.Sword:
                return GameManager.Instance.IsSwordPassiveUnlocked() ? SWORD_PASSIVE_DAMAGE_BONUS : 1f;
            
            case WeaponType.Scythe:
                // Scythe passive affects healing, not damage
                return 1f;
            
            case WeaponType.Bow:
                // Bow passive affects crit chance, not base damage
                return 1f;
            
            case WeaponType.Runeblade:
                return GameManager.Instance.IsRunebladePassiveUnlocked() ? 1.2f : 1f;
            
            case WeaponType.Sabres:
                return GameManager.Instance.IsSabresPassiveUnlocked() ? 1.1f : 1f;
            
            default:
                return 1f;
        }
    }
    
    /// <summary>
    /// Calculate healing amount (used by Scythe)
    /// </summary>
    public static int CalculateHealing(int baseHealing, WeaponType weaponType)
    {
        float multiplier = 1f;
        
        if (weaponType == WeaponType.Scythe && GameManager.Instance != null)
        {
            if (GameManager.Instance.IsScythePassiveUnlocked())
            {
                multiplier = SCYTHE_PASSIVE_HEALING_BONUS;
            }
        }
        
        return Mathf.RoundToInt(baseHealing * multiplier);
    }
    
    /// <summary>
    /// Calculate damage with level scaling (for enemies)
    /// </summary>
    public static int CalculateScaledDamage(int baseDamage, int level)
    {
        // Scale damage by level
        float levelMultiplier = 1f + ((level - 1) * 0.1f);
        return Mathf.RoundToInt(baseDamage * levelMultiplier);
    }
    
    /// <summary>
    /// Calculate damage reduction from armor (future feature)
    /// </summary>
    public static int ApplyArmorReduction(int damage, int armorValue)
    {
        // Simple armor formula: reduction = armor / (armor + 100)
        float reduction = armorValue / (float)(armorValue + 100);
        return Mathf.RoundToInt(damage * (1f - reduction));
    }
}

/// <summary>
/// Result of a damage calculation
/// </summary>
public struct DamageResult
{
    public int damage;
    public bool isCritical;
    public DamageType damageType;
    public WeaponType weaponType;
}

