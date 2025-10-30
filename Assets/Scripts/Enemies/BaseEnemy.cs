using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemy types
/// </summary>
public enum EnemyType
{
    Grunt,      // Basic enemy
    Elite,      // Stronger enemy
    Boss,       // Boss enemy
    Miniboss    // Mini-boss
}

/// <summary>
/// Enemy AI states
/// </summary>
public enum EnemyState
{
    Idle,
    Patrolling,
    Chasing,
    Attacking,
    Stunned,
    Frozen,
    Retreating,
    Dead
}

/// <summary>
/// Base enemy class with AI behavior.
/// Handles movement, aggro, attacking, and status effects.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class BaseEnemy : Entity
{
    [Header("Enemy Identity")]
    [SerializeField] protected EnemyType enemyType = EnemyType.Grunt;
    [SerializeField] protected int level = 1;
    [SerializeField] protected string enemyName = "Enemy";
    
    [Header("Stats")]
    [SerializeField] protected int baseHealth = 100;
    [SerializeField] protected int baseAttackDamage = 15;
    [SerializeField] protected float baseMovementSpeed = 3f;
    
    [Header("Combat Settings")]
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float attackRate = 1f; // Attacks per second
    [SerializeField] protected float aggroRange = 8f;
    [SerializeField] protected float loseAggroDistance = 15f;
    
    [Header("AI Settings")]
    [SerializeField] protected bool canPatrol = true;
    [SerializeField] protected float patrolRadius = 10f;
    [SerializeField] protected float idleTime = 2f;
    
    [Header("Loot")]
    [SerializeField] protected int experienceReward = 10;
    [SerializeField] protected int goldReward = 5;
    
    [Header("Visual/Audio")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected AudioClip attackSound;
    [SerializeField] protected AudioClip hurtSound;
    [SerializeField] protected AudioClip deathSound;
    
    // Components
    protected NavMeshAgent navAgent;
    protected HealthComponent healthComponent;
    protected AudioSource audioSource;
    
    // AI state
    protected EnemyState currentState = EnemyState.Idle;
    protected Transform targetPlayer = null;
    protected Vector3 spawnPosition;
    protected Vector3 patrolTarget;
    protected float lastAttackTime = 0f;
    protected float stateChangeTime = 0f;
    
    // Status effects
    protected bool isFrozen = false;
    protected bool isStunned = false;
    protected float frozenUntil = 0f;
    protected float stunnedUntil = 0f;
    
    // Properties
    public EnemyType Type => enemyType;
    public int Level => level;
    public EnemyState CurrentState => currentState;
    public bool IsDead => healthComponent != null && healthComponent.IsDead;
    
    protected override void Awake()
    {
        base.Awake();
        
        // Get components
        navAgent = GetComponent<NavMeshAgent>();
        healthComponent = GetEntityComponent<HealthComponent>();
        audioSource = GetComponent<AudioSource>();
        
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        
        // Setup health component if not already configured
        if (healthComponent == null)
        {
            healthComponent = gameObject.AddComponent<HealthComponent>();
        }
        
        // Set entity name
        entityName = $"{enemyName} (Lv.{level})";
    }
    
    protected virtual void Start()
    {
        spawnPosition = transform.position;
        
        // Initialize stats based on level and type
        InitializeStats();
        
        // Subscribe to health events
        if (healthComponent != null)
        {
            healthComponent.OnDeath.AddListener(OnDeath);
            healthComponent.OnDamageTaken.AddListener(OnDamageTaken);
        }
        
        // Set initial state
        ChangeState(EnemyState.Idle);
        
        // Register with combat system
        if (CombatSystem.Instance != null)
        {
            CombatSystem.Instance.RegisterEntity(this);
        }
    }
    
    protected virtual void Update()
    {
        if (IsDead) return;
        
        UpdateStatusEffects();
        
        if (isFrozen || isStunned)
        {
            navAgent.isStopped = true;
            return;
        }
        
        // Update AI based on current state
        switch (currentState)
        {
            case EnemyState.Idle:
                UpdateIdleState();
                break;
            case EnemyState.Patrolling:
                UpdatePatrolState();
                break;
            case EnemyState.Chasing:
                UpdateChaseState();
                break;
            case EnemyState.Attacking:
                UpdateAttackState();
                break;
        }
        
        // Update animations
        UpdateAnimations();
    }
    
    /// <summary>
    /// Initialize enemy stats based on level and type
    /// </summary>
    protected virtual void InitializeStats()
    {
        // Calculate scaled stats
        int scaledHealth = CalculateScaledHealth();
        int scaledDamage = CalculateScaledDamage();
        float scaledSpeed = baseMovementSpeed;
        
        // Apply type multipliers
        switch (enemyType)
        {
            case EnemyType.Grunt:
                // Base stats
                break;
            case EnemyType.Elite:
                scaledHealth = Mathf.RoundToInt(scaledHealth * 2f);
                scaledDamage = Mathf.RoundToInt(scaledDamage * 1.5f);
                scaledSpeed *= 1.2f;
                experienceReward *= 3;
                goldReward *= 2;
                break;
            case EnemyType.Miniboss:
                scaledHealth = Mathf.RoundToInt(scaledHealth * 5f);
                scaledDamage = Mathf.RoundToInt(scaledDamage * 2f);
                scaledSpeed *= 1.1f;
                experienceReward *= 10;
                goldReward *= 5;
                break;
            case EnemyType.Boss:
                scaledHealth = Mathf.RoundToInt(scaledHealth * 10f);
                scaledDamage = Mathf.RoundToInt(scaledDamage * 3f);
                scaledSpeed *= 1.0f;
                experienceReward *= 50;
                goldReward *= 20;
                break;
        }
        
        // Set health
        if (healthComponent != null)
        {
            healthComponent.SetMaxHealth(scaledHealth, false);
            healthComponent.RestoreToFull();
        }
        
        // Set attack damage
        baseAttackDamage = scaledDamage;
        
        // Set movement speed
        if (navAgent != null)
        {
            navAgent.speed = scaledSpeed;
        }
    }
    
    protected virtual int CalculateScaledHealth()
    {
        return Mathf.RoundToInt(baseHealth * (1f + (level - 1) * 0.15f));
    }
    
    protected virtual int CalculateScaledDamage()
    {
        return DamageCalculator.CalculateScaledDamage(baseAttackDamage, level);
    }
    
    /// <summary>
    /// Change AI state
    /// </summary>
    protected virtual void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;
        
        // Exit previous state
        OnStateExit(currentState);
        
        // Change state
        currentState = newState;
        stateChangeTime = Time.time;
        
        // Enter new state
        OnStateEnter(newState);
    }
    
    protected virtual void OnStateEnter(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Patrolling:
                SetNewPatrolTarget();
                break;
        }
    }
    
    protected virtual void OnStateExit(EnemyState state)
    {
        // Override in derived classes
    }
    
    #region AI State Updates
    
    protected virtual void UpdateIdleState()
    {
        // Check for nearby players
        if (CheckForPlayers())
        {
            return; // State changed to chasing
        }
        
        // Start patrolling after idle time
        if (canPatrol && Time.time - stateChangeTime >= idleTime)
        {
            ChangeState(EnemyState.Patrolling);
        }
    }
    
    protected virtual void UpdatePatrolState()
    {
        // Check for nearby players
        if (CheckForPlayers())
        {
            return; // State changed to chasing
        }
        
        // Move to patrol target
        if (navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            ChangeState(EnemyState.Idle);
        }
    }
    
    protected virtual void UpdateChaseState()
    {
        if (targetPlayer == null)
        {
            ChangeState(EnemyState.Idle);
            return;
        }
        
        float distanceToTarget = Vector3.Distance(transform.position, targetPlayer.position);
        
        // Check if lost aggro
        if (distanceToTarget > loseAggroDistance)
        {
            targetPlayer = null;
            ChangeState(EnemyState.Retreating);
            return;
        }
        
        // Check if in attack range
        if (distanceToTarget <= attackRange)
        {
            ChangeState(EnemyState.Attacking);
            return;
        }
        
        // Chase target
        navAgent.isStopped = false;
        navAgent.SetDestination(targetPlayer.position);
    }
    
    protected virtual void UpdateAttackState()
    {
        if (targetPlayer == null)
        {
            ChangeState(EnemyState.Idle);
            return;
        }
        
        float distanceToTarget = Vector3.Distance(transform.position, targetPlayer.position);
        
        // Check if target moved out of attack range
        if (distanceToTarget > attackRange * 1.2f)
        {
            ChangeState(EnemyState.Chasing);
            return;
        }
        
        // Stop moving
        navAgent.isStopped = true;
        
        // Face target
        Vector3 directionToTarget = (targetPlayer.position - transform.position).normalized;
        directionToTarget.y = 0;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(directionToTarget),
            Time.deltaTime * 5f
        );
        
        // Attack if cooldown ready
        if (Time.time - lastAttackTime >= (1f / attackRate))
        {
            PerformAttack();
        }
    }
    
    #endregion
    
    /// <summary>
    /// Check for nearby players and start chasing if found
    /// </summary>
    protected virtual bool CheckForPlayers()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, aggroRange);
        
        foreach (Collider hit in hits)
        {
            // Check if it's a player (you'd check for player tag/layer here)
            if (hit.CompareTag("Player"))
            {
                targetPlayer = hit.transform;
                ChangeState(EnemyState.Chasing);
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Perform attack on target
    /// </summary>
    protected virtual void PerformAttack()
    {
        lastAttackTime = Time.time;
        
        // Play attack animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        
        // Play attack sound
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
        
        // Deal damage to target
        if (targetPlayer != null)
        {
            Entity targetEntity = targetPlayer.GetComponent<Entity>();
            if (targetEntity != null && CombatSystem.Instance != null)
            {
                CombatSystem.Instance.QueueDamage(
                    targetEntity,
                    this,
                    baseAttackDamage,
                    DamageType.Physical,
                    WeaponType.Sword, // Enemy default
                    false, // Enemies don't crit by default
                    targetPlayer.position
                );
            }
        }
        
        Debug.Log($"{entityName} attacks for {baseAttackDamage} damage!");
    }
    
    /// <summary>
    /// Set new random patrol target
    /// </summary>
    protected virtual void SetNewPatrolTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += spawnPosition;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolTarget = hit.position;
            navAgent.SetDestination(patrolTarget);
        }
    }
    
    /// <summary>
    /// Update status effects
    /// </summary>
    protected virtual void UpdateStatusEffects()
    {
        if (isFrozen && Time.time >= frozenUntil)
        {
            isFrozen = false;
            navAgent.isStopped = false;
        }
        
        if (isStunned && Time.time >= stunnedUntil)
        {
            isStunned = false;
            navAgent.isStopped = false;
        }
    }
    
    /// <summary>
    /// Apply freeze status effect
    /// </summary>
    public virtual void ApplyFreeze(float duration)
    {
        isFrozen = true;
        frozenUntil = Time.time + duration;
        navAgent.isStopped = true;
        
        Debug.Log($"{entityName} is frozen for {duration} seconds!");
    }
    
    /// <summary>
    /// Apply stun status effect
    /// </summary>
    public virtual void ApplyStun(float duration)
    {
        isStunned = true;
        stunnedUntil = Time.time + duration;
        navAgent.isStopped = true;
        
        Debug.Log($"{entityName} is stunned for {duration} seconds!");
    }
    
    /// <summary>
    /// Called when enemy takes damage
    /// </summary>
    protected virtual void OnDamageTaken(int damage)
    {
        // Play hurt sound
        if (audioSource != null && hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }
        
        // Play hurt animation
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }
        
        // Aggro on damage if idle
        if (currentState == EnemyState.Idle || currentState == EnemyState.Patrolling)
        {
            CheckForPlayers();
        }
    }
    
    /// <summary>
    /// Called when enemy dies
    /// </summary>
    protected virtual void OnDeath()
    {
        ChangeState(EnemyState.Dead);
        
        // Play death sound
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        
        // Play death animation
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
        
        // Disable AI
        navAgent.enabled = false;
        
        // Award rewards
        AwardRewards();
        
        // Destroy after delay
        Destroy(gameObject, 5f);
        
        Debug.Log($"{entityName} has been defeated!");
    }
    
    /// <summary>
    /// Award experience and gold to players
    /// </summary>
    protected virtual void AwardRewards()
    {
        // This would integrate with your player progression system
        Debug.Log($"Awarded {experienceReward} XP and {goldReward} gold");
    }
    
    /// <summary>
    /// Update animations based on state
    /// </summary>
    protected virtual void UpdateAnimations()
    {
        if (animator == null) return;
        
        float speed = navAgent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
        animator.SetBool("IsAttacking", currentState == EnemyState.Attacking);
    }
    
    protected virtual void OnDrawGizmosSelected()
    {
        // Draw aggro range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw patrol radius
        if (canPatrol)
        {
            Gizmos.color = Color.blue;
            Vector3 spawnPos = Application.isPlaying ? spawnPosition : transform.position;
            Gizmos.DrawWireSphere(spawnPos, patrolRadius);
        }
    }
}

