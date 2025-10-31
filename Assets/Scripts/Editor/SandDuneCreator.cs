using UnityEngine;
using UnityEditor;

/// <summary>
/// Transform terrain into sand dunes with desert environment.
/// Creates beautiful sandy landscape with proper lighting and atmosphere.
/// </summary>
public class SandDuneCreator : EditorWindow
{
    [Header("Sand Colors")]
    private Color sandColorLight = new Color(0.96f, 0.87f, 0.70f); // Light tan sand
    private Color sandColorDark = new Color(0.85f, 0.75f, 0.55f); // Darker sand for shadows
    
    [Header("Environment")]
    private Color skyColor = new Color(0.53f, 0.81f, 0.98f); // Clear blue sky
    private Color sunColor = new Color(1f, 0.95f, 0.8f); // Warm sun
    private float sunIntensity = 1.5f;
    private bool enableHeatHaze = true;
    
    [MenuItem("Tools/Create Sand Dune Environment")]
    public static void ShowWindow()
    {
        GetWindow<SandDuneCreator>("Sand Dunes");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Sand Dune Creator", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox("Transform your terrain into a desert sand dune environment!", MessageType.Info);
        EditorGUILayout.Space();
        
        GUILayout.Label("Sand Colors", EditorStyles.boldLabel);
        sandColorLight = EditorGUILayout.ColorField("Light Sand", sandColorLight);
        sandColorDark = EditorGUILayout.ColorField("Dark Sand (Shadows)", sandColorDark);
        
        EditorGUILayout.Space();
        GUILayout.Label("Desert Atmosphere", EditorStyles.boldLabel);
        skyColor = EditorGUILayout.ColorField("Sky Color", skyColor);
        sunColor = EditorGUILayout.ColorField("Sun Color", sunColor);
        sunIntensity = EditorGUILayout.Slider("Sun Intensity", sunIntensity, 1f, 2f);
        enableHeatHaze = EditorGUILayout.Toggle("Heat Haze Effect", enableHeatHaze);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Transform to Sand Dunes", GUILayout.Height(40)))
        {
            CreateSandDunes();
        }
        
        EditorGUILayout.Space();
        
        GUILayout.Label("Quick Presets:", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Sahara Desert"))
        {
            ApplySaharaPreset();
        }
        if (GUILayout.Button("White Sands"))
        {
            ApplyWhiteSandsPreset();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Red Desert"))
        {
            ApplyRedDesertPreset();
        }
        if (GUILayout.Button("Golden Dunes"))
        {
            ApplyGoldenDunesPreset();
        }
        EditorGUILayout.EndHorizontal();
    }
    
    private void CreateSandDunes()
    {
        Terrain terrain = FindObjectOfType<Terrain>();
        if (terrain == null)
        {
            EditorUtility.DisplayDialog("Error", "No terrain found in scene!\n\nPlease create a terrain first.", "OK");
            return;
        }
        
        Debug.Log("Creating sand dune environment...");
        
        // Create sand material
        CreateSandMaterial(terrain);
        
        // Remove trees and grass
        ClearVegetation(terrain);
        
        // Setup desert lighting
        SetupDesertLighting();
        
        // Setup desert atmosphere
        SetupDesertAtmosphere();
        
        // Apply sand texture to terrain
        ApplySandTexture(terrain);
        
        Debug.Log("Sand dune environment created!");
        EditorUtility.DisplayDialog("Success!", 
            "Sand dune environment created!\n\n" +
            "Your terrain now has:\n" +
            "✓ Sandy color and texture\n" +
            "✓ Desert lighting\n" +
            "✓ Clear desert sky\n" +
            "✓ Warm sun\n\n" +
            "Press Play to explore your desert!", 
            "OK");
    }
    
    private void CreateSandMaterial(Terrain terrain)
    {
        // Create a sand material
        Material sandMat = new Material(Shader.Find("Standard"));
        sandMat.name = "SandMaterial";
        sandMat.color = sandColorLight;
        sandMat.SetFloat("_Metallic", 0f);
        sandMat.SetFloat("_Glossiness", 0.15f); // Slight shine for sand
        
        // Save material
        string folderPath = "Assets/Materials/Desert";
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Materials", "Desert");
        }
        
        string matPath = $"{folderPath}/Sand.mat";
        if (AssetDatabase.LoadAssetAtPath<Material>(matPath) != null)
        {
            AssetDatabase.DeleteAsset(matPath);
        }
        
        AssetDatabase.CreateAsset(sandMat, matPath);
        AssetDatabase.SaveAssets();
        
        Debug.Log("Created sand material");
    }
    
    private void ApplySandTexture(Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        
        // Create terrain layer with sand color
        TerrainLayer sandLayer = new TerrainLayer();
        sandLayer.diffuseTexture = CreateSandTexture();
        sandLayer.tileSize = new Vector2(15, 15);
        sandLayer.metallic = 0;
        sandLayer.smoothness = 0.15f;
        
        // Save terrain layer
        string layerPath = "Assets/Materials/Desert/SandLayer.terrainlayer";
        if (AssetDatabase.LoadAssetAtPath<TerrainLayer>(layerPath) != null)
        {
            AssetDatabase.DeleteAsset(layerPath);
        }
        
        AssetDatabase.CreateAsset(sandLayer, layerPath);
        AssetDatabase.SaveAssets();
        
        // Apply to terrain
        terrainData.terrainLayers = new TerrainLayer[] { sandLayer };
        
        Debug.Log("Applied sand texture to terrain");
    }
    
    private Texture2D CreateSandTexture()
    {
        // Create a simple procedural sand texture
        int size = 256;
        Texture2D sandTexture = new Texture2D(size, size);
        sandTexture.name = "SandTexture";
        
        // Generate sand-like pattern
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // Add some noise for sand grain effect
                float noise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f) * 0.1f;
                Color sandColor = Color.Lerp(sandColorDark, sandColorLight, 0.5f + noise);
                sandTexture.SetPixel(x, y, sandColor);
            }
        }
        
        sandTexture.Apply();
        
        // Save texture
        string texturePath = "Assets/Materials/Desert/SandTexture.png";
        byte[] bytes = sandTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(texturePath, bytes);
        AssetDatabase.Refresh();
        
        // Reload and return
        TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        if (importer != null)
        {
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.SaveAndReimport();
        }
        
        return AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
    }
    
    private void ClearVegetation(Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        
        // Remove all trees
        terrainData.treeInstances = new TreeInstance[0];
        
        // Remove all details (grass, etc.)
        terrainData.detailPrototypes = new DetailPrototype[0];
        
        Debug.Log("Cleared all vegetation (trees and grass)");
    }
    
    private void SetupDesertLighting()
    {
        // Setup strong sunlight
        Light sun = FindObjectOfType<Light>();
        if (sun == null || sun.type != LightType.Directional)
        {
            GameObject sunObj = new GameObject("Desert Sun");
            sun = sunObj.AddComponent<Light>();
            sun.type = LightType.Directional;
        }
        
        Undo.RecordObject(sun, "Setup Desert Sun");
        sun.color = sunColor;
        sun.intensity = sunIntensity;
        sun.transform.rotation = Quaternion.Euler(45, -30, 0); // High sun angle
        sun.shadows = LightShadows.Soft;
        
        // Ambient lighting (bright for desert)
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.7f, 0.75f, 0.8f); // Bright ambient
        
        Debug.Log("Setup desert lighting");
    }
    
    private void SetupDesertAtmosphere()
    {
        // Clear sky with slight heat haze
        RenderSettings.fog = enableHeatHaze;
        if (enableHeatHaze)
        {
            RenderSettings.fogColor = skyColor;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.002f; // Very light fog for heat haze
        }
        
        // Setup skybox
        RenderSettings.skybox = null; // Use solid color
        RenderSettings.ambientSkyColor = skyColor;
        
        Debug.Log("Setup desert atmosphere");
    }
    
    // Preset methods
    private void ApplySaharaPreset()
    {
        sandColorLight = new Color(0.96f, 0.87f, 0.70f); // Classic tan
        sandColorDark = new Color(0.85f, 0.75f, 0.55f);
        skyColor = new Color(0.53f, 0.81f, 0.98f);
        sunColor = new Color(1f, 0.95f, 0.8f);
        sunIntensity = 1.5f;
        enableHeatHaze = true;
        Repaint();
    }
    
    private void ApplyWhiteSandsPreset()
    {
        sandColorLight = new Color(0.98f, 0.98f, 0.95f); // Almost white
        sandColorDark = new Color(0.90f, 0.90f, 0.88f);
        skyColor = new Color(0.4f, 0.7f, 0.95f); // Deeper blue
        sunColor = new Color(1f, 1f, 0.95f);
        sunIntensity = 1.8f;
        enableHeatHaze = true;
        Repaint();
    }
    
    private void ApplyRedDesertPreset()
    {
        sandColorLight = new Color(0.85f, 0.55f, 0.40f); // Reddish
        sandColorDark = new Color(0.70f, 0.45f, 0.30f);
        skyColor = new Color(0.6f, 0.75f, 0.90f);
        sunColor = new Color(1f, 0.90f, 0.75f);
        sunIntensity = 1.6f;
        enableHeatHaze = true;
        Repaint();
    }
    
    private void ApplyGoldenDunesPreset()
    {
        sandColorLight = new Color(1f, 0.85f, 0.50f); // Golden
        sandColorDark = new Color(0.90f, 0.70f, 0.40f);
        skyColor = new Color(0.95f, 0.80f, 0.60f); // Sunset sky
        sunColor = new Color(1f, 0.85f, 0.60f);
        sunIntensity = 1.4f;
        enableHeatHaze = true;
        Repaint();
    }
}

/// <summary>
/// Quick menu item for instant sand dune creation
/// </summary>
public class QuickSandDunes
{
    [MenuItem("Tools/Quick: Make Sand Dunes NOW! %#S")] // Ctrl+Shift+S
    public static void QuickCreateSandDunes()
    {
        Terrain terrain = Object.FindObjectOfType<Terrain>();
        if (terrain == null)
        {
            Debug.LogError("No terrain found!");
            return;
        }
        
        // Quick sand dune setup
        TerrainData terrainData = terrain.terrainData;
        
        // Clear vegetation
        terrainData.treeInstances = new TreeInstance[0];
        terrainData.detailPrototypes = new DetailPrototype[0];
        
        // Create and apply sand material
        Color sandColor = new Color(0.96f, 0.87f, 0.70f);
        
        // Create simple texture
        Texture2D sandTexture = new Texture2D(256, 256);
        for (int y = 0; y < 256; y++)
        {
            for (int x = 0; x < 256; x++)
            {
                float noise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f) * 0.1f;
                sandTexture.SetPixel(x, y, sandColor * (0.9f + noise));
            }
        }
        sandTexture.Apply();
        
        // Save texture
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            AssetDatabase.CreateFolder("Assets", "Materials");
        if (!AssetDatabase.IsValidFolder("Assets/Materials/Desert"))
            AssetDatabase.CreateFolder("Assets/Materials", "Desert");
        
        string texPath = "Assets/Materials/Desert/QuickSand.png";
        System.IO.File.WriteAllBytes(texPath, sandTexture.EncodeToPNG());
        AssetDatabase.Refresh();
        
        // Create terrain layer
        TerrainLayer layer = new TerrainLayer();
        layer.diffuseTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
        layer.tileSize = new Vector2(15, 15);
        
        string layerPath = "Assets/Materials/Desert/QuickSandLayer.terrainlayer";
        AssetDatabase.CreateAsset(layer, layerPath);
        AssetDatabase.SaveAssets();
        
        terrainData.terrainLayers = new TerrainLayer[] { layer };
        
        // Setup lighting
        Light sun = Object.FindObjectOfType<Light>();
        if (sun != null)
        {
            sun.color = new Color(1f, 0.95f, 0.8f);
            sun.intensity = 1.5f;
        }
        
        RenderSettings.ambientLight = new Color(0.7f, 0.75f, 0.8f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.96f, 0.92f, 0.85f);
        RenderSettings.fogDensity = 0.002f;
        
        Debug.Log("<color=orange>✓ Sand dunes created instantly!</color>");
    }
}

