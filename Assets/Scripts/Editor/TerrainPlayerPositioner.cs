using UnityEngine;
using UnityEditor;

/// <summary>
/// Quick tool to position player correctly on terrain.
/// Automatically finds terrain height and places player on top.
/// </summary>
public class TerrainPlayerPositioner : EditorWindow
{
    private float heightOffset = 2f;
    private bool positionAtCenter = true;
    private Vector3 customPosition = new Vector3(250, 0, 250);
    
    [MenuItem("Tools/Fix Player Position on Terrain")]
    public static void ShowWindow()
    {
        GetWindow<TerrainPlayerPositioner>("Position Player");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Position Player on Terrain", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox("This will place your player on top of the terrain at the correct height.", MessageType.Info);
        EditorGUILayout.Space();
        
        heightOffset = EditorGUILayout.Slider("Height Above Ground", heightOffset, 0.5f, 10f);
        EditorGUILayout.Space();
        
        positionAtCenter = EditorGUILayout.Toggle("Position at Terrain Center", positionAtCenter);
        
        if (!positionAtCenter)
        {
            EditorGUILayout.LabelField("Custom Position (X, Z):");
            EditorGUILayout.BeginHorizontal();
            customPosition.x = EditorGUILayout.FloatField("X:", customPosition.x);
            customPosition.z = EditorGUILayout.FloatField("Z:", customPosition.z);
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Position Player on Terrain", GUILayout.Height(40)))
        {
            PositionPlayerOnTerrain();
        }
        
        EditorGUILayout.Space();
        
        GUILayout.Label("Quick Fixes:", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Move Player Up (+5)"))
        {
            MovePlayerVertical(5f);
        }
        if (GUILayout.Button("Move Player Down (-5)"))
        {
            MovePlayerVertical(-5f);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Flatten Terrain (Reduce Hills)"))
        {
            FlattenTerrain();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Reset Terrain (Completely Flat)"))
        {
            if (EditorUtility.DisplayDialog("Reset Terrain?", 
                "This will make the terrain completely flat. Continue?", "Yes", "Cancel"))
            {
                ResetTerrainFlat();
            }
        }
    }
    
    private void PositionPlayerOnTerrain()
    {
        // Find player
        GameObject player = GameObject.Find("Player_Mage");
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        if (player == null)
        {
            EditorUtility.DisplayDialog("Error", "Player_Mage not found in scene!\n\nMake sure your player is named 'Player_Mage' or has the 'Player' tag.", "OK");
            return;
        }
        
        // Find terrain
        Terrain terrain = FindObjectOfType<Terrain>();
        if (terrain == null)
        {
            EditorUtility.DisplayDialog("Error", "No terrain found in scene!", "OK");
            return;
        }
        
        // Calculate position
        Vector3 targetPosition;
        
        if (positionAtCenter)
        {
            // Center of terrain
            targetPosition = new Vector3(
                terrain.terrainData.size.x / 2,
                0,
                terrain.terrainData.size.z / 2
            );
        }
        else
        {
            // Custom position
            targetPosition = customPosition;
        }
        
        // Get terrain height at this position
        float terrainHeight = terrain.SampleHeight(targetPosition);
        targetPosition.y = terrainHeight + heightOffset;
        
        // Position player
        Undo.RecordObject(player.transform, "Position Player on Terrain");
        player.transform.position = targetPosition;
        
        // Select player and focus camera on it
        Selection.activeGameObject = player;
        SceneView.lastActiveSceneView.FrameSelected();
        
        Debug.Log($"<color=green>✓ Player positioned at {targetPosition} (Terrain height: {terrainHeight}, Offset: {heightOffset})</color>");
        EditorUtility.DisplayDialog("Success!", 
            $"Player positioned on terrain!\n\nPosition: {targetPosition}\nTerrain Height: {terrainHeight:F2}\n\nThe player is now {heightOffset}m above the ground.", 
            "OK");
    }
    
    private void MovePlayerVertical(float amount)
    {
        GameObject player = GameObject.Find("Player_Mage");
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        if (player == null)
        {
            EditorUtility.DisplayDialog("Error", "Player not found!", "OK");
            return;
        }
        
        Undo.RecordObject(player.transform, "Move Player Vertical");
        Vector3 newPos = player.transform.position;
        newPos.y += amount;
        player.transform.position = newPos;
        
        Debug.Log($"Moved player to Y: {newPos.y}");
    }
    
    private void FlattenTerrain()
    {
        Terrain terrain = FindObjectOfType<Terrain>();
        if (terrain == null)
        {
            EditorUtility.DisplayDialog("Error", "No terrain found!", "OK");
            return;
        }
        
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        
        // Get current heights and flatten by 50%
        float[,] heights = terrainData.GetHeights(0, 0, width, height);
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] *= 0.5f; // Reduce height by 50%
            }
        }
        
        Undo.RegisterCompleteObjectUndo(terrainData, "Flatten Terrain");
        terrainData.SetHeights(0, 0, heights);
        
        Debug.Log("Terrain flattened by 50%");
        EditorUtility.DisplayDialog("Success!", "Terrain hills reduced by 50%!\n\nRun again to flatten more if needed.", "OK");
    }
    
    private void ResetTerrainFlat()
    {
        Terrain terrain = FindObjectOfType<Terrain>();
        if (terrain == null)
        {
            EditorUtility.DisplayDialog("Error", "No terrain found!", "OK");
            return;
        }
        
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        
        // Create flat terrain
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = 0.1f; // Slightly above 0 for visibility
            }
        }
        
        Undo.RegisterCompleteObjectUndo(terrainData, "Reset Terrain Flat");
        terrainData.SetHeights(0, 0, heights);
        
        Debug.Log("Terrain reset to completely flat");
        EditorUtility.DisplayDialog("Success!", "Terrain is now completely flat!", "OK");
    }
}

/// <summary>
/// Quick menu item to instantly position player on terrain center
/// </summary>
public class QuickPlayerFix
{
    [MenuItem("Tools/Quick Fix: Position Player NOW! %#P")] // Ctrl+Shift+P
    public static void QuickPositionPlayer()
    {
        // Find player
        GameObject player = GameObject.Find("Player_Mage");
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }
        
        // Find terrain
        Terrain terrain = Object.FindObjectOfType<Terrain>();
        if (terrain == null)
        {
            Debug.LogError("Terrain not found!");
            return;
        }
        
        // Position at center, 2m above terrain
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
        SceneView.lastActiveSceneView.FrameSelected();
        
        Debug.Log($"<color=green>✓ Player quickly positioned at {centerPos}!</color>");
    }
}

