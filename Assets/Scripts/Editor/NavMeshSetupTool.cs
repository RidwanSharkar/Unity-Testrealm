using UnityEngine;
using UnityEditor;
using Unity.AI.Navigation;

/// <summary>
/// Quick tool to setup NavMesh for enemy AI pathfinding.
/// Automatically adds NavMesh Surface to terrain and bakes it!
/// </summary>
public class NavMeshSetupTool : EditorWindow
{
    [MenuItem("Tools/Setup NavMesh for Enemies! 🎯")]
    public static void ShowWindow()
    {
        GetWindow<NavMeshSetupTool>("NavMesh Setup");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("NavMesh Setup Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "This will setup NavMesh on your terrain so enemies can navigate!\n\n" +
            "It will:\n" +
            "• Add NavMeshSurface component to terrain\n" +
            "• Configure it for humanoid agents\n" +
            "• Bake the NavMesh automatically", 
            MessageType.Info);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("SETUP NAVMESH NOW!", GUILayout.Height(50)))
        {
            SetupNavMesh();
        }
        
        EditorGUILayout.Space();
        
        GUILayout.Label("Manual Steps:", EditorStyles.boldLabel);
        if (GUILayout.Button("Just Add NavMeshSurface Component", GUILayout.Height(30)))
        {
            AddNavMeshSurfaceOnly();
        }
        
        if (GUILayout.Button("Bake All NavMeshSurfaces in Scene", GUILayout.Height(30)))
        {
            BakeAllNavMeshes();
        }
    }
    
    private void SetupNavMesh()
    {
        // Find terrain
        Terrain terrain = Object.FindObjectOfType<Terrain>();
        if (terrain == null)
        {
            EditorUtility.DisplayDialog("Error", 
                "No terrain found in scene!\n\n" +
                "Create a terrain first or make sure it's active in the scene.", 
                "OK");
            return;
        }
        
        // Check if NavMeshSurface already exists
        NavMeshSurface existingSurface = terrain.GetComponent<NavMeshSurface>();
        if (existingSurface != null)
        {
            if (!EditorUtility.DisplayDialog("NavMesh Already Exists", 
                "A NavMeshSurface component already exists on the terrain.\n\n" +
                "Do you want to rebuild it?", 
                "Yes, Rebuild", "Cancel"))
            {
                return;
            }
            
            // Remove old one
            Undo.DestroyObjectImmediate(existingSurface);
        }
        
        // Add NavMeshSurface component
        NavMeshSurface surface = Undo.AddComponent<NavMeshSurface>(terrain.gameObject);
        
        // Configure for humanoid agents (matches the Agent settings shown in the screenshot)
        surface.agentTypeID = 0; // Humanoid
        surface.collectObjects = CollectObjects.Volume;
        surface.size = terrain.terrainData.size;
        surface.center = terrain.terrainData.size / 2f;
        surface.layerMask = ~0; // All layers
        // useGeometry uses default settings (PhysicsColliders)
        
        // Mark as modified
        EditorUtility.SetDirty(surface);
        
        Debug.Log("<color=yellow>NavMeshSurface added to terrain. Now baking...</color>");
        
        // Bake the NavMesh
        surface.BuildNavMesh();
        
        Debug.Log("<color=green>✓ NavMesh baked successfully!</color>");
        
        // Set player tag if not already set
        SetPlayerTag();
        
        EditorUtility.DisplayDialog("Success!", 
            "✓ NavMesh setup complete!\n\n" +
            "• NavMeshSurface component added to terrain\n" +
            "• NavMesh baked successfully\n" +
            "• Player tag checked\n\n" +
            "Your enemies can now navigate the terrain!", 
            "Awesome!");
    }
    
    private void AddNavMeshSurfaceOnly()
    {
        Terrain terrain = Object.FindObjectOfType<Terrain>();
        if (terrain == null)
        {
            EditorUtility.DisplayDialog("Error", "No terrain found!", "OK");
            return;
        }
        
        // Check if already exists
        if (terrain.GetComponent<NavMeshSurface>() != null)
        {
            EditorUtility.DisplayDialog("Already Exists", 
                "NavMeshSurface component already exists on the terrain!", 
                "OK");
            return;
        }
        
        NavMeshSurface surface = Undo.AddComponent<NavMeshSurface>(terrain.gameObject);
        surface.collectObjects = CollectObjects.Volume;
        surface.size = terrain.terrainData.size;
        surface.center = terrain.terrainData.size / 2f;
        
        EditorUtility.SetDirty(surface);
        Selection.activeGameObject = terrain.gameObject;
        
        Debug.Log("<color=green>✓ NavMeshSurface component added!</color>");
        EditorUtility.DisplayDialog("Success!", 
            "NavMeshSurface component added!\n\n" +
            "Now you need to bake it. Click 'Bake All NavMeshSurfaces in Scene' button.", 
            "OK");
    }
    
    private void BakeAllNavMeshes()
    {
        NavMeshSurface[] surfaces = Object.FindObjectsOfType<NavMeshSurface>();
        
        if (surfaces.Length == 0)
        {
            EditorUtility.DisplayDialog("No NavMeshSurface Found", 
                "No NavMeshSurface components found in the scene!\n\n" +
                "Add one first using 'Just Add NavMeshSurface Component' button.", 
                "OK");
            return;
        }
        
        foreach (NavMeshSurface surface in surfaces)
        {
            Debug.Log($"Baking NavMesh on {surface.gameObject.name}...");
            surface.BuildNavMesh();
        }
        
        Debug.Log($"<color=green>✓ Baked {surfaces.Length} NavMesh(es) successfully!</color>");
        EditorUtility.DisplayDialog("Success!", 
            $"Baked {surfaces.Length} NavMesh surface(s)!\n\n" +
            "Your enemies can now navigate!", 
            "OK");
    }
    
    private void SetPlayerTag()
    {
        GameObject player = GameObject.Find("Player_Mage");
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        if (player != null && !player.CompareTag("Player"))
        {
            Undo.RecordObject(player, "Set Player Tag");
            player.tag = "Player";
            Debug.Log("<color=green>✓ Player tag set on Player_Mage</color>");
        }
        else if (player != null)
        {
            Debug.Log("Player already has 'Player' tag - good!");
        }
        else
        {
            Debug.LogWarning("Player not found in scene. Make sure to set the 'Player' tag on your player GameObject manually.");
        }
    }
}

/// <summary>
/// Super quick menu item to instantly setup NavMesh
/// </summary>
public class QuickNavMeshSetup
{
    [MenuItem("Tools/QUICK: Setup NavMesh NOW! 🎯 %#N")] // Ctrl+Shift+N
    public static void QuickSetupNavMesh()
    {
        Terrain terrain = Object.FindObjectOfType<Terrain>();
        if (terrain == null)
        {
            Debug.LogError("No terrain found!");
            EditorUtility.DisplayDialog("Error", "No terrain found in scene!", "OK");
            return;
        }
        
        // Remove existing if present
        NavMeshSurface existingSurface = terrain.GetComponent<NavMeshSurface>();
        if (existingSurface != null)
        {
            Object.DestroyImmediate(existingSurface);
        }
        
        // Add and configure NavMeshSurface
        NavMeshSurface surface = Undo.AddComponent<NavMeshSurface>(terrain.gameObject);
        surface.collectObjects = CollectObjects.Volume;
        surface.size = terrain.terrainData.size;
        surface.center = terrain.terrainData.size / 2f;
        surface.layerMask = ~0;
        
        // Bake
        Debug.Log("Baking NavMesh...");
        surface.BuildNavMesh();
        
        // Set player tag
        GameObject player = GameObject.Find("Player_Mage");
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        if (player != null && !player.CompareTag("Player"))
        {
            Undo.RecordObject(player, "Set Player Tag");
            player.tag = "Player";
        }
        
        Debug.Log("<color=green>✓✓✓ NAVMESH SETUP COMPLETE! Enemies ready to navigate! ✓✓✓</color>");
    }
}

