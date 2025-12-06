using UnityEngine;
using UnityEditor;

/// <summary>
/// Quick material creator for forest environment - no textures needed!
/// Creates simple but nice-looking materials for ground, trees, grass, rocks.
/// </summary>
public class QuickForestMaterials : EditorWindow
{
    // REMOVED: Use Quick: Apply Forest Ground NOW! instead
    // [MenuItem("Tools/Create Forest Materials")]
    // public static void ShowWindow()
    // {
    //     GetWindow<QuickForestMaterials>("Forest Materials");
    // }
    
    private void OnGUI()
    {
        GUILayout.Label("Quick Forest Materials Creator", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox("Creates simple colored materials for your forest. No textures required!", MessageType.Info);
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Create All Forest Materials", GUILayout.Height(40)))
        {
            CreateAllMaterials();
        }
        
        EditorGUILayout.Space();
        GUILayout.Label("Or create individually:", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Ground Material (Grass)"))
        {
            CreateGroundMaterial();
        }
        
        if (GUILayout.Button("Tree Trunk Material"))
        {
            CreateTreeTrunkMaterial();
        }
        
        if (GUILayout.Button("Tree Leaves Material"))
        {
            CreateLeavesMaterial();
        }
        
        if (GUILayout.Button("Rock Material"))
        {
            CreateRockMaterial();
        }
        
        if (GUILayout.Button("Bush Material"))
        {
            CreateBushMaterial();
        }
    }
    
    private void CreateAllMaterials()
    {
        CreateGroundMaterial();
        CreateTreeTrunkMaterial();
        CreateLeavesMaterial();
        CreateRockMaterial();
        CreateBushMaterial();
        
        // Also create terrain texture and layer
        CreateForestTerrainLayer();
        
        EditorUtility.DisplayDialog("Success!", 
            "Created 5 forest materials + terrain layer in Assets/Materials/Forest/\n\n" +
            "✓ Ground material (grass green)\n" +
            "✓ TreeTrunk material (brown)\n" +
            "✓ TreeLeaves material (forest green)\n" +
            "✓ Rock material (gray)\n" +
            "✓ Bush material (dark green)\n" +
            "✓ Forest terrain layer + texture", 
            "OK");
    }
    
    private void CreateGroundMaterial()
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.name = "ForestGround";
        mat.color = new Color(0.4f, 0.6f, 0.3f); // Grass green
        mat.SetFloat("_Metallic", 0f);
        mat.SetFloat("_Glossiness", 0.2f);
        
        SaveMaterial(mat, "ForestGround");
        Debug.Log("Created Forest Ground material (grass green)");
    }
    
    private void CreateTreeTrunkMaterial()
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.name = "TreeTrunk";
        mat.color = new Color(0.4f, 0.3f, 0.2f); // Brown bark
        mat.SetFloat("_Metallic", 0f);
        mat.SetFloat("_Glossiness", 0.1f);
        
        SaveMaterial(mat, "TreeTrunk");
        Debug.Log("Created Tree Trunk material (brown)");
    }
    
    private void CreateLeavesMaterial()
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.name = "TreeLeaves";
        mat.color = new Color(0.2f, 0.5f, 0.2f); // Forest green
        mat.SetFloat("_Metallic", 0f);
        mat.SetFloat("_Glossiness", 0.3f);
        
        SaveMaterial(mat, "TreeLeaves");
        Debug.Log("Created Tree Leaves material (forest green)");
    }
    
    private void CreateRockMaterial()
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.name = "Rock";
        mat.color = new Color(0.5f, 0.5f, 0.5f); // Gray stone
        mat.SetFloat("_Metallic", 0f);
        mat.SetFloat("_Glossiness", 0.15f);
        
        SaveMaterial(mat, "Rock");
        Debug.Log("Created Rock material (gray)");
    }
    
    private void CreateBushMaterial()
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.name = "Bush";
        mat.color = new Color(0.15f, 0.4f, 0.15f); // Dark green
        mat.SetFloat("_Metallic", 0f);
        mat.SetFloat("_Glossiness", 0.25f);
        
        SaveMaterial(mat, "Bush");
        Debug.Log("Created Bush material (dark green)");
    }
    
    private void SaveMaterial(Material mat, string name)
    {
        string folderPath = "Assets/Materials/Forest";
        
        // Create folders if they don't exist
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Materials", "Forest");
        }
        
        string path = $"{folderPath}/{name}.mat";
        
        // Delete existing if present
        if (AssetDatabase.LoadAssetAtPath<Material>(path) != null)
        {
            AssetDatabase.DeleteAsset(path);
        }
        
        AssetDatabase.CreateAsset(mat, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    private void CreateForestTerrainLayer()
    {
        string folderPath = "Assets/Materials/Forest";
        
        // Create folders if they don't exist
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Materials", "Forest");
        }
        
        // Create forest ground texture
        Texture2D forestTexture = CreateForestGroundTexture();
        
        // Save texture
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
        
        // Create terrain layer
        TerrainLayer layer = new TerrainLayer();
        layer.diffuseTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        layer.tileSize = new Vector2(15, 15);
        layer.metallic = 0;
        layer.smoothness = 0.2f;
        
        string layerPath = $"{folderPath}/ForestGroundLayer.terrainlayer";
        
        // Delete existing if present
        if (AssetDatabase.LoadAssetAtPath<TerrainLayer>(layerPath) != null)
        {
            AssetDatabase.DeleteAsset(layerPath);
        }
        
        AssetDatabase.CreateAsset(layer, layerPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("Created forest terrain layer and texture");
    }
    
    private Texture2D CreateForestGroundTexture()
    {
        // Create a procedural forest ground texture
        int size = 256;
        Texture2D texture = new Texture2D(size, size);
        texture.name = "ForestGroundTexture";
        
        // Forest ground colors
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
                    pixelColor = Color.Lerp(grassGreen, darkGreen, 0.3f);
                }
                else if (combined < 0.6f)
                {
                    pixelColor = Color.Lerp(brownDirt, grassGreen, 0.4f);
                }
                else
                {
                    pixelColor = Color.Lerp(darkGreen, grassGreen, combined - 0.5f);
                }
                
                // Add random variation
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

