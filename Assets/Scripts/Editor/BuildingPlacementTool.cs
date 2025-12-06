using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Editor tool for placing buildings in the scene with proper positioning and setup.
/// Allows quick placement of buildings with automatic terrain alignment.
/// </summary>
public class BuildingPlacementTool : EditorWindow
{
    [Header("Building Selection")]
    private GameObject selectedBuildingPrefab;
    private string[] buildingOptions = { "Simple Building", "Custom Building..." };
    private int selectedBuildingIndex = 0;

    [Header("Placement Settings")]
    private bool alignToTerrain = true;
    private bool snapToGrid = false;
    private float gridSize = 5f;
    private float placementHeight = 0f;

    [Header("Terrain Settings")]
    private Terrain activeTerrain;
    private LayerMask terrainLayer = 1 << 0; // Default layer

    [MenuItem("Tools/Building Placement Tool")]
    public static void ShowWindow()
    {
        GetWindow<BuildingPlacementTool>("Building Placement");
    }

    private void OnEnable()
    {
        // Find active terrain
        activeTerrain = Terrain.activeTerrain;

        // Load building prefabs
        LoadBuildingPrefabs();

        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        GUILayout.Label("Building Placement Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Building selection
        GUILayout.Label("Building Type", EditorStyles.boldLabel);
        selectedBuildingIndex = EditorGUILayout.Popup("Building:", selectedBuildingIndex, buildingOptions);

        if (selectedBuildingIndex == 0) // Simple Building
        {
            selectedBuildingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Buildings/SimpleBuilding.prefab");
        }
        else // Custom building
        {
            selectedBuildingPrefab = (GameObject)EditorGUILayout.ObjectField("Custom Prefab:", selectedBuildingPrefab, typeof(GameObject), false);
        }

        EditorGUILayout.Space();

        // Placement settings
        GUILayout.Label("Placement Options", EditorStyles.boldLabel);
        alignToTerrain = EditorGUILayout.Toggle("Align to Terrain", alignToTerrain);
        snapToGrid = EditorGUILayout.Toggle("Snap to Grid", snapToGrid);

        if (snapToGrid)
        {
            gridSize = EditorGUILayout.FloatField("Grid Size", gridSize);
        }

        if (!alignToTerrain)
        {
            placementHeight = EditorGUILayout.FloatField("Height Offset", placementHeight);
        }

        EditorGUILayout.Space();

        // Terrain settings
        GUILayout.Label("Terrain", EditorStyles.boldLabel);
        activeTerrain = (Terrain)EditorGUILayout.ObjectField("Active Terrain:", activeTerrain, typeof(Terrain), true);

        EditorGUILayout.Space();

        // Action buttons
        if (GUILayout.Button("Place Building at Mouse", GUILayout.Height(30)))
        {
            PlaceBuildingAtMouse();
        }

        if (GUILayout.Button("Place Building at Origin", GUILayout.Height(30)))
        {
            PlaceBuildingAtPosition(Vector3.zero);
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Click and drag in the scene view to preview building placement. Click 'Place Building at Mouse' to place at current mouse position.", MessageType.Info);
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (selectedBuildingPrefab == null) return;

        // Get mouse position
        Vector2 mousePos = Event.current.mousePosition;
        mousePos.y = sceneView.camera.pixelHeight - mousePos.y;

        Ray ray = sceneView.camera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, terrainLayer))
        {
            Vector3 placementPos = hit.point;

            // Apply placement settings
            if (alignToTerrain && activeTerrain != null)
            {
                placementPos.y = activeTerrain.SampleHeight(placementPos) + placementHeight;
            }
            else
            {
                placementPos.y = placementHeight;
            }

            if (snapToGrid)
            {
                placementPos.x = Mathf.Round(placementPos.x / gridSize) * gridSize;
                placementPos.z = Mathf.Round(placementPos.z / gridSize) * gridSize;
            }

            // Draw preview
            DrawBuildingPreview(placementPos);

            // Place building on mouse click
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && !Event.current.alt)
            {
                PlaceBuildingAtPosition(placementPos);
                Event.current.Use();
            }
        }
    }

    private void DrawBuildingPreview(Vector3 position)
    {
        if (selectedBuildingPrefab == null) return;

        // Get building bounds for preview
        Bounds bounds = new Bounds();
        Renderer[] renderers = selectedBuildingPrefab.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }
        }

        // Draw wireframe preview
        Gizmos.color = Color.green;
        Vector3 previewSize = bounds.size;
        Gizmos.DrawWireCube(position + bounds.center - selectedBuildingPrefab.transform.position, previewSize);

        // Draw placement marker
        Handles.color = Color.yellow;
        Handles.DrawSolidDisc(position, Vector3.up, 0.5f);
    }

    private void PlaceBuildingAtMouse()
    {
        // This would be called from a button, but placement is handled in OnSceneGUI
        Debug.Log("Use mouse click in scene view to place buildings, or use 'Place Building at Origin'");
    }

    private void PlaceBuildingAtPosition(Vector3 position)
    {
        if (selectedBuildingPrefab == null)
        {
            Debug.LogError("No building prefab selected!");
            return;
        }

        // Instantiate building
        GameObject buildingInstance = (GameObject)PrefabUtility.InstantiatePrefab(selectedBuildingPrefab);
        buildingInstance.transform.position = position;

        // Set a unique name
        buildingInstance.name = $"{selectedBuildingPrefab.name}_{System.DateTime.Now.ToString("HHmmss")}";

        // Mark scene as dirty
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log($"Placed building '{buildingInstance.name}' at position {position}");
    }

    private void LoadBuildingPrefabs()
    {
        // Check if default building exists
        GameObject simpleBuilding = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Buildings/SimpleBuilding.prefab");
        if (simpleBuilding != null)
        {
            buildingOptions[0] = "Simple Building (Available)";
        }
        else
        {
            buildingOptions[0] = "Simple Building (Not Found - Create First)";
        }
    }
}
