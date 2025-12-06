using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a building that players can enter/exit.
/// Manages doors, interior positioning, and building state.
/// </summary>
public class Building : MonoBehaviour
{
    [Header("Building Settings")]
    [SerializeField] private string buildingName = "Building";
    [SerializeField] private Vector3 interiorPosition = Vector3.zero; // Where to teleport player inside
    [SerializeField] private Vector3 interiorRotation = Vector3.zero; // Player rotation inside
    [SerializeField] private Vector3 exitPosition = Vector3.zero; // Where to teleport player outside

    [Header("Door Settings")]
    [SerializeField] private List<Door> doors = new List<Door>();
    [SerializeField] private bool isPlayerInside = false;

    [Header("Visual Settings")]
    [SerializeField] private GameObject exteriorModel;
    [SerializeField] private GameObject interiorModel;

    // Events
    public delegate void BuildingEvent(Building building);
    public event BuildingEvent OnPlayerEntered;
    public event BuildingEvent OnPlayerExited;

    // Properties
    public string BuildingName
    {
        get => buildingName;
        set => buildingName = value;
    }
    public Vector3 InteriorPosition
    {
        get => interiorPosition;
        set => interiorPosition = value;
    }
    public Vector3 ExitPosition
    {
        get => exitPosition;
        set => exitPosition = value;
    }
    public bool IsPlayerInside => isPlayerInside;
    public List<Door> Doors => doors;

    void Start()
    {
        // Setup doors
        foreach (Door door in doors)
        {
            if (door != null)
            {
                door.SetBuilding(this);
            }
        }

        // Register with building manager
        if (BuildingManager.Instance != null)
        {
            BuildingManager.Instance.RegisterBuilding(this);
        }

        // Set initial visual state
        UpdateVisualState();
    }

    void OnDestroy()
    {
        // Unregister from building manager
        if (BuildingManager.Instance != null)
        {
            BuildingManager.Instance.UnregisterBuilding(this);
        }
    }

    /// <summary>
    /// Called when player enters the building through a door
    /// </summary>
    public void EnterBuilding(PlayerController player, Door entryDoor)
    {
        if (isPlayerInside) return;

        Debug.Log($"Player entering {buildingName} through {entryDoor.DoorName}");

        // Teleport player to interior position
        if (player.MovementComponent != null)
        {
            player.MovementComponent.TeleportTo(interiorPosition);
            player.transform.rotation = Quaternion.Euler(interiorRotation);
        }

        isPlayerInside = true;
        UpdateVisualState();

        // Notify player controller
        player.OnEnteredBuilding();

        // Notify listeners
        OnPlayerEntered?.Invoke(this);
    }

    /// <summary>
    /// Called when player exits the building
    /// </summary>
    public void ExitBuilding(PlayerController player)
    {
        if (!isPlayerInside) return;

        Debug.Log($"Player exiting {buildingName}");

        // Teleport player to exit position
        if (player.MovementComponent != null)
        {
            player.MovementComponent.TeleportTo(exitPosition);
        }

        isPlayerInside = false;
        UpdateVisualState();

        // Notify player controller
        player.OnExitedBuilding();

        // Notify listeners
        OnPlayerExited?.Invoke(this);
    }

    /// <summary>
    /// Update visual state based on whether player is inside
    /// </summary>
    private void UpdateVisualState()
    {
        // Show/hide exterior and interior models based on player location
        // This is a simple implementation - more complex buildings might need different approaches

        if (exteriorModel != null)
        {
            exteriorModel.SetActive(!isPlayerInside);
        }

        if (interiorModel != null)
        {
            interiorModel.SetActive(isPlayerInside);
        }
    }

    /// <summary>
    /// Add a door to this building
    /// </summary>
    public void AddDoor(Door door)
    {
        if (!doors.Contains(door))
        {
            doors.Add(door);
            door.SetBuilding(this);
        }
    }

    /// <summary>
    /// Remove a door from this building
    /// </summary>
    public void RemoveDoor(Door door)
    {
        doors.Remove(door);
        door.SetBuilding(null);
    }

    void OnDrawGizmosSelected()
    {
        // Draw interior position
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.TransformPoint(interiorPosition), 0.5f);
        Gizmos.DrawLine(transform.position, transform.TransformPoint(interiorPosition));

        // Draw exit position
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.TransformPoint(exitPosition), 0.5f);
        Gizmos.DrawLine(transform.position, transform.TransformPoint(exitPosition));
    }
}
