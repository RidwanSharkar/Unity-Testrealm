using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor tool to create basic interior environments for buildings.
/// Adds furniture, decorations, and lighting to building interiors.
/// </summary>
public class InteriorCreator : EditorWindow
{
    [Header("Room Dimensions")]
    private float roomWidth = 6f;
    private float roomHeight = 3f;
    private float roomDepth = 6f;

    [Header("Furniture")]
    private bool addTable = true;
    private bool addChairs = true;
    private bool addBed = true;
    private bool addChest = true;
    private bool addLighting = true;

    [Header("Materials")]
    private Color woodColor = new Color(0.6f, 0.4f, 0.2f);
    private Color fabricColor = new Color(0.8f, 0.8f, 0.9f);

    [MenuItem("Tools/Create Building Interior")]
    public static void ShowWindow()
    {
        GetWindow<InteriorCreator>("Interior Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Building Interior Creator", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        GUILayout.Label("Room Dimensions", EditorStyles.boldLabel);
        roomWidth = EditorGUILayout.FloatField("Width", roomWidth);
        roomHeight = EditorGUILayout.FloatField("Height", roomHeight);
        roomDepth = EditorGUILayout.FloatField("Depth", roomDepth);

        EditorGUILayout.Space();
        GUILayout.Label("Furniture", EditorStyles.boldLabel);
        addTable = EditorGUILayout.Toggle("Table", addTable);
        addChairs = EditorGUILayout.Toggle("Chairs", addChairs);
        addBed = EditorGUILayout.Toggle("Bed", addBed);
        addChest = EditorGUILayout.Toggle("Chest", addChest);
        addLighting = EditorGUILayout.Toggle("Lighting", addLighting);

        EditorGUILayout.Space();
        GUILayout.Label("Materials", EditorStyles.boldLabel);
        woodColor = EditorGUILayout.ColorField("Wood Color", woodColor);
        fabricColor = EditorGUILayout.ColorField("Fabric Color", fabricColor);

        EditorGUILayout.Space();

        if (GUILayout.Button("Create Interior", GUILayout.Height(40)))
        {
            CreateInterior();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This creates a basic interior environment with furniture and lighting. The interior will be created at the selected building or at the scene origin.", MessageType.Info);
    }

    private void CreateInterior()
    {
        GameObject interiorObj = new GameObject("BuildingInterior");

        // Create materials
        Material woodMaterial = CreateMaterial(woodColor);
        Material fabricMaterial = CreateMaterial(fabricColor);

        // Add furniture based on selections
        if (addTable)
        {
            CreateTable(interiorObj, woodMaterial);
        }

        if (addChairs)
        {
            CreateChairs(interiorObj, woodMaterial, fabricMaterial);
        }

        if (addBed)
        {
            CreateBed(interiorObj, woodMaterial, fabricMaterial);
        }

        if (addChest)
        {
            CreateChest(interiorObj, woodMaterial);
        }

        if (addLighting)
        {
            CreateLighting(interiorObj);
        }

        // Create interior prefab
        string prefabPath = "Assets/Prefabs/Buildings/Interiors/BasicInterior.prefab";
        EnsureDirectoryExists("Assets/Prefabs/Buildings/Interiors");

        PrefabUtility.SaveAsPrefabAsset(interiorObj, prefabPath);
        DestroyImmediate(interiorObj);

        Debug.Log($"Interior created and saved as prefab: {prefabPath}");
        EditorUtility.DisplayDialog("Success!", "Interior created successfully!\n\nPrefab saved to: Assets/Prefabs/Buildings/Interiors/BasicInterior.prefab", "OK");
    }

    private void CreateTable(GameObject parent, Material material)
    {
        GameObject table = new GameObject("Table");

        // Table top
        GameObject top = GameObject.CreatePrimitive(PrimitiveType.Cube);
        top.name = "TableTop";
        top.transform.parent = table.transform;
        top.transform.localPosition = new Vector3(0, 0.7f, 0);
        top.transform.localScale = new Vector3(2f, 0.1f, 1f);
        top.GetComponent<Renderer>().material = material;

        // Table legs
        for (int i = 0; i < 4; i++)
        {
            GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            leg.name = $"TableLeg{i}";
            leg.transform.parent = table.transform;
            leg.transform.localScale = new Vector3(0.05f, 0.7f, 0.05f);

            float x = (i % 2 == 0) ? 0.9f : -0.9f;
            float z = (i / 2 == 0) ? 0.4f : -0.4f;
            leg.transform.localPosition = new Vector3(x, 0.35f, z);
            leg.GetComponent<Renderer>().material = material;
        }

        table.transform.parent = parent.transform;
        table.transform.localPosition = new Vector3(0, 0, 0);
    }

    private void CreateChairs(GameObject parent, Material woodMaterial, Material fabricMaterial)
    {
        // Create 4 chairs around the table
        Vector3[] chairPositions = {
            new Vector3(0, 0, 1.5f),
            new Vector3(2.5f, 0, 0),
            new Vector3(0, 0, -1.5f),
            new Vector3(-2.5f, 0, 0)
        };

        Vector3[] chairRotations = {
            new Vector3(0, 0, 0),
            new Vector3(0, 90, 0),
            new Vector3(0, 180, 0),
            new Vector3(0, 270, 0)
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject chair = CreateChair($"Chair{i}", woodMaterial, fabricMaterial);
            chair.transform.parent = parent.transform;
            chair.transform.localPosition = chairPositions[i];
            chair.transform.localEulerAngles = chairRotations[i];
        }
    }

    private GameObject CreateChair(string name, Material woodMaterial, Material fabricMaterial)
    {
        GameObject chair = new GameObject(name);

        // Seat
        GameObject seat = GameObject.CreatePrimitive(PrimitiveType.Cube);
        seat.name = "Seat";
        seat.transform.parent = chair.transform;
        seat.transform.localPosition = new Vector3(0, 0.4f, 0);
        seat.transform.localScale = new Vector3(0.6f, 0.1f, 0.6f);
        seat.GetComponent<Renderer>().material = fabricMaterial;

        // Backrest
        GameObject back = GameObject.CreatePrimitive(PrimitiveType.Cube);
        back.name = "Backrest";
        back.transform.parent = chair.transform;
        back.transform.localPosition = new Vector3(0, 0.8f, -0.25f);
        back.transform.localScale = new Vector3(0.6f, 0.8f, 0.1f);
        back.GetComponent<Renderer>().material = woodMaterial;

        // Legs
        for (int i = 0; i < 4; i++)
        {
            GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            leg.name = $"ChairLeg{i}";
            leg.transform.parent = chair.transform;
            leg.transform.localScale = new Vector3(0.03f, 0.4f, 0.03f);

            float x = (i % 2 == 0) ? 0.25f : -0.25f;
            float z = (i / 2 == 0) ? 0.25f : -0.25f;
            leg.transform.localPosition = new Vector3(x, 0.2f, z);
            leg.GetComponent<Renderer>().material = woodMaterial;
        }

        return chair;
    }

    private void CreateBed(GameObject parent, Material woodMaterial, Material fabricMaterial)
    {
        GameObject bed = new GameObject("Bed");

        // Bed frame
        GameObject frame = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frame.name = "BedFrame";
        frame.transform.parent = bed.transform;
        frame.transform.localPosition = new Vector3(0, 0.3f, 0);
        frame.transform.localScale = new Vector3(2f, 0.1f, 3f);
        frame.GetComponent<Renderer>().material = woodMaterial;

        // Mattress
        GameObject mattress = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mattress.name = "Mattress";
        mattress.transform.parent = bed.transform;
        mattress.transform.localPosition = new Vector3(0, 0.4f, 0);
        mattress.transform.localScale = new Vector3(1.8f, 0.2f, 2.8f);
        mattress.GetComponent<Renderer>().material = fabricMaterial;

        // Headboard
        GameObject headboard = GameObject.CreatePrimitive(PrimitiveType.Cube);
        headboard.name = "Headboard";
        headboard.transform.parent = bed.transform;
        headboard.transform.localPosition = new Vector3(0, 0.8f, -1.4f);
        headboard.transform.localScale = new Vector3(2f, 1.2f, 0.1f);
        headboard.GetComponent<Renderer>().material = woodMaterial;

        bed.transform.parent = parent.transform;
        bed.transform.localPosition = new Vector3(-2f, 0, -2f);
    }

    private void CreateChest(GameObject parent, Material material)
    {
        GameObject chest = GameObject.CreatePrimitive(PrimitiveType.Cube);
        chest.name = "Chest";
        chest.transform.parent = parent.transform;
        chest.transform.localPosition = new Vector3(2f, 0.4f, -2f);
        chest.transform.localScale = new Vector3(0.8f, 0.8f, 0.6f);
        chest.GetComponent<Renderer>().material = material;
    }

    private void CreateLighting(GameObject parent)
    {
        // Create a point light for interior lighting
        GameObject lightObj = new GameObject("InteriorLight");
        lightObj.transform.parent = parent.transform;
        lightObj.transform.localPosition = new Vector3(0, roomHeight - 0.5f, 0);

        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.95f, 0.8f);
        light.intensity = 1.5f;
        light.range = Mathf.Max(roomWidth, roomDepth) * 1.5f;
        light.shadows = LightShadows.Soft;
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
