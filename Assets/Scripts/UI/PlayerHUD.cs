using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Player HUD managing health bar, ability cooldowns, and status display.
/// </summary>
public class PlayerHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    
    [Header("Health Bar")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthFillImage;
    [SerializeField] private Color healthColorHigh = Color.green;
    [SerializeField] private Color healthColorMid = Color.yellow;
    [SerializeField] private Color healthColorLow = Color.red;
    
    [Header("Ability Cooldowns")]
    [SerializeField] private Image ability1Image;
    [SerializeField] private Image ability2Image;
    [SerializeField] private Image ability3Image;
    [SerializeField] private Image ability4Image;
    [SerializeField] private TextMeshProUGUI ability1Text;
    [SerializeField] private TextMeshProUGUI ability2Text;
    [SerializeField] private TextMeshProUGUI ability3Text;
    [SerializeField] private TextMeshProUGUI ability4Text;
    
    [Header("Weapon Display")]
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private Image weaponIcon;
    
    [Header("Player Info")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI levelText;
    
    private HealthComponent playerHealth;
    private BaseWeapon currentWeapon;
    
    void Start()
    {
        if (playerController != null)
        {
            playerHealth = playerController.GetEntityComponent<HealthComponent>();
            
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged.AddListener(UpdateHealthBar);
            }
            
            // Set player name
            if (playerNameText != null)
            {
                playerNameText.text = playerController.PlayerName;
            }
        }
        
        // Initialize HUD
        UpdateHealthBar(0, 0);
    }
    
    void Update()
    {
        UpdateAbilityCooldowns();
        UpdateWeaponDisplay();
    }
    
    /// <summary>
    /// Update health bar display
    /// </summary>
    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (playerHealth == null) return;
        
        currentHealth = playerHealth.CurrentHealth;
        maxHealth = playerHealth.MaxHealth;
        
        // Update slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        
        // Update text
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
        
        // Update color based on health percentage
        if (healthFillImage != null)
        {
            float healthPercent = playerHealth.HealthPercentage;
            
            if (healthPercent > 0.6f)
                healthFillImage.color = healthColorHigh;
            else if (healthPercent > 0.3f)
                healthFillImage.color = healthColorMid;
            else
                healthFillImage.color = healthColorLow;
        }
    }
    
    /// <summary>
    /// Update ability cooldown displays
    /// </summary>
    private void UpdateAbilityCooldowns()
    {
        if (playerController == null) return;
        
        currentWeapon = playerController.CurrentWeapon;
        if (currentWeapon == null) return;
        
        // Update Q ability
        UpdateAbilityCooldownUI(0, ability1Image, ability1Text);
        
        // Update E ability
        UpdateAbilityCooldownUI(1, ability2Image, ability2Text);
        
        // Update R ability
        UpdateAbilityCooldownUI(2, ability3Image, ability3Text);
        
        // Update F ability
        UpdateAbilityCooldownUI(3, ability4Image, ability4Text);
    }
    
    /// <summary>
    /// Update individual ability cooldown UI
    /// </summary>
    private void UpdateAbilityCooldownUI(int abilityIndex, Image cooldownImage, TextMeshProUGUI cooldownText)
    {
        if (currentWeapon == null) return;
        
        AbilityCooldown cooldown = currentWeapon.GetAbilityCooldown(abilityIndex);
        
        if (cooldown == null) return;
        
        // Update fill amount
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 1f - cooldown.Progress;
            
            // Darken if on cooldown
            cooldownImage.color = cooldown.IsReady ? Color.white : new Color(0.3f, 0.3f, 0.3f, 1f);
        }
        
        // Update text
        if (cooldownText != null)
        {
            if (cooldown.IsReady)
            {
                cooldownText.text = "";
            }
            else
            {
                cooldownText.text = Mathf.Ceil(cooldown.remainingTime).ToString();
            }
        }
    }
    
    /// <summary>
    /// Update weapon display
    /// </summary>
    private void UpdateWeaponDisplay()
    {
        if (playerController == null) return;
        
        currentWeapon = playerController.CurrentWeapon;
        
        if (weaponNameText != null && currentWeapon != null)
        {
            weaponNameText.text = currentWeapon.WeaponName;
        }
    }
    
    /// <summary>
    /// Set player controller reference
    /// </summary>
    public void SetPlayerController(PlayerController controller)
    {
        playerController = controller;
        
        if (playerController != null)
        {
            playerHealth = playerController.GetEntityComponent<HealthComponent>();
            
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged.AddListener(UpdateHealthBar);
                UpdateHealthBar(0, 0);
            }
        }
    }
    
    /// <summary>
    /// Show message on HUD
    /// </summary>
    public void ShowMessage(string message, float duration = 3f)
    {
        // You could create a message panel for this
        Debug.Log($"HUD Message: {message}");
    }
}

