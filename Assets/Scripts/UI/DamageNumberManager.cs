using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages floating damage numbers and healing numbers.
/// Uses object pooling for performance.
/// </summary>
public class DamageNumberManager : MonoBehaviour
{
    public static DamageNumberManager Instance { get; private set; }
    
    [Header("Damage Number Settings")]
    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Canvas worldCanvas;
    
    [Header("Appearance Settings")]
    [SerializeField] private Color normalDamageColor = Color.white;
    [SerializeField] private Color criticalDamageColor = Color.yellow;
    [SerializeField] private Color healingColor = Color.green;
    [SerializeField] private float normalFontSize = 36f;
    [SerializeField] private float criticalFontSize = 48f;
    [SerializeField] private float healingFontSize = 40f;
    
    [Header("Animation Settings")]
    [SerializeField] private float duration = 2f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private Vector3 randomOffset = new Vector3(0.5f, 0.5f, 0);
    
    [Header("Pooling")]
    [SerializeField] private int poolSize = 20;
    
    // Object pool
    private Queue<DamageNumber> damageNumberPool = new Queue<DamageNumber>();
    private List<DamageNumber> activeDamageNumbers = new List<DamageNumber>();
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        if (mainCamera == null)
            mainCamera = Camera.main;
        
        InitializePool();
    }
    
    /// <summary>
    /// Initialize the object pool
    /// </summary>
    private void InitializePool()
    {
        if (damageNumberPrefab == null)
        {
            Debug.LogWarning("DamageNumberManager: No damage number prefab assigned!");
            return;
        }
        
        // Create world canvas if not assigned
        if (worldCanvas == null)
        {
            GameObject canvasObj = new GameObject("DamageNumberCanvas");
            canvasObj.transform.SetParent(transform);
            worldCanvas = canvasObj.AddComponent<Canvas>();
            worldCanvas.renderMode = RenderMode.WorldSpace;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }
        
        // Pre-instantiate damage numbers
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewDamageNumber();
        }
    }
    
    /// <summary>
    /// Create a new damage number and add it to the pool
    /// </summary>
    private DamageNumber CreateNewDamageNumber()
    {
        GameObject obj = Instantiate(damageNumberPrefab, worldCanvas.transform);
        DamageNumber damageNumber = obj.GetComponent<DamageNumber>();
        
        if (damageNumber == null)
        {
            damageNumber = obj.AddComponent<DamageNumber>();
        }
        
        obj.SetActive(false);
        damageNumberPool.Enqueue(damageNumber);
        
        return damageNumber;
    }
    
    /// <summary>
    /// Get a damage number from the pool
    /// </summary>
    private DamageNumber GetDamageNumber()
    {
        DamageNumber damageNumber;
        
        if (damageNumberPool.Count > 0)
        {
            damageNumber = damageNumberPool.Dequeue();
        }
        else
        {
            // Create new if pool is empty
            damageNumber = CreateNewDamageNumber();
            damageNumberPool.Dequeue();
        }
        
        damageNumber.gameObject.SetActive(true);
        activeDamageNumbers.Add(damageNumber);
        
        return damageNumber;
    }
    
    /// <summary>
    /// Return a damage number to the pool
    /// </summary>
    private void ReturnDamageNumber(DamageNumber damageNumber)
    {
        damageNumber.gameObject.SetActive(false);
        activeDamageNumbers.Remove(damageNumber);
        damageNumberPool.Enqueue(damageNumber);
    }
    
    /// <summary>
    /// Show damage number at world position
    /// </summary>
    public void ShowDamage(int damage, Vector3 worldPosition, bool isCritical = false)
    {
        DamageNumber damageNumber = GetDamageNumber();
        
        // Add random offset
        Vector3 randomPos = worldPosition + new Vector3(
            Random.Range(-randomOffset.x, randomOffset.x),
            Random.Range(0, randomOffset.y),
            Random.Range(-randomOffset.z, randomOffset.z)
        );
        
        // Setup damage number
        damageNumber.Setup(
            damage.ToString(),
            randomPos,
            isCritical ? criticalDamageColor : normalDamageColor,
            isCritical ? criticalFontSize : normalFontSize
        );
        
        // Animate
        StartCoroutine(AnimateDamageNumber(damageNumber, randomPos));
    }
    
    /// <summary>
    /// Show healing number at world position
    /// </summary>
    public void ShowHealing(int healAmount, Vector3 worldPosition)
    {
        DamageNumber damageNumber = GetDamageNumber();
        
        // Add random offset
        Vector3 randomPos = worldPosition + new Vector3(
            Random.Range(-randomOffset.x, randomOffset.x),
            Random.Range(0, randomOffset.y),
            Random.Range(-randomOffset.z, randomOffset.z)
        );
        
        // Setup healing number with + prefix
        damageNumber.Setup(
            $"+{healAmount}",
            randomPos,
            healingColor,
            healingFontSize
        );
        
        // Animate
        StartCoroutine(AnimateDamageNumber(damageNumber, randomPos));
    }
    
    /// <summary>
    /// Animate damage number
    /// </summary>
    private IEnumerator AnimateDamageNumber(DamageNumber damageNumber, Vector3 startPosition)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            
            // Move upward
            Vector3 newPosition = startPosition + Vector3.up * (moveSpeed * elapsed);
            damageNumber.transform.position = newPosition;
            
            // Always face camera
            if (mainCamera != null)
            {
                damageNumber.transform.rotation = Quaternion.LookRotation(
                    damageNumber.transform.position - mainCamera.transform.position
                );
            }
            
            // Fade out
            float alpha = 1f - (progress * fadeSpeed);
            damageNumber.SetAlpha(alpha);
            
            // Scale animation for critical hits
            if (elapsed < 0.2f)
            {
                float scale = Mathf.Lerp(0.5f, 1.2f, elapsed / 0.2f);
                damageNumber.transform.localScale = Vector3.one * scale;
            }
            else if (elapsed < 0.4f)
            {
                float scale = Mathf.Lerp(1.2f, 1f, (elapsed - 0.2f) / 0.2f);
                damageNumber.transform.localScale = Vector3.one * scale;
            }
            
            yield return null;
        }
        
        // Return to pool
        ReturnDamageNumber(damageNumber);
    }
    
    /// <summary>
    /// Clear all active damage numbers
    /// </summary>
    public void ClearAll()
    {
        foreach (DamageNumber damageNumber in activeDamageNumbers.ToArray())
        {
            ReturnDamageNumber(damageNumber);
        }
        activeDamageNumbers.Clear();
    }
}

/// <summary>
/// Individual damage number component
/// </summary>
public class DamageNumber : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    
    void Awake()
    {
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
        
        if (textComponent == null)
        {
            // Create text component if it doesn't exist
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(transform);
            textObj.transform.localPosition = Vector3.zero;
            textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.fontSize = 36;
        }
    }
    
    /// <summary>
    /// Setup damage number
    /// </summary>
    public void Setup(string text, Vector3 position, Color color, float fontSize)
    {
        textComponent.text = text;
        textComponent.color = color;
        textComponent.fontSize = fontSize;
        transform.position = position;
        transform.localScale = Vector3.one;
    }
    
    /// <summary>
    /// Set alpha for fade effect
    /// </summary>
    public void SetAlpha(float alpha)
    {
        Color color = textComponent.color;
        color.a = alpha;
        textComponent.color = color;
    }
}

