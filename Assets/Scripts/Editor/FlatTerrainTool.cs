using UnityEngine;
using UnityEditor;

/// <summary>
/// Quick tool to make terrain completely flat and position player correctly.
/// Simple one-click solution for flat ground setup!
/// </summary>
public class FlatTerrainTool : EditorWindow
{
    private float terrainHeight = 10f;
    private float playerHeightOffset = 2f;
    
    [MenuItem("Tools/Make Terrain FLAT! ⚡")]
    public static void ShowWindow()
    {
        GetWindow<FlatTerrainTool>("Flat Terrain");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Flat Terrain Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox("Makes your terrain completely flat - perfect for testing!", MessageType.Info);
        EditorGUILayout.Space();
        
        terrainHeight = EditorGUILayout.Slider("Terrain Base Height", terrainHeight, 0f, 50f);
        playerHeightOffset = EditorGUILayout.Slider("Player Height Above Terrain", playerHeightOffset, 0.5f, 10f);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("FLATTEN TERRAIN NOW!", GUILayout.Height(50)))
        {
            FlattenTerrainNow();
        }
        
        EditorGUILayout.Space();
        
        GUILayout.Label("Quick Actions:", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Just Position Player on Ground", GUILayout.Height(30)))
        {
            PositionPlayerOnly();
        }
    }
    
    private void FlattenTerrainNow()
    {
        Terrain terrain = Object.FindObjectOfType<Terrain>();
        if (terrain == null)
        {
            EditorUtility.DisplayDialog("Error", "No terrain found in scene!\n\nCreate a terrain first (GameObject → 3D Object → Terrain)", "OK");
            return;
        }
        
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        
        // Create completely flat terrain
        float[,] heights = new float[width, height];
        float normalizedHeight = terrainHeight / terrainData.size.y;
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = normalizedHeight;
            }
        }
        
        Undo.RegisterCompleteObjectUndo(terrainData, "Flatten Terrain");
        terrainData.SetHeights(0, 0, heights);
        
        Debug.Log($"<color=green>✓ Terrain flattened to height: {terrainHeight}</color>");
        
        // Position player
        PositionPlayerOnFlatTerrain(terrain);
        
        EditorUtility.DisplayDialog("Success!", 
            $"Terrain is now completely FLAT!\n\n" +
            $"• Terrain Height: {terrainHeight}m\n" +
            $"• Player positioned at Y: {terrainHeight + playerHeightOffset}m\n\n" +
            "Ready for testing!", 
            "Awesome!");
    }
    
    private void PositionPlayerOnly()
    {
        Terrain terrain = Object.FindObjectOfType<Terrain>();
        if (terrain == null)
        {
            EditorUtility.DisplayDialog("Error", "No terrain found!", "OK");
            return;
        }
        
        PositionPlayerOnFlatTerrain(terrain);
    }
    
    private void PositionPlayerOnFlatTerrain(Terrain terrain)
    {
        // Find player
        GameObject player = GameObject.Find("Player_Mage");
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        if (player == null)
        {
            Debug.LogWarning("Player not found - couldn't position on terrain. Make sure player is named 'Player_Mage' or has 'Player' tag.");
            return;
        }
        
        // Position at center of terrain
        Vector3 centerPos = new Vector3(
            terrain.terrainData.size.x / 2,
            0,
            terrain.terrainData.size.z / 2
        );
        
        float terrainHeightAtCenter = terrain.SampleHeight(centerPos);
        centerPos.y = terrainHeightAtCenter + playerHeightOffset;
        
        Undo.RecordObject(player.transform, "Position Player on Flat Terrain");
        player.transform.position = centerPos;
        
        // Select player
        Selection.activeGameObject = player;
        if (SceneView.lastActiveSceneView != null)
        {
            SceneView.lastActiveSceneView.FrameSelected();
        }
        
        Debug.Log($"<color=green>✓ Player positioned at {centerPos}</color>");
    }
}

/// <summary>
/// Super quick menu item to instantly flatten terrain and position player
/// </summary>
public class QuickFlatTerrain
{
    [MenuItem("Tools/QUICK: Flatten Terrain + Position Player ⚡ %#T")] // Ctrl+Shift+T
    public static void QuickFlattenAndPosition()
    {
        Terrain terrain = Object.FindObjectOfType<Terrain>();
        if (terrain == null)
        {
            Debug.LogError("No terrain found!");
            return;
        }
        
        // Flatten terrain completely
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        
        float[,] heights = new float[width, height];
        float flatHeight = 10f / terrainData.size.y; // 10m normalized
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = flatHeight;
            }
        }
        
        Undo.RegisterCompleteObjectUndo(terrainData, "Quick Flatten Terrain");
        terrainData.SetHeights(0, 0, heights);
        
        // Position player
        GameObject player = GameObject.Find("Player_Mage");
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        if (player != null)
        {
            Vector3 centerPos = new Vector3(
                terrain.terrainData.size.x / 2,
                0,
                terrain.terrainData.size.z / 2
            );
            
            float terrainHeight = terrain.SampleHeight(centerPos);
            centerPos.y = terrainHeight + 2f;
            
            Undo.RecordObject(player.transform, "Quick Position Player");
            player.transform.position = centerPos;
            
            Selection.activeGameObject = player;
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.FrameSelected();
            }
        }
        
        Debug.Log("<color=green>✓✓✓ TERRAIN FLATTENED + PLAYER POSITIONED! ✓✓✓</color>");
    }
}

