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
    private bool useHillyTerrain = false; // DEFAULT: FLAT GROUND
    
    [Header("Forest Density")]
    private int treeCount = 200;
    private int grassDensity = 50;
    
    [Header("Lighting")]
    private Color ambientLight = new Color(0.4f, 0.5f, 0.6f);
    private Color sunColor = new Color(1f, 0.95f, 0.8f);
    private float sunIntensity = 1.2f;
    
    // REMOVED: Use Quick: Apply Forest Ground NOW! instead
    // [MenuItem("Tools/Create Forest Environment")]
    // public static void ShowWindow()
    // {
    //     GetWindow<ForestEnvironmentCreator>("Forest Creator");
    // }
    
    private void OnGUI()
    {
        GUILayout.Label("Forest Environment Creator", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        GUILayout.Label("Terrain Settings", EditorStyles.boldLabel);
        terrainWidth = EditorGUILayout.IntSlider("Terrain Width", terrainWidth, 100, 1000);
        terrainLength = EditorGUILayout.IntSlider("Terrain Length", terrainLength, 100, 1000);
        terrainHeight = EditorGUILayout.IntSlider("Terrain Height", terrainHeight, 50, 300);
        useHillyTerrain = EditorGUILayout.Toggle("Use Hilly Terrain", useHillyTerrain);
        
        if (!useHillyTerrain)
        {
            EditorGUILayout.HelpBox("DEFAULT: Flat ground enabled! Perfect for testing.", MessageType.Info);
        }
        
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
        EditorGUILayout.HelpBox("This will create a terrain with trees, grass, and proper lighting.\n\nDEFAULT: Flat ground (perfect for testing!)\nOptional: Enable 'Use Hilly Terrain' for procedural hills.", MessageType.Info);
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
        
        // Generate terrain shape (flat or hilly based on settings)
        if (useHillyTerrain)
        {
            GenerateHills(terrain);
        }
        else
        {
            GenerateFlatTerrain(terrain);
        }
        
        // Apply forest ground texture to terrain (IMPORTANT: Must come before trees/grass)
        ApplyForestGroundTexture(terrain);
        
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
    
    private void GenerateFlatTerrain(Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        float[,] heights = new float[width, height];
        
        // Create completely flat terrain at base height (10m)
        float flatHeight = 10f / terrainData.size.y; // Normalized height
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = flatHeight;
            }
        }
        
        terrainData.SetHeights(0, 0, heights);
        Debug.Log("Generated FLAT terrain (default) - perfect for testing!");
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
    
    private void ApplyForestGroundTexture(Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        
        // Create forest ground texture (grass green with some variation)
        Texture2D forestTexture = CreateForestGroundTexture();
        
        // Save the texture
        string folderPath = "Assets/Materials/Forest";
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Materials", "Forest");
        }
        
        string texturePath = $"{folderPath}/ForestGroundTexture.png";
        byte[] bytes = forestTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(texturePath, bytes);
        AssetDatabase.Refresh();
        
        // Setup texture import settings
        TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        if (importer != null)
        {
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.SaveAndReimport();
        }
        
        // Create terrain layer with forest ground texture
        TerrainLayer forestLayer = new TerrainLayer();
        forestLayer.diffuseTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        forestLayer.tileSize = new Vector2(15, 15);
        forestLayer.metallic = 0;
        forestLayer.smoothness = 0.2f;
        
        // Save terrain layer
        string layerPath = $"{folderPath}/ForestGroundLayer.terrainlayer";
        if (AssetDatabase.LoadAssetAtPath<TerrainLayer>(layerPath) != null)
        {
            AssetDatabase.DeleteAsset(layerPath);
        }
        
        AssetDatabase.CreateAsset(forestLayer, layerPath);
        AssetDatabase.SaveAssets();
        
        // Apply to terrain
        terrainData.terrainLayers = new TerrainLayer[] { forestLayer };
        
        Debug.Log("Applied forest ground texture to terrain");
    }
    
    private Texture2D CreateForestGroundTexture()
    {
        // Create a procedural forest ground texture with grass-like appearance
        int size = 256;
        Texture2D texture = new Texture2D(size, size);
        texture.name = "ForestGroundTexture";
        
        // Forest ground colors - various shades of green and brown
        Color grassGreen = new Color(0.4f, 0.6f, 0.3f);
        Color darkGreen = new Color(0.3f, 0.5f, 0.25f);
        Color brownDirt = new Color(0.4f, 0.35f, 0.25f);
        
        // Generate forest ground pattern
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // Multiple layers of noise for natural look
                float noise1 = Mathf.PerlinNoise(x * 0.05f, y * 0.05f);
                float noise2 = Mathf.PerlinNoise(x * 0.1f, y * 0.1f) * 0.5f;
                float noise3 = Mathf.PerlinNoise(x * 0.2f, y * 0.2f) * 0.25f;
                
                float combined = noise1 + noise2 + noise3;
                
                // Blend between grass and dirt based on noise
                Color pixelColor;
                if (combined > 1.2f)
                {
                    // Lighter grass areas
                    pixelColor = Color.Lerp(grassGreen, darkGreen, 0.3f);
                }
                else if (combined < 0.6f)
                {
                    // Dirt patches
                    pixelColor = Color.Lerp(brownDirt, grassGreen, 0.4f);
                }
                else
                {
                    // Main grass color
                    pixelColor = Color.Lerp(darkGreen, grassGreen, combined - 0.5f);
                }
                
                // Add some random variation for texture
                float randomVariation = Random.Range(-0.05f, 0.05f);
                pixelColor = new Color(
                    Mathf.Clamp01(pixelColor.r + randomVariation),
                    Mathf.Clamp01(pixelColor.g + randomVariation),
                    Mathf.Clamp01(pixelColor.b + randomVariation)
                );
                
                texture.SetPixel(x, y, pixelColor);
            }
        }
        
        texture.Apply();
        return texture;
    }
}

/// <summary>
/// Quick menu item for instant forest ground application
/// </summary>
public class QuickForest
{
    [MenuItem("Tools/Quick: Apply Forest Ground NOW! %#F")] // Ctrl+Shift+F
    public static void QuickApplyForestGround()
    {
        Terrain terrain = Object.FindObjectOfType<Terrain>();
        if (terrain == null)
        {
            Debug.LogError("No terrain found!");
            return;
        }
        
        // Create forest ground texture
        Texture2D forestTexture = CreateQuickForestTexture();
        
        // Save texture
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            AssetDatabase.CreateFolder("Assets", "Materials");
        if (!AssetDatabase.IsValidFolder("Assets/Materials/Forest"))
            AssetDatabase.CreateFolder("Assets/Materials", "Forest");
        
        string texPath = "Assets/Materials/Forest/QuickForestGround.png";
        System.IO.File.WriteAllBytes(texPath, forestTexture.EncodeToPNG());
        AssetDatabase.Refresh();
        
        // Setup texture import
        TextureImporter importer = AssetImporter.GetAtPath(texPath) as TextureImporter;
        if (importer != null)
        {
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.SaveAndReimport();
        }
        
        // Create terrain layer
        TerrainLayer layer = new TerrainLayer();
        layer.diffuseTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
        layer.tileSize = new Vector2(15, 15);
        layer.metallic = 0;
        layer.smoothness = 0.2f;
        
        string layerPath = "Assets/Materials/Forest/QuickForestGroundLayer.terrainlayer";
        if (AssetDatabase.LoadAssetAtPath<TerrainLayer>(layerPath) != null)
        {
            AssetDatabase.DeleteAsset(layerPath);
        }
        AssetDatabase.CreateAsset(layer, layerPath);
        AssetDatabase.SaveAssets();
        
        // Apply to terrain
        TerrainData terrainData = terrain.terrainData;
        terrainData.terrainLayers = new TerrainLayer[] { layer };
        
        // Setup forest-like lighting
        Light sun = Object.FindObjectOfType<Light>();
        if (sun != null && sun.type == LightType.Directional)
        {
            sun.color = new Color(1f, 0.95f, 0.8f);
            sun.intensity = 1.2f;
        }
        
        RenderSettings.ambientLight = new Color(0.4f, 0.5f, 0.6f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.7f, 0.8f, 0.85f);
        RenderSettings.fogDensity = 0.005f;
        
        Debug.Log("<color=green>✓ Forest ground applied instantly!</color>");
    }
    
    private static Texture2D CreateQuickForestTexture()
    {
        int size = 256;
        Texture2D texture = new Texture2D(size, size);
        
        Color grassGreen = new Color(0.4f, 0.6f, 0.3f);
        Color darkGreen = new Color(0.3f, 0.5f, 0.25f);
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
                Color pixelColor = Color.Lerp(darkGreen, grassGreen, noise);
                texture.SetPixel(x, y, pixelColor);
            }
        }
        
        texture.Apply();
        return texture;
    }
}

