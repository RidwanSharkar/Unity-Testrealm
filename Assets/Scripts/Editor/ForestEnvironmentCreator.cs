using UnityEngine;
using UnityEditor;

/// <summary>
/// Quick forest environment creator - generates terrain, trees, grass, and atmosphere.
/// Creates a beautiful forest scene with one click!
/// </summary>
public class ForestEnvironmentCreator : EditorWindow
{
    [Header("Terrain Settings")]
    private int terrainWidth = 500;
    private int terrainLength = 500;
    private int terrainHeight = 100;
    private int heightmapResolution = 513;
    private int detailResolution = 1024;
    
    [Header("Forest Density")]
    private int treeCount = 200;
    private int grassDensity = 50;
    
    [Header("Lighting")]
    private Color ambientLight = new Color(0.4f, 0.5f, 0.6f);
    private Color sunColor = new Color(1f, 0.95f, 0.8f);
    private float sunIntensity = 1.2f;
    
    [MenuItem("Tools/Create Forest Environment")]
    public static void ShowWindow()
    {
        GetWindow<ForestEnvironmentCreator>("Forest Creator");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Forest Environment Creator", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        GUILayout.Label("Terrain Settings", EditorStyles.boldLabel);
        terrainWidth = EditorGUILayout.IntSlider("Terrain Width", terrainWidth, 100, 1000);
        terrainLength = EditorGUILayout.IntSlider("Terrain Length", terrainLength, 100, 1000);
        terrainHeight = EditorGUILayout.IntSlider("Terrain Height", terrainHeight, 50, 300);
        
        EditorGUILayout.Space();
        GUILayout.Label("Forest Density", EditorStyles.boldLabel);
        treeCount = EditorGUILayout.IntSlider("Tree Count", treeCount, 50, 500);
        grassDensity = EditorGUILayout.IntSlider("Grass Density", grassDensity, 10, 100);
        
        EditorGUILayout.Space();
        GUILayout.Label("Lighting", EditorStyles.boldLabel);
        ambientLight = EditorGUILayout.ColorField("Ambient Light", ambientLight);
        sunColor = EditorGUILayout.ColorField("Sun Color", sunColor);
        sunIntensity = EditorGUILayout.Slider("Sun Intensity", sunIntensity, 0.5f, 2f);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Create Forest Environment", GUILayout.Height(40)))
        {
            CreateForestEnvironment();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This will create a terrain with procedural hills, trees, grass, and proper lighting. Click the button above to generate!", MessageType.Info);
    }
    
    private void CreateForestEnvironment()
    {
        // Create terrain
        Debug.Log("Creating forest terrain...");
        TerrainData terrainData = new TerrainData();
        terrainData.heightmapResolution = heightmapResolution;
        terrainData.size = new Vector3(terrainWidth, terrainHeight, terrainLength);
        terrainData.SetDetailResolution(detailResolution, 16);
        
        GameObject terrainObject = Terrain.CreateTerrainGameObject(terrainData);
        terrainObject.name = "Forest Terrain";
        Terrain terrain = terrainObject.GetComponent<Terrain>();
        
        // Generate random hills
        GenerateHills(terrain);
        
        // Setup lighting
        SetupLighting();
        
        // Add trees (using Unity's default tree)
        AddTrees(terrain);
        
        // Add grass
        AddGrass(terrain);
        
        // Setup fog for atmosphere
        SetupFog();
        
        // Position player above terrain
        PositionPlayerOnTerrain(terrain);
        
        Debug.Log("Forest environment created successfully!");
        EditorUtility.DisplayDialog("Success!", "Forest environment created!\n\nTip: Use the Unity Asset Store to download free tree and grass assets for even better visuals!", "OK");
    }
    
    private void GenerateHills(Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        float[,] heights = new float[width, height];
        
        // Generate rolling hills using Perlin noise
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Multiple octaves of noise for natural terrain
                float xCoord = (float)x / width * 3;
                float yCoord = (float)y / height * 3;
                
                float noise1 = Mathf.PerlinNoise(xCoord, yCoord) * 0.3f;
                float noise2 = Mathf.PerlinNoise(xCoord * 2, yCoord * 2) * 0.15f;
                float noise3 = Mathf.PerlinNoise(xCoord * 4, yCoord * 4) * 0.05f;
                
                heights[x, y] = noise1 + noise2 + noise3;
            }
        }
        
        terrainData.SetHeights(0, 0, heights);
        Debug.Log("Generated terrain hills");
    }
    
    private void AddTrees(Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        
        // Create a simple tree prototype (Unity will use default tree)
        TreePrototype[] treePrototypes = new TreePrototype[1];
        treePrototypes[0] = new TreePrototype();
        
        // Try to find a tree prefab in the project
        GameObject treePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tree.prefab");
        if (treePrefab == null)
        {
            // Use Unity's built-in tree if no custom tree found
            Debug.LogWarning("No tree prefab found at Assets/Prefabs/Tree.prefab - Using procedural trees. Download tree assets from Asset Store for better visuals!");
        }
        else
        {
            treePrototypes[0].prefab = treePrefab;
        }
        
        terrainData.treePrototypes = treePrototypes;
        
        // Place trees randomly
        TreeInstance[] trees = new TreeInstance[treeCount];
        for (int i = 0; i < treeCount; i++)
        {
            TreeInstance tree = new TreeInstance();
            tree.prototypeIndex = 0;
            tree.position = new Vector3(Random.Range(0f, 1f), 0, Random.Range(0f, 1f));
            tree.heightScale = Random.Range(0.8f, 1.2f);
            tree.widthScale = Random.Range(0.8f, 1.2f);
            tree.color = Color.white;
            tree.lightmapColor = Color.white;
            
            trees[i] = tree;
        }
        
        terrainData.treeInstances = trees;
        Debug.Log($"Placed {treeCount} trees");
    }
    
    private void AddGrass(Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        
        // Create grass detail layer
        DetailPrototype[] detailPrototypes = new DetailPrototype[1];
        detailPrototypes[0] = new DetailPrototype();
        detailPrototypes[0].renderMode = DetailRenderMode.GrassBillboard;
        detailPrototypes[0].healthyColor = new Color(0.6f, 0.8f, 0.4f);
        detailPrototypes[0].dryColor = new Color(0.7f, 0.7f, 0.5f);
        detailPrototypes[0].minHeight = 0.5f;
        detailPrototypes[0].maxHeight = 1f;
        detailPrototypes[0].minWidth = 0.5f;
        detailPrototypes[0].maxWidth = 1f;
        
        // Try to find grass texture
        Texture2D grassTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/Grass.png");
        if (grassTexture != null)
        {
            detailPrototypes[0].prototypeTexture = grassTexture;
        }
        
        terrainData.detailPrototypes = detailPrototypes;
        
        // Place grass
        int detailMapSize = terrainData.detailResolution;
        int[,] grassMap = new int[detailMapSize, detailMapSize];
        
        for (int x = 0; x < detailMapSize; x++)
        {
            for (int y = 0; y < detailMapSize; y++)
            {
                if (Random.Range(0, 100) < grassDensity)
                {
                    grassMap[x, y] = Random.Range(1, 5);
                }
            }
        }
        
        terrainData.SetDetailLayer(0, 0, 0, grassMap);
        Debug.Log("Added grass to terrain");
    }
    
    private void SetupLighting()
    {
        // Setup ambient lighting
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = ambientLight;
        
        // Find or create directional light
        Light sunLight = FindObjectOfType<Light>();
        if (sunLight == null || sunLight.type != LightType.Directional)
        {
            GameObject lightObj = new GameObject("Directional Light");
            sunLight = lightObj.AddComponent<Light>();
            sunLight.type = LightType.Directional;
        }
        
        sunLight.color = sunColor;
        sunLight.intensity = sunIntensity;
        sunLight.transform.rotation = Quaternion.Euler(50, -30, 0);
        sunLight.shadows = LightShadows.Soft;
        
        Debug.Log("Setup forest lighting");
    }
    
    private void SetupFog()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.7f, 0.8f, 0.85f);
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = 0.005f;
        
        Debug.Log("Setup atmospheric fog");
    }
    
    private void PositionPlayerOnTerrain(Terrain terrain)
    {
        // Find player
        GameObject player = GameObject.Find("Player_Mage");
        if (player == null)
        {
            // Try other common names
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        if (player != null)
        {
            // Position in center of terrain, on surface
            float centerX = terrain.terrainData.size.x / 2;
            float centerZ = terrain.terrainData.size.z / 2;
            float terrainHeight = terrain.SampleHeight(new Vector3(centerX, 0, centerZ));
            
            player.transform.position = new Vector3(centerX, terrainHeight + 2, centerZ);
            Debug.Log("Positioned player on terrain");
        }
        else
        {
            Debug.LogWarning("Player not found - couldn't position on terrain");
        }
    }
}

