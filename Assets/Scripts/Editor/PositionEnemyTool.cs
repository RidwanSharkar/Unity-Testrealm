using UnityEngine;
using UnityEditor;

/// <summary>
/// Quick tool to position enemies in the scene
/// </summary>
public class PositionEnemyTool : EditorWindow
{
    private float distanceFromPlayer = 10f;
    private bool positionAtCenter = false;
    
    [MenuItem("Tools/Position Enemy Near Player 🎯")]
    public static void ShowWindow()
    {
        GetWindow<PositionEnemyTool>("Position Enemy");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Position Enemy Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "Positions the Mutant enemy near the player for easy testing.", 
            MessageType.Info);
        
        EditorGUILayout.Space();
        
        positionAtCenter = EditorGUILayout.Toggle("Position at Terrain Center", positionAtCenter);
        
        if (!positionAtCenter)
        {
            distanceFromPlayer = EditorGUILayout.Slider("Distance from Player", distanceFromPlayer, 5f, 30f);
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("POSITION MUTANT NOW!", GUILayout.Height(40)))
        {
            PositionMutant();
        }
    }
    
    [MenuItem("Tools/QUICK: Position Mutant Near Player! 🎯 %#E")] // Ctrl+Shift+E
    public static void QuickPositionMutant()
    {
        PositionMutant(10f, false);
    }
    
    private void PositionMutant()
    {
        PositionMutant(distanceFromPlayer, positionAtCenter);
    }
    
    private static void PositionMutant(float distance, bool atCenter)
    {
        // Find Mutant enemy
        MutantEnemy mutant = Object.FindObjectOfType<MutantEnemy>();
        if (mutant == null)
        {
            EditorUtility.DisplayDialog("Error", 
                "Mutant enemy not found in scene!\n\n" +
                "Make sure you have a MutantEnemy in the scene.", 
                "OK");
            return;
        }
        
        // Find player
        GameObject player = GameObject.Find("Player_Mage");
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        // Find terrain
        Terrain terrain = Object.FindObjectOfType<Terrain>();
        
        Vector3 newPosition;
        
        if (atCenter && terrain != null)
        {
            // Position at center of terrain
            newPosition = new Vector3(
                terrain.terrainData.size.x / 2,
                0,
                terrain.terrainData.size.z / 2
            );
            
            if (terrain != null)
            {
                float terrainHeight = terrain.SampleHeight(newPosition);
                newPosition.y = terrainHeight + 0.5f; // Slightly above ground
            }
        }
        else if (player != null)
        {
            // Position near player
            // Place in front of player's forward direction
            Vector3 offset = player.transform.forward * distance;
            newPosition = player.transform.position + offset;
            
            // Sample terrain height at this position
            if (terrain != null)
            {
                float terrainHeight = terrain.SampleHeight(newPosition);
                newPosition.y = terrainHeight + 0.5f; // Slightly above ground
            }
            else
            {
                newPosition.y = player.transform.position.y;
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Error", 
                "Player not found!\n\n" +
                "Make sure Player_Mage exists in the scene.", 
                "OK");
            return;
        }
        
        // Move the mutant
        Undo.RecordObject(mutant.transform, "Position Mutant Enemy");
        mutant.transform.position = newPosition;
        
        // Select the mutant
        Selection.activeGameObject = mutant.gameObject;
        if (SceneView.lastActiveSceneView != null)
        {
            SceneView.lastActiveSceneView.FrameSelected();
        }
        
        Debug.Log($"<color=green>✓ Mutant positioned at {newPosition} ({(atCenter ? "center" : distance + "m from player")})</color>");
        
        EditorUtility.DisplayDialog("Success!", 
            $"Mutant enemy repositioned!\n\n" +
            $"Position: {newPosition}\n" +
            $"Location: {(atCenter ? "Terrain center" : distance + "m from player")}", 
            "OK");
    }
}

