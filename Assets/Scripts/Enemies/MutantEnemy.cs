using UnityEngine;

/// <summary>
/// Mutant enemy class - a specialized enemy type with high health but moderate damage.
/// </summary>
public class MutantEnemy : BaseEnemy
{
    // Track attack alternation
    private bool usePunchAttack = true;
    private bool isMoving = false;

    protected override void Awake()
    {
        // Set mutant-specific defaults before base Awake
        enemyType = EnemyType.Mutant;
        enemyName = "Mutant";
        baseHealth = 200;
        baseAttackDamage = 5;
        aggroRange = 8f;
        loseAggroDistance = 15f;

        base.Awake();
    }

    /// <summary>
    /// Override animation updates for Mutant-specific animations
    /// </summary>
    protected override void UpdateAnimations()
    {
        // Check if animator exists and is properly initialized with a controller
        if (animator == null)
        {
            Debug.LogError($"{entityName}: Animator is NULL! Check if Animator component exists on the Mutant child object.");
            return;
        }
        
        if (!animator.isActiveAndEnabled)
        {
            Debug.LogError($"{entityName}: Animator is not active or enabled!");
            return;
        }
        
        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogError($"{entityName}: Animator has no controller assigned! Assign Mutant_AnimatorController in the Inspector.");
            return;
        }

        // Check if currently moving (safely check navAgent)
        float currentSpeed = (navAgent != null && navAgent.isOnNavMesh) ? navAgent.velocity.magnitude : 0f;
        bool currentlyMoving = currentSpeed > 0.1f;

        // Handle state transitions
        if (currentlyMoving != isMoving)
        {
            isMoving = currentlyMoving;
            if (isMoving)
            {
                // Transition to run animation
                Debug.Log($"{entityName}: Playing RUN animation (speed: {currentSpeed:F2})");
                animator.SetTrigger("Run");
                animator.ResetTrigger("Idle");
            }
            else
            {
                // Transition to idle animation
                Debug.Log($"{entityName}: Playing IDLE animation");
                animator.SetTrigger("Idle");
                animator.ResetTrigger("Run");
            }
        }

        // Set speed parameter for blend trees
        animator.SetFloat("Speed", currentSpeed);

        // Handle attack state
        animator.SetBool("IsAttacking", currentState == EnemyState.Attacking);
    }

    /// <summary>
    /// Override attack to alternate between punch and swiping animations
    /// </summary>
    protected override void PerformAttack()
    {
        lastAttackTime = Time.time;

        // Alternate between punch and swiping attacks (with animator safety check)
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            if (usePunchAttack)
            {
                Debug.Log($"{entityName}: Triggering PUNCH attack animation!");
                animator.SetTrigger("Punch");
            }
            else
            {
                Debug.Log($"{entityName}: Triggering SWIPING attack animation!");
                animator.SetTrigger("Swiping");
            }
        }
        else
        {
            Debug.LogWarning($"{entityName}: Cannot play attack animation - animator not set up!");
        }

        // Toggle for next attack
        usePunchAttack = !usePunchAttack;

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
    /// Override death to play Mutant_Dying animation
    /// </summary>
    protected override void OnDeath()
    {
        // Play death animation
        if (animator != null)
        {
            animator.SetTrigger("Dying");
            animator.ResetTrigger("Idle");
            animator.ResetTrigger("Run");
            animator.SetBool("IsAttacking", false);
        }

        // Play death sound
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        ChangeState(EnemyState.Dead);

        // Disable AI
        navAgent.enabled = false;

        // Award rewards
        AwardRewards();

        // Destroy after delay (longer for death animation)
        Destroy(gameObject, 8f);

        Debug.Log($"{entityName} has been defeated!");
    }
}
