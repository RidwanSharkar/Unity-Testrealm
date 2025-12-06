using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Mana component for entities.
/// Manages mana, consumption, regeneration, and mana events.
/// </summary>
public class ManaComponent : MonoBehaviour
{
    [Header("Mana Settings")]
    [SerializeField] private int maxMana = 100;
    [SerializeField] private int currentMana;
    [SerializeField] private bool canRegenerate = true;
    [SerializeField] private float regenerationRate = 5f; // Mana per second
    [SerializeField] private float regenerationDelay = 2f; // Delay after spending mana
    private float timeSinceLastSpend = 0f;

    // Events
    public UnityEvent<int, int> OnManaChanged; // current, max
    public UnityEvent<int> OnManaSpent; // amount spent
    public UnityEvent<int> OnManaGained; // amount gained

    // Properties
    public int MaxMana => maxMana;
    public int CurrentMana => currentMana;
    public bool IsFull => currentMana >= maxMana;
    public float ManaPercentage => maxMana > 0 ? (float)currentMana / maxMana : 0f;

    void Awake()
    {
        currentMana = maxMana;
    }

    void Update()
    {
        if (canRegenerate && currentMana < maxMana)
        {
            timeSinceLastSpend += Time.deltaTime;

            if (timeSinceLastSpend >= regenerationDelay)
            {
                GainMana(Mathf.CeilToInt(regenerationRate * Time.deltaTime));
            }
        }
    }

    /// <summary>
    /// Spend mana for spells/abilities
    /// </summary>
    public bool SpendMana(int amount)
    {
        if (amount <= 0)
            return true;

        if (currentMana < amount)
        {
            Debug.LogWarning($"Not enough mana! Need {amount}, have {currentMana}");
            return false;
        }

        timeSinceLastSpend = 0f; // Reset regeneration timer

        int oldMana = currentMana;
        currentMana -= amount;
        currentMana = Mathf.Max(currentMana, 0);

        OnManaChanged?.Invoke(currentMana, maxMana);
        OnManaSpent?.Invoke(amount);

        return true;
    }

    /// <summary>
    /// Gain mana (regeneration or items)
    /// </summary>
    public void GainMana(int amount)
    {
        if (amount <= 0)
            return;

        int oldMana = currentMana;
        currentMana += amount;
        currentMana = Mathf.Min(currentMana, maxMana);

        int actualGained = currentMana - oldMana;

        if (actualGained > 0)
        {
            OnManaChanged?.Invoke(currentMana, maxMana);
            OnManaGained?.Invoke(actualGained);
        }
    }

    /// <summary>
    /// Set maximum mana (scales current mana proportionally)
    /// </summary>
    public void SetMaxMana(int newMaxMana, bool scaleCurrentMana = true)
    {
        if (scaleCurrentMana)
        {
            float manaPercentage = ManaPercentage;
            maxMana = newMaxMana;
            currentMana = Mathf.RoundToInt(maxMana * manaPercentage);
        }
        else
        {
            maxMana = newMaxMana;
            currentMana = Mathf.Min(currentMana, maxMana);
        }

        OnManaChanged?.Invoke(currentMana, maxMana);
    }

    /// <summary>
    /// Increase max mana permanently (e.g., from upgrades)
    /// </summary>
    public void IncreaseMaxMana(int amount)
    {
        maxMana += amount;
        currentMana += amount; // Also increase current mana
        OnManaChanged?.Invoke(currentMana, maxMana);
    }

    /// <summary>
    /// Restore to full mana
    /// </summary>
    public void RestoreToFull()
    {
        currentMana = maxMana;
        OnManaChanged?.Invoke(currentMana, maxMana);
    }

    /// <summary>
    /// Set regeneration enabled/disabled
    /// </summary>
    public void SetRegenerationEnabled(bool enabled)
    {
        canRegenerate = enabled;
    }

    /// <summary>
    /// Check if entity has enough mana for a cost
    /// </summary>
    public bool HasEnoughMana(int cost)
    {
        return currentMana >= cost;
    }
}
