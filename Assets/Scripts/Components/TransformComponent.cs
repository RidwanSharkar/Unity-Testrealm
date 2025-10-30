using UnityEngine;

/// <summary>
/// Enhanced transform component with additional utilities.
/// Provides smooth movement, rotation, and transform operations.
/// </summary>
public class TransformComponent : MonoBehaviour
{
    [Header("Transform State")]
    [SerializeField] private Vector3 position;
    [SerializeField] private Quaternion rotation;
    [SerializeField] private Vector3 scale = Vector3.one;
    
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private bool smoothMovement = false;
    [SerializeField] private float smoothTime = 0.1f;
    
    private Vector3 velocity = Vector3.zero;
    private Transform cachedTransform;
    
    // Properties
    public Vector3 Position => position;
    public Quaternion Rotation => rotation;
    public Vector3 Scale => scale;
    public Vector3 Forward => rotation * Vector3.forward;
    public Vector3 Right => rotation * Vector3.right;
    public Vector3 Up => rotation * Vector3.up;
    
    void Awake()
    {
        cachedTransform = transform;
        SyncFromTransform();
    }
    
    void LateUpdate()
    {
        ApplyTransform();
    }
    
    /// <summary>
    /// Sync internal state from Unity Transform
    /// </summary>
    public void SyncFromTransform()
    {
        position = cachedTransform.position;
        rotation = cachedTransform.rotation;
        scale = cachedTransform.localScale;
    }
    
    /// <summary>
    /// Apply internal state to Unity Transform
    /// </summary>
    private void ApplyTransform()
    {
        if (smoothMovement)
        {
            cachedTransform.position = Vector3.SmoothDamp(
                cachedTransform.position, 
                position, 
                ref velocity, 
                smoothTime
            );
        }
        else
        {
            cachedTransform.position = position;
        }
        
        cachedTransform.rotation = rotation;
        cachedTransform.localScale = scale;
    }
    
    /// <summary>
    /// Set position directly
    /// </summary>
    public void SetPosition(Vector3 newPosition)
    {
        position = newPosition;
    }
    
    /// <summary>
    /// Set rotation directly
    /// </summary>
    public void SetRotation(Quaternion newRotation)
    {
        rotation = newRotation;
    }
    
    /// <summary>
    /// Set rotation from euler angles
    /// </summary>
    public void SetRotation(Vector3 eulerAngles)
    {
        rotation = Quaternion.Euler(eulerAngles);
    }
    
    /// <summary>
    /// Set scale directly
    /// </summary>
    public void SetScale(Vector3 newScale)
    {
        scale = newScale;
    }
    
    /// <summary>
    /// Translate by delta
    /// </summary>
    public void Translate(Vector3 delta)
    {
        position += delta;
    }
    
    /// <summary>
    /// Translate in local space
    /// </summary>
    public void TranslateLocal(Vector3 localDelta)
    {
        position += rotation * localDelta;
    }
    
    /// <summary>
    /// Move towards a target position
    /// </summary>
    public void MoveTowards(Vector3 targetPosition, float deltaTime)
    {
        Vector3 direction = (targetPosition - position).normalized;
        position += direction * movementSpeed * deltaTime;
    }
    
    /// <summary>
    /// Rotate towards a target rotation
    /// </summary>
    public void RotateTowards(Quaternion targetRotation, float deltaTime)
    {
        rotation = Quaternion.Slerp(rotation, targetRotation, rotationSpeed * deltaTime);
    }
    
    /// <summary>
    /// Look at a target position
    /// </summary>
    public void LookAt(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - position).normalized;
        if (direction != Vector3.zero)
        {
            rotation = Quaternion.LookRotation(direction);
        }
    }
    
    /// <summary>
    /// Rotate around axis
    /// </summary>
    public void Rotate(Vector3 axis, float angle)
    {
        rotation *= Quaternion.AngleAxis(angle, axis);
    }
    
    /// <summary>
    /// Get distance to a point
    /// </summary>
    public float DistanceTo(Vector3 point)
    {
        return Vector3.Distance(position, point);
    }
    
    /// <summary>
    /// Get direction to a point
    /// </summary>
    public Vector3 DirectionTo(Vector3 point)
    {
        return (point - position).normalized;
    }
    
    /// <summary>
    /// Set movement speed
    /// </summary>
    public void SetMovementSpeed(float speed)
    {
        movementSpeed = speed;
    }
    
    /// <summary>
    /// Get movement speed
    /// </summary>
    public float GetMovementSpeed()
    {
        return movementSpeed;
    }
}

