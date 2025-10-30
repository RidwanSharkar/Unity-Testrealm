using UnityEngine;

/// <summary>
/// Third-person camera controller with zoom and orbit
/// Uses RIGHT CLICK for camera control (left click reserved for attacks)
/// </summary>
public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target; // The player character
    
    [Header("Camera Settings")]
    [SerializeField] private float distance = 5f; // Distance from target
    [SerializeField] private float height = 2f; // Height above target
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float zoomSpeed = 2f;
    
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float minVerticalAngle = -20f;
    [SerializeField] private float maxVerticalAngle = 60f;
    
    [Header("Smoothing")]
    [SerializeField] private float smoothSpeed = 10f;
    
    private float currentDistance;
    private float horizontalAngle = 0f; // Yaw (left/right)
    private float verticalAngle = 20f;  // Pitch (up/down)
    private bool rightClickHeld = false;
    
    void Start()
    {
        currentDistance = distance;
        
        // If no target set, try to find Player_Mage
        if (target == null)
        {
            GameObject player = GameObject.Find("Player_Mage");
            if (player != null)
            {
                target = player.transform;
            }
        }
        
        // Initialize camera angles based on target's current rotation
        if (target != null)
        {
            horizontalAngle = target.eulerAngles.y;
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        // Check if right mouse button is held
        rightClickHeld = Input.GetMouseButton(1); // Right click
        
        // Handle camera rotation with RIGHT CLICK + MOUSE MOVEMENT
        if (rightClickHeld)
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
            
            // Horizontal rotation (yaw)
            horizontalAngle += mouseX;
            
            // Vertical rotation (pitch)
            verticalAngle -= mouseY;
            verticalAngle = Mathf.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);
            
            // Rotate the character to face camera direction (horizontal only)
            if (target != null)
            {
                target.rotation = Quaternion.Euler(0f, horizontalAngle, 0f);
            }
        }
        
        // Handle zoom with mouse scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentDistance -= scroll * zoomSpeed;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
        
        // Calculate camera position based on angles
        Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0f);
        Vector3 offset = rotation * (Vector3.back * currentDistance);
        Vector3 desiredPosition = target.position + Vector3.up * height + offset;
        
        // Smoothly move camera to desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        // Look at the target (slightly above for better view)
        Vector3 lookAtPosition = target.position + Vector3.up * height;
        transform.LookAt(lookAtPosition);
    }
    
    // Public property to check if camera is being controlled
    public bool IsControllingCamera => rightClickHeld;
}

