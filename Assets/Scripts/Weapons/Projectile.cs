using UnityEngine;
using System.Collections;

/// <summary>
/// Base projectile class for all projectile-based attacks.
/// Handles movement, collision, lifetime, and visual effects.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] protected float speed = 20f;
    [SerializeField] protected float lifetime = 5f;
    [SerializeField] protected bool useGravity = false;
    [SerializeField] protected LayerMask collisionMask;
    
    [Header("Visual Effects")]
    [SerializeField] protected TrailRenderer trailRenderer;
    [SerializeField] protected GameObject impactEffectPrefab;
    [SerializeField] protected float impactEffectLifetime = 2f;
    
    [Header("Audio")]
    [SerializeField] protected AudioClip launchSound;
    [SerializeField] protected AudioClip impactSound;
    [SerializeField] protected AudioSource audioSource;
    
    // Projectile data
    protected Entity owner;
    protected int damage;
    protected WeaponType weaponType;
    protected Vector3 direction;
    protected bool hasHit = false;
    
    // Components
    protected Rigidbody rb;
    protected Collider projectileCollider;
    
    // Lifetime timer
    protected float lifetimeTimer = 0f;
    
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        projectileCollider = GetComponent<Collider>();
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        if (trailRenderer == null)
            trailRenderer = GetComponentInChildren<TrailRenderer>();
        
        // Configure rigidbody
        if (rb != null)
        {
            rb.useGravity = useGravity;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }
    
    protected virtual void Update()
    {
        // Update lifetime
        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer >= lifetime)
        {
            DestroyProjectile();
        }
    }
    
    /// <summary>
    /// Initialize projectile with damage and owner information
    /// </summary>
    public virtual void Initialize(Entity ownerEntity, int projectileDamage, WeaponType type, Vector3 shootDirection)
    {
        owner = ownerEntity;
        damage = projectileDamage;
        weaponType = type;
        direction = shootDirection.normalized;
        
        // Set velocity
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
        
        // Play launch sound
        PlaySound(launchSound);
        
        // Ignore collision with owner
        if (owner != null && projectileCollider != null)
        {
            Collider ownerCollider = owner.GetComponent<Collider>();
            if (ownerCollider != null)
            {
                Physics.IgnoreCollision(projectileCollider, ownerCollider);
            }
        }
        
        // Reset state
        hasHit = false;
        lifetimeTimer = 0f;
    }
    
    /// <summary>
    /// Handle collision with objects
    /// </summary>
    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;
        
        // Check if we should collide with this object
        if (!ShouldCollideWith(collision.gameObject))
            return;
        
        hasHit = true;
        
        // Get contact point
        ContactPoint contact = collision.contacts[0];
        Vector3 hitPoint = contact.point;
        Vector3 hitNormal = contact.normal;
        
        // Check if hit an entity
        Entity target = collision.gameObject.GetComponent<Entity>();
        if (target != null && target != owner)
        {
            OnHitEntity(target, hitPoint, hitNormal);
        }
        else
        {
            OnHitEnvironment(hitPoint, hitNormal);
        }
        
        // Spawn impact effect and destroy
        SpawnImpactEffect(hitPoint, hitNormal);
        DestroyProjectile();
    }
    
    /// <summary>
    /// Check if projectile should collide with object
    /// </summary>
    protected virtual bool ShouldCollideWith(GameObject obj)
    {
        // Don't collide with owner
        if (obj.transform == owner.transform)
            return false;
        
        // Check layer mask
        int objLayer = 1 << obj.layer;
        return (collisionMask.value & objLayer) != 0;
    }
    
    /// <summary>
    /// Called when projectile hits an entity
    /// </summary>
    protected virtual void OnHitEntity(Entity target, Vector3 hitPoint, Vector3 hitNormal)
    {
        // Calculate and apply damage
        DamageResult result = DamageCalculator.CalculateDamage(damage, weaponType);
        
        if (CombatSystem.Instance != null)
        {
            CombatSystem.Instance.QueueDamage(
                target,
                owner,
                result.damage,
                result.damageType,
                weaponType,
                result.isCritical,
                hitPoint,
                hitNormal
            );
        }
    }
    
    /// <summary>
    /// Called when projectile hits environment (walls, ground, etc.)
    /// </summary>
    protected virtual void OnHitEnvironment(Vector3 hitPoint, Vector3 hitNormal)
    {
        // Override in derived classes for special effects
    }
    
    /// <summary>
    /// Spawn impact effect at hit location
    /// </summary>
    protected virtual void SpawnImpactEffect(Vector3 position, Vector3 normal)
    {
        if (impactEffectPrefab != null)
        {
            // Align effect with surface normal
            Quaternion rotation = Quaternion.LookRotation(normal);
            GameObject effect = Instantiate(impactEffectPrefab, position, rotation);
            Destroy(effect, impactEffectLifetime);
        }
        
        // Play impact sound
        PlaySound(impactSound);
    }
    
    /// <summary>
    /// Play sound effect
    /// </summary>
    protected void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    /// <summary>
    /// Destroy projectile
    /// </summary>
    protected virtual void DestroyProjectile()
    {
        // Detach trail renderer so it finishes naturally
        if (trailRenderer != null)
        {
            trailRenderer.transform.SetParent(null);
            Destroy(trailRenderer.gameObject, trailRenderer.time);
        }
        
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Get projectile speed
    /// </summary>
    public float Speed => speed;
    
    /// <summary>
    /// Set projectile speed
    /// </summary>
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
    }
}

