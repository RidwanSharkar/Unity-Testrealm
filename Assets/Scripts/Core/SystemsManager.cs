using UnityEngine;

/// <summary>
/// Manages core game systems and ensures they exist in the scene.
/// Add this to a GameObject in your scene to automatically initialize all systems.
/// </summary>
public class SystemsManager : MonoBehaviour
{
    [Header("Auto-Initialize Systems")]
    [SerializeField] private bool autoCreateSystems = true;
    
    [Header("System References (Auto-Created if null)")]
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private ProjectileManager projectileManager;
    [SerializeField] private DamageNumberManager damageNumberManager;
    
    private void Awake()
    {
        if (autoCreateSystems)
        {
            InitializeAllSystems();
        }
    }
    
    /// <summary>
    /// Initialize all core game systems
    /// </summary>
    private void InitializeAllSystems()
    {
        // Create CombatSystem if missing
        if (combatSystem == null)
        {
            combatSystem = FindObjectOfType<CombatSystem>();
            
            if (combatSystem == null)
            {
                GameObject combatSystemObj = new GameObject("CombatSystem");
                combatSystemObj.transform.SetParent(transform);
                combatSystem = combatSystemObj.AddComponent<CombatSystem>();
                Debug.Log("[SystemsManager] Created CombatSystem");
            }
        }
        
        // Create ProjectileManager if missing
        if (projectileManager == null)
        {
            projectileManager = FindObjectOfType<ProjectileManager>();
            
            if (projectileManager == null)
            {
                GameObject projectileManagerObj = new GameObject("ProjectileManager");
                projectileManagerObj.transform.SetParent(transform);
                projectileManager = projectileManagerObj.AddComponent<ProjectileManager>();
                Debug.Log("[SystemsManager] Created ProjectileManager");
            }
        }
        
        // Create DamageNumberManager if missing (optional)
        if (damageNumberManager == null)
        {
            damageNumberManager = FindObjectOfType<DamageNumberManager>();
            
            if (damageNumberManager == null)
            {
                Debug.LogWarning("[SystemsManager] DamageNumberManager not found. Damage numbers will not be displayed.");
            }
        }
        
        Debug.Log("[SystemsManager] All systems initialized successfully!");
    }
    
    /// <summary>
    /// Check if all systems are present
    /// </summary>
    public bool AreAllSystemsReady()
    {
        return combatSystem != null && projectileManager != null;
    }
    
    /// <summary>
    /// Get CombatSystem instance
    /// </summary>
    public CombatSystem GetCombatSystem()
    {
        return combatSystem;
    }
    
    /// <summary>
    /// Get ProjectileManager instance
    /// </summary>
    public ProjectileManager GetProjectileManager()
    {
        return projectileManager;
    }
}

