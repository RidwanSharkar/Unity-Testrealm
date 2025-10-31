using UnityEngine;

/// <summary>
/// Player controller for character movement, combat, and interactions.
/// Handles input from new Input System and integrates all player components.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : Entity
{
    [Header("Player Identity")]
    [SerializeField] private int playerId = 1;
    [SerializeField] private string playerName = "Player";
    
    [Header("Components")]
    [SerializeField] private MovementComponent movementComponent;
    [SerializeField] private HealthComponent healthComponent;
    [SerializeField] private Camera playerCamera;
    
    [Header("Weapons")]
    [SerializeField] private BaseWeapon[] availableWeapons;
    [SerializeField] private int currentWeaponIndex = 0;
    private BaseWeapon currentWeapon;
    
    [Header("Direct Spell Casting (Weapon-Free)")]
    [SerializeField] private MageSpellCaster spellCaster; // For mage characters that cast without weapons
    
    [Header("Camera Settings")]
    [SerializeField] private float cameraSensitivity = 2f;
    [SerializeField] private float cameraMinY = -60f;
    [SerializeField] private float cameraMaxY = 60f;
    private float cameraRotationX = 0f;
    private float cameraRotationY = 0f;
    
    [Header("Input Settings")]
    [SerializeField] private bool mouseLocked = true;
    
    // Input values
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool sprintInput;
    private bool jumpInput;
    private bool primaryAttackInput; // Left click - primary attack
    private bool secondaryAttackInput; // Right click - secondary attack
    private bool leftClickPressed; // For single click detection
    
    // State
    private bool isAlive = true;
    private bool canMove = true;
    
    // Properties
    public int PlayerId => playerId;
    public string PlayerName => playerName;
    public BaseWeapon CurrentWeapon => currentWeapon;
    public bool IsAlive => isAlive;
    
    protected override void Awake()
    {
        base.Awake();
        
        // Get components
        if (movementComponent == null)
            movementComponent = GetComponent<MovementComponent>();
        
        if (healthComponent == null)
            healthComponent = GetComponent<HealthComponent>();
        
        if (playerCamera == null)
            playerCamera = Camera.main;
        
        // Check for spell caster (weapon-free casting)
        if (spellCaster == null)
            spellCaster = GetComponent<MageSpellCaster>();
        
        // Setup cursor
        if (mouseLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        entityName = playerName;
    }
    
    protected virtual void Start()
    {
        // Subscribe to health events
        if (healthComponent != null)
        {
            healthComponent.OnDeath.AddListener(OnPlayerDeath);
            healthComponent.OnDamageTaken.AddListener(OnPlayerDamaged);
        }
        
        // Setup starting weapon
        if (availableWeapons != null && availableWeapons.Length > 0)
        {
            EquipWeapon(0);
        }
        
        // Register with combat system
        if (CombatSystem.Instance != null)
        {
            CombatSystem.Instance.RegisterEntity(this);
        }
        
        // Initialize camera rotation
        cameraRotationY = transform.eulerAngles.y;
    }
    
    void Update()
    {
        if (!isAlive) return;
        
        HandleInput();
        HandleCameraRotation();
        HandleMovement();
        HandleCombat();
        HandleAbilities();
        HandleWeaponSwitching();
    }
    
    /// <summary>
    /// Handle input using Unity's legacy Input system
    /// </summary>
    private void HandleInput()
    {
        // Movement input (WASD)
        moveInput.x = Input.GetAxisRaw("Horizontal"); // A/D - using GetAxisRaw for immediate response
        moveInput.y = Input.GetAxisRaw("Vertical");   // W/S - using GetAxisRaw for immediate response
        
        // Mouse look input
        lookInput.x = Input.GetAxis("Mouse X");
        lookInput.y = Input.GetAxis("Mouse Y");
        
        // Sprint (Left Shift)
        sprintInput = Input.GetKey(KeyCode.LeftShift);
        
        // Jump (Spacebar)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInput = true;
        }
        
        // Left click - Primary Attack (single click for casting)
        leftClickPressed = Input.GetMouseButtonDown(0);
        primaryAttackInput = Input.GetMouseButton(0); // Held for continuous casting
        
        // Right click - Secondary Attack (held)
        secondaryAttackInput = Input.GetMouseButton(1); // Right mouse button
        
        // Abilities - check for spell caster first, then weapon
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (spellCaster != null)
            {
                spellCaster.CastFireballAbility();
            }
            else if (currentWeapon != null)
            {
                currentWeapon.PerformAbility("Q");
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (spellCaster != null)
            {
                spellCaster.CastIceNova();
            }
            else if (currentWeapon != null)
            {
                currentWeapon.PerformAbility("E");
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (spellCaster != null)
            {
                spellCaster.CastLightningStrike();
            }
            else if (currentWeapon != null)
            {
                currentWeapon.PerformAbility("R");
            }
        }
        if (Input.GetKeyDown(KeyCode.F) && currentWeapon != null)
        {
            currentWeapon.PerformAbility("F");
        }
        
        // Weapon switching (mouse wheel)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            SwitchWeapon(1);
        }
        else if (scroll < 0f)
        {
            SwitchWeapon(-1);
        }
    }
    
    /// <summary>
    /// Handle camera rotation based on mouse input
    /// Camera is now controlled by ThirdPersonCamera script with RIGHT CLICK
    /// This method is kept for compatibility but camera control moved to ThirdPersonCamera
    /// </summary>
    private void HandleCameraRotation()
    {
        // Camera rotation is now handled by ThirdPersonCamera component
        // Right-click to rotate camera (left-click reserved for attacks)
        // This method kept for future camera-related logic if needed
    }
    
    /// <summary>
    /// Handle player movement
    /// </summary>
    private void HandleMovement()
    {
        if (!canMove || movementComponent == null) return;
        
        // Calculate movement direction relative to camera
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        
        // Flatten directions (ignore Y axis)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        
        // Calculate move direction
        Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x);
        
        // Only normalize if there's actual input (prevents drift to idle animation)
        if (moveDirection.magnitude > 0.1f)
        {
            moveDirection.Normalize();
        }
        else
        {
            moveDirection = Vector3.zero; // Ensure clean zero when not moving
        }
        
        // Move character (pass raw input for animations)
        movementComponent.Move(moveDirection, sprintInput, moveInput);
        
        // Handle jump
        if (jumpInput)
        {
            movementComponent.Jump();
            jumpInput = false;
        }
    }
    
    /// <summary>
    /// Handle combat actions
    /// </summary>
    private void HandleCombat()
    {
        // Check for weapon-free spell caster first (for mage)
        if (spellCaster != null)
        {
            // Primary Attack (Left Click) - casting attacks like fireball
            if (leftClickPressed)
            {
                Debug.Log($"[PlayerController] Left-click detected! Calling spell caster PerformPrimaryAttack()");
                spellCaster.PerformPrimaryAttack();
            }
            
            // Note: Mage doesn't use secondary attack (right-click)
            // Right-click is used for camera rotation by ThirdPersonCamera
            return;
        }
        
        // Fall back to weapon-based combat
        if (currentWeapon == null)
        {
            if (leftClickPressed || secondaryAttackInput)
            {
                Debug.LogWarning("No weapon or spell caster! Cannot attack.");
            }
            return;
        }
        
        // Primary Attack (Left Click) - casting attacks like fireball
        if (leftClickPressed)
        {
            Debug.Log($"[PlayerController] Left-click detected! Calling PerformPrimaryAttack() on {currentWeapon.WeaponName}");
            currentWeapon.PerformPrimaryAttack();
        }
        
        // Secondary Attack (Right Click held) - continuous attacks like beam
        if (secondaryAttackInput)
        {
            currentWeapon.PerformAttack();
        }
    }
    
    /// <summary>
    /// Handle ability inputs (Q, E, R, F)
    /// </summary>
    private void HandleAbilities()
    {
        // Abilities are handled by Input System callbacks
        // This method can be used for additional ability logic
    }
    
    /// <summary>
    /// Handle weapon switching
    /// </summary>
    private void HandleWeaponSwitching()
    {
        // Handle number keys for direct weapon switching
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EquipWeapon(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            EquipWeapon(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            EquipWeapon(4);
        }
    }
    
    /// <summary>
    /// Equip a weapon by index
    /// </summary>
    public void EquipWeapon(int weaponIndex)
    {
        if (availableWeapons == null || weaponIndex < 0 || weaponIndex >= availableWeapons.Length)
            return;
        
        if (availableWeapons[weaponIndex] == null)
            return;
        
        // Unequip current weapon
        if (currentWeapon != null)
        {
            currentWeapon.Unequip();
        }
        
        // Equip new weapon
        currentWeaponIndex = weaponIndex;
        currentWeapon = availableWeapons[weaponIndex];
        currentWeapon.SetOwner(this);
        currentWeapon.Equip();
        
        Debug.Log($"Equipped {currentWeapon.WeaponName}");
    }
    
    /// <summary>
    /// Switch to next/previous weapon
    /// </summary>
    public void SwitchWeapon(int direction)
    {
        if (availableWeapons == null || availableWeapons.Length == 0)
            return;
        
        int newIndex = currentWeaponIndex + direction;
        
        // Wrap around
        if (newIndex >= availableWeapons.Length)
            newIndex = 0;
        else if (newIndex < 0)
            newIndex = availableWeapons.Length - 1;
        
        EquipWeapon(newIndex);
    }
    
    /// <summary>
    /// Called when player takes damage
    /// </summary>
    private void OnPlayerDamaged(int damage)
    {
        // Add screen shake, damage indicator, etc.
        Debug.Log($"{playerName} took {damage} damage! HP: {healthComponent.CurrentHealth}/{healthComponent.MaxHealth}");
    }
    
    /// <summary>
    /// Called when player dies
    /// </summary>
    private void OnPlayerDeath()
    {
        isAlive = false;
        canMove = false;
        
        // Disable movement
        if (movementComponent != null)
        {
            movementComponent.SetMovementEnabled(false);
        }
        
        // Unequip weapon
        if (currentWeapon != null)
        {
            currentWeapon.Unequip();
        }
        
        Debug.Log($"{playerName} has died!");
        
        // Trigger death screen, respawn logic, etc.
    }
    
    /// <summary>
    /// Respawn player
    /// </summary>
    public void Respawn(Vector3 respawnPosition)
    {
        // Restore health
        if (healthComponent != null)
        {
            healthComponent.Revive();
        }
        
        // Teleport to respawn position
        if (movementComponent != null)
        {
            movementComponent.TeleportTo(respawnPosition);
            movementComponent.SetMovementEnabled(true);
        }
        
        // Re-equip weapon
        if (currentWeapon != null)
        {
            currentWeapon.Equip();
        }
        
        isAlive = true;
        canMove = true;
        
        Debug.Log($"{playerName} respawned!");
    }
    
    /// <summary>
    /// Toggle cursor lock
    /// </summary>
    public void ToggleCursorLock()
    {
        mouseLocked = !mouseLocked;
        
        if (mouseLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    /// <summary>
    /// Set player movement enabled/disabled
    /// </summary>
    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
    }
}

