using UnityEngine;
using UnityEditor;

/// <summary>
/// Quick material creator for forest environment - no textures needed!
/// Creates simple but nice-looking materials for ground, trees, grass, rocks.
/// </summary>
public class QuickForestMaterials : EditorWindow
{
    [MenuItem("Tools/Create Forest Materials")]
    public static void ShowWindow()
    {
        GetWindow<QuickForestMaterials>("Forest Materials");
    }
    
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
        
        EditorUtility.DisplayDialog("Success!", 
            "Created 5 forest materials in Assets/Materials/Forest/\n\n" +
            "- Ground (grass green)\n" +
            "- TreeTrunk (brown)\n" +
            "- TreeLeaves (forest green)\n" +
            "- Rock (gray)\n" +
            "- Bush (dark green)", 
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
}

