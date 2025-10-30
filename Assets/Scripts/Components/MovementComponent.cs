using UnityEngine;

/// <summary>
/// Character movement component with physics support.
/// Handles ground movement, jumping, and gravity.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class MovementComponent : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float baseMovementSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float crouchMultiplier = 0.5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    
    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private int maxJumpCount = 1;
    private int currentJumpCount = 0;
    
    [Header("Ground Detection")]
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Speed Modifiers")]
    [SerializeField] private float speedModifier = 1f; // From runes/buffs
    
    [Header("Animation")]
    [SerializeField] private bool updateAnimator = true; // Automatically update animator parameters
    
    // Components
    private CharacterController characterController;
    private Animator animator;
    
    // Movement state
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private Vector3 inputDirection = Vector3.zero; // Store raw input for animations
    private Vector2 rawInput = Vector2.zero; // Store raw WASD input for animations
    private bool isGrounded = false;
    private bool isSprinting = false;
    private bool isCrouching = false;
    private bool isMovementDisabled = false;
    
    // Properties
    public bool IsGrounded => isGrounded;
    public bool IsSprinting => isSprinting;
    public bool IsCrouching => isCrouching;
    public Vector3 Velocity => velocity;
    public float CurrentSpeed => velocity.magnitude;
    public bool IsMoving => moveDirection.magnitude > 0.01f;
    
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        
        // Apply speed modifiers from runes
        if (GameManager.Instance != null)
        {
            speedModifier += GameManager.Instance.SpeedRuneCount * 0.05f;
        }
    }
    
    void Update()
    {
        CheckGroundStatus();
        ApplyGravity();
        UpdateAnimationParameters();
    }
    
    /// <summary>
    /// Move the character
    /// </summary>
    public void Move(Vector3 direction, bool sprint = false, Vector2 input = default)
    {
        if (isMovementDisabled) return;
        
        isSprinting = sprint && !isCrouching;
        moveDirection = direction.normalized;
        inputDirection = direction.normalized; // Store for animations
        rawInput = input; // Store raw WASD input for animations
        
        // Calculate target speed
        float targetSpeed = baseMovementSpeed * speedModifier;
        
        if (isSprinting)
            targetSpeed *= sprintMultiplier;
        else if (isCrouching)
            targetSpeed *= crouchMultiplier;
        
        // Smooth acceleration/deceleration
        Vector3 targetVelocity = moveDirection * targetSpeed;
        float accel = moveDirection.magnitude > 0 ? acceleration : deceleration;
        
        velocity.x = Mathf.Lerp(velocity.x, targetVelocity.x, accel * Time.deltaTime);
        velocity.z = Mathf.Lerp(velocity.z, targetVelocity.z, accel * Time.deltaTime);
        
        // Apply movement
        characterController.Move(velocity * Time.deltaTime);
    }
    
    /// <summary>
    /// Jump
    /// </summary>
    public void Jump()
    {
        if (isMovementDisabled) return;
        
        if (isGrounded)
        {
            currentJumpCount = 0;
        }
        
        if (currentJumpCount < maxJumpCount)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            currentJumpCount++;
            
            // Trigger jump animation
            TriggerJumpAnimation();
        }
    }
    
    /// <summary>
    /// Check if character is grounded
    /// </summary>
    private void CheckGroundStatus()
    {
        // Check slightly below the character controller
        Vector3 spherePosition = transform.position + Vector3.up * (characterController.radius * 0.5f);
        isGrounded = Physics.CheckSphere(spherePosition, characterController.radius + groundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore);
        
        // Also use CharacterController's built-in isGrounded for reliability
        if (characterController.isGrounded || isGrounded)
        {
            isGrounded = true;
            
            if (velocity.y < 0)
            {
                velocity.y = -2f; // Small downward force to keep grounded
            }
            
            currentJumpCount = 0;
        }
    }
    
    /// <summary>
    /// Apply gravity
    /// </summary>
    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }
    
    /// <summary>
    /// Set crouch state
    /// </summary>
    public void SetCrouching(bool crouching)
    {
        isCrouching = crouching;
    }
    
    /// <summary>
    /// Disable/enable movement
    /// </summary>
    public void SetMovementEnabled(bool enabled)
    {
        isMovementDisabled = !enabled;
        
        if (isMovementDisabled)
        {
            velocity = Vector3.zero;
            moveDirection = Vector3.zero;
        }
    }
    
    /// <summary>
    /// Apply knockback force
    /// </summary>
    public void ApplyKnockback(Vector3 force)
    {
        velocity += force;
    }
    
    /// <summary>
    /// Add movement speed modifier (from buffs/debuffs)
    /// </summary>
    public void AddSpeedModifier(float modifier)
    {
        speedModifier += modifier;
        speedModifier = Mathf.Max(0.1f, speedModifier); // Minimum 10% speed
    }
    
    /// <summary>
    /// Reset speed modifier to base (including runes)
    /// </summary>
    public void ResetSpeedModifier()
    {
        speedModifier = 1f;
        
        if (GameManager.Instance != null)
        {
            speedModifier += GameManager.Instance.SpeedRuneCount * 0.05f;
        }
    }
    
    /// <summary>
    /// Teleport to position
    /// </summary>
    public void TeleportTo(Vector3 position)
    {
        characterController.enabled = false;
        transform.position = position;
        characterController.enabled = true;
        velocity = Vector3.zero;
    }
    
    /// <summary>
    /// Get current movement speed including modifiers
    /// </summary>
    public float GetCurrentMovementSpeed()
    {
        float speed = baseMovementSpeed * speedModifier;
        
        if (isSprinting)
            speed *= sprintMultiplier;
        else if (isCrouching)
            speed *= crouchMultiplier;
        
        return speed;
    }
    
    /// <summary>
    /// Update animator parameters based on movement
    /// Uses raw input values for proper animation blending
    /// </summary>
    private void UpdateAnimationParameters()
    {
        if (!updateAnimator || animator == null) return;
        
        // Use raw input for animations (WASD input directly)
        // MoveX: -1 (A key/left) to +1 (D key/right)
        // MoveZ: -1 (S key/back) to +1 (W key/forward)
        animator.SetFloat("MoveX", rawInput.x);
        animator.SetFloat("MoveZ", rawInput.y);
        
        // Debug logging (TEMPORARY - remove after fixing)
        if (rawInput.magnitude > 0.01f)
        {
            Debug.Log($"Animation Params - MoveX: {rawInput.x:F2}, MoveZ: {rawInput.y:F2}, Speed: {rawInput.magnitude:F2}");
        }
        
        // Set grounded state
        animator.SetBool("IsGrounded", isGrounded);
        
        // Set movement speed magnitude
        float movementMagnitude = rawInput.magnitude;
        animator.SetFloat("Speed", movementMagnitude);
        
        // Clear input if not moving (for proper idle transition)
        if (rawInput.magnitude < 0.01f)
        {
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveZ", 0f);
            animator.SetFloat("Speed", 0f);
        }
    }
    
    /// <summary>
    /// Trigger jump animation
    /// </summary>
    public void TriggerJumpAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Jump");
        }
    }
}

