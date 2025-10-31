using UnityEngine;

/// <summary>
/// Creates a pair of projectiles that spiral around each other while moving forward
/// Perfect for magical projectile effects like void spheres, twin fireballs, etc.
/// </summary>
public class SpiralProjectilePair : MonoBehaviour
{
    [Header("Spiral Settings")]
    [SerializeField] private float forwardSpeed = 25f;
    [SerializeField] private float spiralRadius = 0.5f;
    [SerializeField] private float spiralSpeed = 360f; // Degrees per second
    [SerializeField] private float lifetime = 5f;
    
    [Header("Projectile Prefabs")]
    [SerializeField] private GameObject projectilePrefab;
    
    [Header("Auto-Setup")]
    [SerializeField] private bool useExistingChildren = false; // If true, uses child projectiles instead of spawning
    
    private GameObject projectile1;
    private GameObject projectile2;
    private Vector3 direction;
    private float currentAngle = 0f;
    private float timeAlive = 0f;
    private float distanceTraveled = 0f;
    private Entity owner;
    private int damage;
    private WeaponType weaponType;
    
    // Grace period settings
    private const float minDistanceBeforeTerrainCollision = 7f; // Must travel 2 units before terrain can destroy it
    
    private void Start()
    {
        Debug.Log($"[SpiralProjectilePair] Start called at position {transform.position}");
        
        if (useExistingChildren)
        {
            // Use existing child projectiles
            if (transform.childCount >= 2)
            {
                projectile1 = transform.GetChild(0).gameObject;
                projectile2 = transform.GetChild(1).gameObject;
                Debug.Log("[SpiralProjectilePair] Using existing child projectiles");
                
                // Disable physics on existing children
                DisableProjectilePhysics(projectile1);
                DisableProjectilePhysics(projectile2);
            }
            else
            {
                Debug.LogError("[SpiralProjectilePair] useExistingChildren is true but less than 2 children found!");
            }
        }
        else if (projectilePrefab != null)
        {
            // Spawn two projectiles as children
            Debug.Log($"[SpiralProjectilePair] Spawning projectiles from prefab: {projectilePrefab.name}");
            projectile1 = Instantiate(projectilePrefab, transform);
            projectile2 = Instantiate(projectilePrefab, transform);
            
            // Make sure they're active
            projectile1.SetActive(true);
            projectile2.SetActive(true);
            
            Debug.Log($"[SpiralProjectilePair] Projectile1: {projectile1.name}, Active: {projectile1.activeSelf}");
            Debug.Log($"[SpiralProjectilePair] Projectile2: {projectile2.name}, Active: {projectile2.activeSelf}");
            
            // Disable physics on individual projectiles (parent will handle movement)
            DisableProjectilePhysics(projectile1);
            DisableProjectilePhysics(projectile2);
            
            Debug.Log("[SpiralProjectilePair] Spawned and configured two spiral projectiles");
        }
        else
        {
            Debug.LogError("[SpiralProjectilePair] No projectile prefab assigned and useExistingChildren is false!");
        }
        
        // Set initial positions
        if (projectile1 != null && projectile2 != null)
        {
            projectile1.transform.localPosition = new Vector3(spiralRadius, 0, 0);
            projectile2.transform.localPosition = new Vector3(-spiralRadius, 0, 0);
            Debug.Log($"[SpiralProjectilePair] Set initial positions: P1={projectile1.transform.position}, P2={projectile2.transform.position}");
        }
        else
        {
            Debug.LogError("[SpiralProjectilePair] Failed to create/find projectiles!");
        }
    }
    
    private void Update()
    {
        if (direction == Vector3.zero)
        {
            Debug.LogWarning("[SpiralProjectilePair] Direction is zero! Not initialized properly?");
            return;
        }
        
        // Move forward
        float moveDistance = forwardSpeed * Time.deltaTime;
        Vector3 movement = direction * moveDistance;
        transform.position += movement;
        distanceTraveled += moveDistance;
        
        // Update spiral rotation
        currentAngle += spiralSpeed * Time.deltaTime;
        if (currentAngle >= 360f)
        {
            currentAngle -= 360f;
        }
        
        // Update projectile positions in spiral pattern
        if (projectile1 != null && projectile2 != null)
        {
            float angle1Rad = currentAngle * Mathf.Deg2Rad;
            float angle2Rad = (currentAngle + 180f) * Mathf.Deg2Rad; // Opposite side
            
            // Calculate positions relative to center (in local space)
            Vector3 offset1 = new Vector3(
                Mathf.Cos(angle1Rad) * spiralRadius,
                Mathf.Sin(angle1Rad) * spiralRadius,
                0
            );
            
            Vector3 offset2 = new Vector3(
                Mathf.Cos(angle2Rad) * spiralRadius,
                Mathf.Sin(angle2Rad) * spiralRadius,
                0
            );
            
            // Apply in local space (relative to direction)
            projectile1.transform.localPosition = offset1;
            projectile2.transform.localPosition = offset2;
        }
        else
        {
            Debug.LogWarning("[SpiralProjectilePair] Projectiles are null in Update!");
        }
        
        // Check lifetime
        timeAlive += Time.deltaTime;
        if (timeAlive >= lifetime)
        {
            Debug.Log("[SpiralProjectilePair] Lifetime expired, destroying");
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Initialize the spiral projectile pair
    /// </summary>
    public void Initialize(Entity ownerEntity, int projectileDamage, WeaponType type, Vector3 shootDirection)
    {
        owner = ownerEntity;
        damage = projectileDamage;
        weaponType = type;
        direction = shootDirection.normalized;
        
        // Face the direction of travel
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        Debug.Log($"[SpiralProjectilePair] Initialized: Direction={direction}, Damage={damage}, Speed={forwardSpeed}");
    }
    
    /// <summary>
    /// Disable physics on individual projectiles so they don't collide independently
    /// </summary>
    private void DisableProjectilePhysics(GameObject projectile)
    {
        // Disable the projectile script itself to prevent it from moving independently
        MageFireballProjectile mageFireball = projectile.GetComponent<MageFireballProjectile>();
        if (mageFireball != null)
        {
            mageFireball.enabled = false;
            Debug.Log("[SpiralProjectilePair] Disabled MageFireballProjectile script");
        }
        
        Projectile proj = projectile.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.enabled = false;
            Debug.Log("[SpiralProjectilePair] Disabled Projectile script");
        }
        
        // Make rigidbody kinematic but keep it active for rendering
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // IMPORTANT: Set velocity BEFORE making kinematic
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true; // Set this AFTER velocity
            Debug.Log("[SpiralProjectilePair] Made Rigidbody kinematic");
        }
        
        // Disable individual colliders (parent handles collision)
        Collider col = projectile.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
            Debug.Log("[SpiralProjectilePair] Disabled individual collider");
        }
    }
    
    /// <summary>
    /// Handle collision with the pair as a whole
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Ignore collision with owner
        if (owner != null && other.gameObject == owner.gameObject)
        {
            Debug.Log($"[SpiralProjectilePair] Ignoring collision with owner: {other.name}");
            return;
        }
        
        // Check if hit an entity (always allow entity hits)
        Entity target = other.GetComponent<Entity>();
        if (target != null && target != owner)
        {
            Debug.Log($"[SpiralProjectilePair] Hit entity: {target.name}");
            
            // Apply damage
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
                    other.ClosestPoint(transform.position),
                    (transform.position - other.transform.position).normalized
                );
            }
            
            // Destroy the spiral pair
            Destroy(gameObject);
            return;
        }
        
        // Check for terrain/environment collision (only after grace period)
        if (distanceTraveled < minDistanceBeforeTerrainCollision)
        {
            // Still in grace period - ignore terrain collisions
            Debug.Log($"[SpiralProjectilePair] Ignoring terrain during grace period ({distanceTraveled:F2}/{minDistanceBeforeTerrainCollision}): {other.name}");
            return;
        }
        
        // After grace period, destroy on environment/terrain hit
        if (other.gameObject.layer == LayerMask.NameToLayer("Environment") || 
            other.gameObject.layer == LayerMask.NameToLayer("Default") ||
            other.gameObject.name.Contains("Terrain"))
        {
            Debug.Log($"[SpiralProjectilePair] Hit environment after traveling {distanceTraveled:F2}m: {other.name}");
            Destroy(gameObject);
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        // Ignore collision with owner
        if (owner != null && collision.gameObject == owner.gameObject)
        {
            Debug.Log($"[SpiralProjectilePair] Ignoring collision with owner: {collision.gameObject.name}");
            return;
        }
        
        // Handle non-trigger collisions
        Entity target = collision.gameObject.GetComponent<Entity>();
        if (target != null && target != owner)
        {
            Debug.Log($"[SpiralProjectilePair] Collision with entity: {target.name}");
            
            // Apply damage
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
                    collision.contacts[0].point,
                    collision.contacts[0].normal
                );
            }
            
            Destroy(gameObject);
            return;
        }
        
        // Check for terrain collision grace period
        if (distanceTraveled < minDistanceBeforeTerrainCollision)
        {
            Debug.Log($"[SpiralProjectilePair] Ignoring terrain collision during grace period ({distanceTraveled:F2}/{minDistanceBeforeTerrainCollision})");
            return;
        }
        
        // Destroy on any collision (except owner) after grace period
        Debug.Log($"[SpiralProjectilePair] Collision detected after {distanceTraveled:F2}m, destroying");
        Destroy(gameObject);
    }
}

