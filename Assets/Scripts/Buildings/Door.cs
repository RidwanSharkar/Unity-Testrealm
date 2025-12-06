using UnityEngine;

/// <summary>
/// Represents a door that players can interact with to enter/exit buildings.
/// Handles interaction detection, opening/closing animations, and building transitions.
/// </summary>
public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private string doorName = "Door";
    [SerializeField] private bool isOpen = false;
    [SerializeField] private bool isLocked = false;

    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private KeyCode interactionKey = KeyCode.X;
    [SerializeField] private string interactionPrompt = "Press X to interact";
    [SerializeField] private InteractionPrompt interactionPromptUI;

    [Header("Animation Settings")]
    [SerializeField] private Transform doorTransform;
    [SerializeField] private Vector3 closedRotation = Vector3.zero;
    [SerializeField] private Vector3 openRotation = new Vector3(0, 90, 0);
    [SerializeField] private float animationSpeed = 2f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioSource audioSource;

    // References
    private Building building;
    private PlayerController nearbyPlayer;
    private bool isAnimating = false;

    // Properties
    public string DoorName
    {
        get => doorName;
        set => doorName = value;
    }
    public Transform DoorTransform
    {
        get => doorTransform;
        set => doorTransform = value;
    }
    public Vector3 ClosedRotation
    {
        get => closedRotation;
        set => closedRotation = value;
    }
    public Vector3 OpenRotation
    {
        get => openRotation;
        set => openRotation = value;
    }
    public bool IsOpen => isOpen;
    public bool IsLocked
    {
        get => isLocked;
        set => SetLocked(value);
    }
    public Building Building => building;

    void Start()
    {
        // Set initial door state
        if (doorTransform == null)
        {
            doorTransform = transform;
        }

        // Find or create interaction prompt
        if (interactionPromptUI == null)
        {
            interactionPromptUI = FindObjectOfType<InteractionPrompt>();
            if (interactionPromptUI == null)
            {
                GameObject promptObj = new GameObject("InteractionPrompt");
                interactionPromptUI = promptObj.AddComponent<InteractionPrompt>();
            }
        }

        UpdateDoorVisual();
    }

    void Update()
    {
        // Check for player interaction
        if (nearbyPlayer != null && !isAnimating)
        {
            float distance = Vector3.Distance(transform.position, nearbyPlayer.transform.position);

            if (distance <= interactionDistance)
            {
                // Show interaction prompt
                if (interactionPromptUI != null)
                {
                    string prompt = isLocked ? "Door is locked" : interactionPrompt;
                    interactionPromptUI.ShowPrompt(prompt);
                }

                if (Input.GetKeyDown(interactionKey) && !isLocked)
                {
                    Interact(nearbyPlayer);
                }
            }
        }
        else
        {
            // Hide prompt when not near door
            if (interactionPromptUI != null)
            {
                interactionPromptUI.HidePrompt();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            nearbyPlayer = player;
            Debug.Log($"Player near door: {doorName}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null && player == nearbyPlayer)
        {
            nearbyPlayer = null;
            Debug.Log($"Player left door area: {doorName}");
        }
    }

    /// <summary>
    /// Handle player interaction with the door
    /// </summary>
    public void Interact(PlayerController player)
    {
        if (isLocked)
        {
            Debug.Log($"Door {doorName} is locked!");
            // Play locked sound or show message
            return;
        }

        if (building == null)
        {
            Debug.LogWarning($"Door {doorName} has no associated building!");
            return;
        }

        // Toggle door state
        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();

            // Handle building entry/exit
            if (building.IsPlayerInside)
            {
                // Player is inside, let them exit
                building.ExitBuilding(player);
            }
            else
            {
                // Player is outside, let them enter
                building.EnterBuilding(player, this);
            }
        }
    }

    /// <summary>
    /// Open the door
    /// </summary>
    public void OpenDoor()
    {
        if (isAnimating || isOpen) return;

        Debug.Log($"Opening door: {doorName}");
        isAnimating = true;
        isOpen = true;

        // Play sound
        PlaySound(openSound);

        // Start animation
        StartCoroutine(AnimateDoor(openRotation));
    }

    /// <summary>
    /// Close the door
    /// </summary>
    public void CloseDoor()
    {
        if (isAnimating || !isOpen) return;

        Debug.Log($"Closing door: {doorName}");
        isAnimating = true;
        isOpen = false;

        // Play sound
        PlaySound(closeSound);

        // Start animation
        StartCoroutine(AnimateDoor(closedRotation));
    }

    /// <summary>
    /// Animate door opening/closing
    /// </summary>
    private System.Collections.IEnumerator AnimateDoor(Vector3 targetRotation)
    {
        Quaternion startRotation = doorTransform.localRotation;
        Quaternion endRotation = Quaternion.Euler(targetRotation);
        float time = 0;

        while (time < 1f)
        {
            time += Time.deltaTime * animationSpeed;
            doorTransform.localRotation = Quaternion.Lerp(startRotation, endRotation, time);
            yield return null;
        }

        doorTransform.localRotation = endRotation;
        isAnimating = false;
    }

    /// <summary>
    /// Update door visual state
    /// </summary>
    private void UpdateDoorVisual()
    {
        if (doorTransform != null)
        {
            doorTransform.localRotation = Quaternion.Euler(isOpen ? openRotation : closedRotation);
        }
    }

    /// <summary>
    /// Play door sound
    /// </summary>
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// Set the building this door belongs to
    /// </summary>
    public void SetBuilding(Building newBuilding)
    {
        if (building != null)
        {
            building.RemoveDoor(this);
        }

        building = newBuilding;

        if (building != null)
        {
            building.AddDoor(this);
        }
    }

    /// <summary>
    /// Lock or unlock the door
    /// </summary>
    public void SetLocked(bool locked)
    {
        isLocked = locked;
        Debug.Log($"Door {doorName} {(locked ? "locked" : "unlocked")}");
    }

    void OnDrawGizmosSelected()
    {
        // Draw interaction radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);

        // Draw door swing direction
        if (doorTransform != null)
        {
            Gizmos.color = Color.blue;
            Vector3 swingDirection = doorTransform.TransformDirection(Vector3.forward);
            Gizmos.DrawLine(transform.position, transform.position + swingDirection * 2f);
        }
    }
}
