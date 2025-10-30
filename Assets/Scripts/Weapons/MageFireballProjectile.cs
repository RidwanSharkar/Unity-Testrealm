using UnityEngine;

/// <summary>
/// Mage's primary attack fireball projectile.
/// Simple, fast-traveling fire projectile with trail effect.
/// </summary>
public class MageFireballProjectile : Projectile
{
    [Header("Fireball Visual Effects")]
    [SerializeField] private Light fireLight;
    [SerializeField] private float lightIntensity = 2f;
    [SerializeField] private Color fireColor = new Color(1f, 0.5f, 0f, 1f);
    [SerializeField] private ParticleSystem fireParticles;
    
    [Header("Fireball Settings")]
    [SerializeField] private float fireballScale = 1f;
    [SerializeField] private bool rotateOverTime = true;
    [SerializeField] private float rotationSpeed = 360f;
    
    protected override void Awake()
    {
        base.Awake();
        
        // Configure fireball-specific settings
        speed = 25f; // Fast-moving projectile
        lifetime = 5f;
        useGravity = false;
        
        // Setup light
        if (fireLight != null)
        {
            fireLight.color = fireColor;
            fireLight.intensity = lightIntensity;
            fireLight.range = 5f;
        }
        
        // Start particle system
        if (fireParticles != null && !fireParticles.isPlaying)
        {
            fireParticles.Play();
        }
    }
    
    protected override void Update()
    {
        base.Update();
        
        // Rotate fireball over time for visual effect
        if (rotateOverTime)
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
        
        // Pulse light intensity
        if (fireLight != null)
        {
            float pulse = Mathf.Sin(Time.time * 10f) * 0.3f + 1f;
            fireLight.intensity = lightIntensity * pulse;
        }
    }
    
    /// <summary>
    /// Initialize fireball projectile
    /// </summary>
    public override void Initialize(Entity ownerEntity, int projectileDamage, WeaponType type, Vector3 shootDirection)
    {
        base.Initialize(ownerEntity, projectileDamage, type, shootDirection);
        
        // Scale fireball
        transform.localScale = Vector3.one * fireballScale;
        
        // Align fireball with direction
        if (shootDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(shootDirection);
        }
    }
    
    /// <summary>
    /// Override hit entity to add burning effect
    /// </summary>
    protected override void OnHitEntity(Entity target, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnHitEntity(target, hitPoint, hitNormal);
        
        // TODO: Add burning DoT effect to target
        Debug.Log($"Fireball hit {target.EntityName}!");
    }
    
    /// <summary>
    /// Override hit environment for fire scorch mark
    /// </summary>
    protected override void OnHitEnvironment(Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnHitEnvironment(hitPoint, hitNormal);
        
        // TODO: Spawn scorch mark decal
        Debug.Log("Fireball hit environment!");
    }
    
    /// <summary>
    /// Enhanced impact effect for fireball
    /// </summary>
    protected override void SpawnImpactEffect(Vector3 position, Vector3 normal)
    {
        base.SpawnImpactEffect(position, normal);
        
        // Stop particle system
        if (fireParticles != null)
        {
            fireParticles.Stop();
        }
        
        // Create small explosion effect
        // TODO: Add explosion particle effect
    }
    
    /// <summary>
    /// Clean up fireball
    /// </summary>
    protected override void DestroyProjectile()
    {
        // Fade out light
        if (fireLight != null)
        {
            fireLight.intensity = 0f;
        }
        
        base.DestroyProjectile();
    }
}

