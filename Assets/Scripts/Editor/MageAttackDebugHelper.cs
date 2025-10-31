using UnityEngine;

/// <summary>
/// Debug helper script to test Mage attack system.
/// Attach this to your player character temporarily to debug left-click issues.
/// REMOVE THIS SCRIPT AFTER DEBUGGING!
/// </summary>
public class MageAttackDebugHelper : MonoBehaviour
{
    [Header("References (Auto-find if empty)")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private StaffWeapon staffWeapon;
    [SerializeField] private Animator animator;
    
    [Header("Debug Settings")]
    [SerializeField] private bool logEveryFrame = false;
    [SerializeField] private bool logInputOnly = true;
    
    private void Start()
    {
        // Auto-find components
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }
        
        if (staffWeapon == null)
        {
            staffWeapon = GetComponentInChildren<StaffWeapon>();
        }
        
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        
        // Log findings
        Debug.Log("=== MAGE ATTACK DEBUG HELPER ===");
        Debug.Log($"PlayerController: {(playerController != null ? "✓ FOUND" : "✗ NOT FOUND")}");
        Debug.Log($"StaffWeapon: {(staffWeapon != null ? "✓ FOUND" : "✗ NOT FOUND")}");
        Debug.Log($"Animator: {(animator != null ? "✓ FOUND" : "✗ NOT FOUND")}");
        
        if (animator != null)
        {
            Debug.Log($"Animator Controller: {(animator.runtimeAnimatorController != null ? animator.runtimeAnimatorController.name : "NONE")}");
            
            // Check for Cast parameter
            bool hasCastParameter = false;
            foreach (var param in animator.parameters)
            {
                if (param.name == "Cast" && param.type == AnimatorControllerParameterType.Trigger)
                {
                    hasCastParameter = true;
                    break;
                }
            }
            
            if (hasCastParameter)
            {
                Debug.Log("<color=green>✓ Animator has 'Cast' trigger parameter!</color>");
            }
            else
            {
                Debug.LogError("<color=red>✗ Animator is MISSING 'Cast' trigger parameter! Add it in the Animator Controller!</color>");
            }
        }
        
        if (staffWeapon != null)
        {
            // Use reflection to check private fields (for debugging only)
            var prefabField = staffWeapon.GetType().GetField("primaryFireballPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var castPointField = staffWeapon.GetType().GetField("spellCastPoint", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (prefabField != null)
            {
                var prefabValue = prefabField.GetValue(staffWeapon);
                Debug.Log($"Primary Fireball Prefab: {(prefabValue != null ? "✓ ASSIGNED" : "✗ NOT ASSIGNED - ASSIGN IN INSPECTOR!")}");
            }
            
            if (castPointField != null)
            {
                var castPointValue = castPointField.GetValue(staffWeapon);
                Debug.Log($"Spell Cast Point: {(castPointValue != null ? "✓ ASSIGNED" : "✗ NOT ASSIGNED - CREATE EMPTY GAMEOBJECT AT STAFF TIP!")}");
            }
        }
        
        Debug.Log("=== PRESS 'T' KEY TO MANUALLY TRIGGER CAST ANIMATION ===");
        Debug.Log("=== PRESS 'F' KEY TO MANUALLY SPAWN FIREBALL ===");
        Debug.Log("================================");
    }
    
    private void Update()
    {
        // Log every frame (optional, can be spammy)
        if (logEveryFrame && !logInputOnly)
        {
            Debug.Log($"Frame: {Time.frameCount}, Left-Click: {Input.GetMouseButton(0)}, Left-Click Down: {Input.GetMouseButtonDown(0)}");
        }
        
        // Log only when input detected
        if (logInputOnly)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log($"<color=yellow>>>> RAW LEFT-CLICK DETECTED (Frame {Time.frameCount}) <<<</color>");
            }
        }
        
        // Manual test: T key to trigger animation
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("<color=cyan>>>> MANUAL TEST: Triggering Cast animation with T key <<<</color>");
            if (animator != null)
            {
                animator.SetTrigger("Cast");
                Debug.Log("Cast trigger sent to animator");
            }
            else
            {
                Debug.LogError("No animator found!");
            }
        }
        
        // Manual test: F key to spawn fireball
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("<color=cyan>>>> MANUAL TEST: Calling PerformPrimaryAttack with F key <<<</color>");
            if (staffWeapon != null)
            {
                staffWeapon.PerformPrimaryAttack();
            }
            else
            {
                Debug.LogError("No staff weapon found!");
            }
        }
        
        // Check weapon status
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("=== CURRENT STATUS ===");
            if (playerController != null)
            {
                Debug.Log($"Current Weapon: {(playerController.CurrentWeapon != null ? playerController.CurrentWeapon.WeaponName : "NONE")}");
            }
            
            if (staffWeapon != null)
            {
                Debug.Log($"Staff Mana: {staffWeapon.CurrentMana}/{staffWeapon.MaxMana}");
            }
            Debug.Log("======================");
        }
    }
}

