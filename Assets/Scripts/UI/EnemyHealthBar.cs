using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// World-space health bar that appears above enemies
/// Automatically follows the enemy and updates based on health
/// </summary>
public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BaseEnemy enemy;
    [SerializeField] private HealthComponent healthComponent;
    
    [Header("UI Elements")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;
    
    [Header("Colors")]
    [SerializeField] private Color healthColorFull = Color.green;
    [SerializeField] private Color healthColorMid = Color.yellow;
    [SerializeField] private Color healthColorLow = Color.red;
    
    [Header("Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 2.5f, 0); // Height above enemy
    [SerializeField] private bool hideWhenFull = true;
    [SerializeField] private bool alwaysFaceCamera = true;
    [SerializeField] private float hideDelay = 3f; // Seconds before hiding when full
    
    private Camera mainCamera;
    private float lastDamageTime = 0f;
    private bool wasFullHealth = true;
    
    private void Awake()
    {
        // Auto-find components if not assigned
        if (enemy == null)
        {
            enemy = GetComponentInParent<BaseEnemy>();
        }
        
        if (healthComponent == null)
        {
            healthComponent = GetComponentInParent<HealthComponent>();
        }
        
        if (canvas == null)
        {
            canvas = GetComponentInChildren<Canvas>();
        }
        
        if (healthSlider == null)
        {
            healthSlider = GetComponentInChildren<Slider>();
        }
        
        if (fillImage == null && healthSlider != null)
        {
            fillImage = healthSlider.fillRect?.GetComponent<Image>();
        }
    }
    
    private void Start()
    {
        Debug.Log($"[EnemyHealthBar] Start called for {(enemy != null ? enemy.name : "Unknown")}");
        
        mainCamera = Camera.main;
        
        // Setup canvas
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = mainCamera;
            Debug.Log("[EnemyHealthBar] Canvas configured for World Space");
        }
        else
        {
            Debug.LogError("[EnemyHealthBar] Canvas is NULL!");
        }
        
        // Subscribe to health changes
        if (healthComponent != null)
        {
            healthComponent.OnHealthChanged.AddListener(OnHealthChanged);
            healthComponent.OnDamageTaken.AddListener(OnDamageTaken);
            Debug.Log($"[EnemyHealthBar] Subscribed to health events. Current health: {healthComponent.CurrentHealth}/{healthComponent.MaxHealth}");
        }
        else
        {
            Debug.LogError("[EnemyHealthBar] HealthComponent is NULL! Cannot subscribe to events.");
        }
        
        // Initialize health bar
        UpdateHealthBar();
        
        // Hide if at full health
        if (hideWhenFull && healthComponent != null && healthComponent.HealthPercentage >= 1f)
        {
            Debug.Log("[EnemyHealthBar] Hiding bar (full health)");
            Hide();
        }
        else
        {
            Debug.Log("[EnemyHealthBar] Showing bar");
            Show();
        }
    }
    
    private void LateUpdate()
    {
        // Update position to follow enemy
        if (enemy != null)
        {
            transform.position = enemy.transform.position + offset;
        }
        
        // Always face camera
        if (alwaysFaceCamera && mainCamera != null && canvas != null)
        {
            canvas.transform.LookAt(canvas.transform.position + mainCamera.transform.rotation * Vector3.forward,
                                   mainCamera.transform.rotation * Vector3.up);
        }
        
        // Auto-hide when at full health after delay
        if (hideWhenFull && healthComponent != null)
        {
            bool isFullHealth = healthComponent.HealthPercentage >= 1f;
            
            if (isFullHealth && !wasFullHealth)
            {
                lastDamageTime = Time.time;
            }
            
            if (isFullHealth && Time.time - lastDamageTime > hideDelay)
            {
                Hide();
            }
            
            wasFullHealth = isFullHealth;
        }
    }
    
    /// <summary>
    /// Called when health changes
    /// </summary>
    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        Debug.Log($"[EnemyHealthBar] OnHealthChanged: {currentHealth}/{maxHealth}");
        UpdateHealthBar();
    }
    
    /// <summary>
    /// Called when enemy takes damage
    /// </summary>
    private void OnDamageTaken(int damage)
    {
        Debug.Log($"[EnemyHealthBar] OnDamageTaken: {damage} damage");
        Show();
        lastDamageTime = Time.time;
    }
    
    /// <summary>
    /// Update health bar visuals
    /// </summary>
    private void UpdateHealthBar()
    {
        if (healthComponent == null)
        {
            Debug.LogWarning("[EnemyHealthBar] UpdateHealthBar: HealthComponent is NULL");
            return;
        }
        
        if (healthSlider == null)
        {
            Debug.LogWarning("[EnemyHealthBar] UpdateHealthBar: healthSlider is NULL");
            return;
        }
        
        // Update slider value
        healthSlider.maxValue = healthComponent.MaxHealth;
        healthSlider.value = healthComponent.CurrentHealth;
        
        Debug.Log($"[EnemyHealthBar] Updated slider: {healthSlider.value}/{healthSlider.maxValue}");
        
        // Update color based on health percentage
        if (fillImage != null)
        {
            float healthPercent = healthComponent.HealthPercentage;
            
            Color newColor;
            if (healthPercent > 0.6f)
            {
                newColor = healthColorFull;
            }
            else if (healthPercent > 0.3f)
            {
                newColor = healthColorMid;
            }
            else
            {
                newColor = healthColorLow;
            }
            
            fillImage.color = newColor;
            Debug.Log($"[EnemyHealthBar] Color updated to {newColor} ({healthPercent:P0})");
        }
        else
        {
            Debug.LogWarning("[EnemyHealthBar] fillImage is NULL");
        }
    }
    
    /// <summary>
    /// Show health bar
    /// </summary>
    public void Show()
    {
        if (canvas != null)
        {
            canvas.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// Hide health bar
    /// </summary>
    public void Hide()
    {
        if (canvas != null)
        {
            canvas.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Set offset above enemy
    /// </summary>
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (healthComponent != null)
        {
            if (healthComponent.OnHealthChanged != null)
            {
                healthComponent.OnHealthChanged.RemoveListener(OnHealthChanged);
            }
            
            if (healthComponent.OnDamageTaken != null)
            {
                healthComponent.OnDamageTaken.RemoveListener(OnDamageTaken);
            }
        }
    }
}

