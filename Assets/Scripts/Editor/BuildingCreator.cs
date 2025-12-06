using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor tool to quickly create basic building prefabs.
/// Creates simple buildings that players can enter.
/// </summary>
public class BuildingCreator : EditorWindow
{
    [Header("Building Dimensions")]
    private float buildingWidth = 8f;
    private float buildingHeight = 6f;
    private float buildingDepth = 8f;
    private float wallThickness = 0.2f;

    [Header("Door Settings")]
    private float doorWidth = 2f;
    private float doorHeight = 3f;
    private Vector3 doorPosition = new Vector3(0, 0, 4f); // Front center

    [Header("Materials")]
    private Color wallColor = new Color(0.8f, 0.7f, 0.6f);
    private Color doorColor = new Color(0.4f, 0.3f, 0.2f);

    [MenuItem("Tools/Create Building")]
    public static void ShowWindow()
    {
        GetWindow<BuildingCreator>("Building Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Simple Building Creator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        GUILayout.Label("Building Dimensions", EditorStyles.boldLabel);
        buildingWidth = EditorGUILayout.FloatField("Width", buildingWidth);
        buildingHeight = EditorGUILayout.FloatField("Height", buildingHeight);
        buildingDepth = EditorGUILayout.FloatField("Depth", buildingDepth);
        wallThickness = EditorGUILayout.FloatField("Wall Thickness", wallThickness);

        EditorGUILayout.Space();
        GUILayout.Label("Door Settings", EditorStyles.boldLabel);
        doorWidth = EditorGUILayout.FloatField("Door Width", doorWidth);
        doorHeight = EditorGUILayout.FloatField("Door Height", doorHeight);
        doorPosition = EditorGUILayout.Vector3Field("Door Position", doorPosition);

        EditorGUILayout.Space();
        GUILayout.Label("Materials", EditorStyles.boldLabel);
        wallColor = EditorGUILayout.ColorField("Wall Color", wallColor);
        doorColor = EditorGUILayout.ColorField("Door Color", doorColor);

        EditorGUILayout.Space();

        if (GUILayout.Button("Create Building", GUILayout.Height(40)))
        {
            CreateBuilding();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This creates a simple building with walls, floor, roof, and an interactive door. The building will be placed at the scene origin.", MessageType.Info);
    }

    private void CreateBuilding()
    {
        // Create building root
        GameObject buildingObj = new GameObject("SimpleBuilding");
        Building building = buildingObj.AddComponent<Building>();
        building.BuildingName = "Simple House";

        // Set interior and exit positions
        building.InteriorPosition = new Vector3(0, 0, 0); // Center of building
        building.ExitPosition = new Vector3(0, 0, buildingDepth/2 + 2); // In front of door

        // Create walls
        CreateWalls(buildingObj, buildingWidth, buildingHeight, buildingDepth, wallThickness, wallColor);

        // Create floor
        CreateFloor(buildingObj, buildingWidth, buildingDepth, wallColor);

        // Create roof
        CreateRoof(buildingObj, buildingWidth, buildingDepth, buildingHeight, wallColor);

        // Create door
        CreateDoor(buildingObj, doorPosition, doorWidth, doorHeight, doorColor, building);

        // Create building prefab
        string prefabPath = "Assets/Prefabs/Buildings/SimpleBuilding.prefab";
        EnsureDirectoryExists("Assets/Prefabs/Buildings");

        PrefabUtility.SaveAsPrefabAsset(buildingObj, prefabPath);
        DestroyImmediate(buildingObj);

        Debug.Log($"Building created and saved as prefab: {prefabPath}");
        EditorUtility.DisplayDialog("Success!", "Building created successfully!\n\nPrefab saved to: Assets/Prefabs/Buildings/SimpleBuilding.prefab", "OK");
    }

    private void CreateWalls(GameObject parent, float width, float height, float depth, float thickness, Color color)
    {
        // Create wall material
        Material wallMaterial = CreateMaterial(color);

        // Front wall (with door opening)
        CreateWall(parent, "FrontWall", new Vector3(0, height/2, depth/2), new Vector3(width, height, thickness), wallMaterial);

        // Back wall
        CreateWall(parent, "BackWall", new Vector3(0, height/2, -depth/2), new Vector3(width, height, thickness), wallMaterial);

        // Left wall
        CreateWall(parent, "LeftWall", new Vector3(-width/2, height/2, 0), new Vector3(thickness, height, depth), wallMaterial);

        // Right wall
        CreateWall(parent, "RightWall", new Vector3(width/2, height/2, 0), new Vector3(thickness, height, depth), wallMaterial);
    }

    private void CreateWall(GameObject parent, string name, Vector3 position, Vector3 scale, Material material)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.parent = parent.transform;
        wall.transform.localPosition = position;
        wall.transform.localScale = scale;

        Renderer renderer = wall.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }
    }

    private void CreateFloor(GameObject parent, float width, float depth, Color color)
    {
        Material floorMaterial = CreateMaterial(color);
        CreateWall(parent, "Floor", new Vector3(0, 0, 0), new Vector3(width, 0.1f, depth), floorMaterial);
    }

    private void CreateRoof(GameObject parent, float width, float depth, float height, Color color)
    {
        Material roofMaterial = CreateMaterial(color);
        CreateWall(parent, "Roof", new Vector3(0, height, 0), new Vector3(width, 0.1f, depth), roofMaterial);
    }

    private void CreateDoor(GameObject parent, Vector3 position, float width, float height, Color color, Building building)
    {
        // Create door frame (remove part of front wall for door opening)
        // This is a simple implementation - in a real game you'd model this properly

        // Create door object
        GameObject doorObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        doorObj.name = "Door";
        doorObj.transform.parent = parent.transform;
        doorObj.transform.localPosition = position;
        doorObj.transform.localScale = new Vector3(width, height, 0.1f);

        // Add door component
        Door door = doorObj.AddComponent<Door>();
        door.DoorName = "Front Door";

        // Set door material
        Material doorMaterial = CreateMaterial(color);
        Renderer renderer = doorObj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = doorMaterial;
        }

        // Add collider for interaction
        BoxCollider triggerCollider = doorObj.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = new Vector3(width * 2, height * 2, 2f);

        // Set door hinge position
        door.DoorTransform = doorObj.transform;
        door.ClosedRotation = Vector3.zero;
        door.OpenRotation = new Vector3(0, -90, 0); // Open inward
    }

    private Material CreateMaterial(Color color)
    {
        Material material = new Material(Shader.Find("Standard"));
        material.color = color;
        return material;
    }

    private void EnsureDirectoryExists(string path)
    {
        string fullPath = Application.dataPath + path.Substring("Assets".Length);
        if (!System.IO.Directory.Exists(fullPath))
        {
            System.IO.Directory.CreateDirectory(fullPath);
            AssetDatabase.Refresh();
        }
    }
}
