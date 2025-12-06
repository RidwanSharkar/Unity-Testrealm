using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages all buildings in the scene and handles building transitions.
/// Ensures only one building is active at a time and manages global building state.
/// </summary>
public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    [Header("Transition Settings")]
    [SerializeField] private float transitionDuration = 0.5f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Camera Settings")]
    [SerializeField] private float interiorFOV = 60f;
    [SerializeField] private float exteriorFOV = 70f;

    // Building state
    private List<Building> buildings = new List<Building>();
    private Building currentBuilding = null;
    private Camera mainCamera;
    private float originalFOV;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Find camera
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalFOV = mainCamera.fieldOfView;
        }

        // Find all buildings in scene
        Building[] sceneBuildings = FindObjectsOfType<Building>();
        buildings.AddRange(sceneBuildings);

        // Subscribe to building events
        foreach (Building building in buildings)
        {
            building.OnPlayerEntered += OnPlayerEnteredBuilding;
            building.OnPlayerExited += OnPlayerExitedBuilding;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        foreach (Building building in buildings)
        {
            if (building != null)
            {
                building.OnPlayerEntered -= OnPlayerEnteredBuilding;
                building.OnPlayerExited -= OnPlayerExitedBuilding;
            }
        }
    }

    /// <summary>
    /// Called when player enters any building
    /// </summary>
    private void OnPlayerEnteredBuilding(Building building)
    {
        Debug.Log($"Player entered building: {building.BuildingName}");

        // Ensure player can only be in one building at a time
        if (currentBuilding != null && currentBuilding != building)
        {
            Debug.LogWarning("Player entered building while already in another building!");
            currentBuilding.ExitBuilding(FindObjectOfType<PlayerController>());
        }

        currentBuilding = building;

        // Start transition effects
        StartCoroutine(TransitionToInterior());
    }

    /// <summary>
    /// Called when player exits any building
    /// </summary>
    private void OnPlayerExitedBuilding(Building building)
    {
        Debug.Log($"Player exited building: {building.BuildingName}");

        if (currentBuilding == building)
        {
            currentBuilding = null;

            // Start transition effects
            StartCoroutine(TransitionToExterior());
        }
    }

    /// <summary>
    /// Smooth transition when entering building interior
    /// </summary>
    private System.Collections.IEnumerator TransitionToInterior()
    {
        if (mainCamera == null) yield break;

        float startFOV = mainCamera.fieldOfView;
        float time = 0;

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            float t = transitionCurve.Evaluate(time / transitionDuration);
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, interiorFOV, t);
            yield return null;
        }

        mainCamera.fieldOfView = interiorFOV;
    }

    /// <summary>
    /// Smooth transition when exiting building to exterior
    /// </summary>
    private System.Collections.IEnumerator TransitionToExterior()
    {
        if (mainCamera == null) yield break;

        float startFOV = mainCamera.fieldOfView;
        float time = 0;

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            float t = transitionCurve.Evaluate(time / transitionDuration);
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, exteriorFOV, t);
            yield return null;
        }

        mainCamera.fieldOfView = exteriorFOV;
    }

    /// <summary>
    /// Register a new building with the manager
    /// </summary>
    public void RegisterBuilding(Building building)
    {
        if (!buildings.Contains(building))
        {
            buildings.Add(building);
            building.OnPlayerEntered += OnPlayerEnteredBuilding;
            building.OnPlayerExited += OnPlayerExitedBuilding;
        }
    }

    /// <summary>
    /// Unregister a building from the manager
    /// </summary>
    public void UnregisterBuilding(Building building)
    {
        buildings.Remove(building);
        building.OnPlayerEntered -= OnPlayerEnteredBuilding;
        building.OnPlayerExited -= OnPlayerExitedBuilding;

        if (currentBuilding == building)
        {
            currentBuilding = null;
        }
    }

    /// <summary>
    /// Get the building the player is currently in
    /// </summary>
    public Building GetCurrentBuilding()
    {
        return currentBuilding;
    }

    /// <summary>
    /// Check if player is inside any building
    /// </summary>
    public bool IsPlayerInsideAnyBuilding()
    {
        return currentBuilding != null;
    }
}
