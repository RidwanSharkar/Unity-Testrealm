using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor tool to quickly create a Mage Fireball prefab
/// Unity Menu: Tools → Create Mage Fireball Prefab
/// </summary>
public class FireballPrefabCreator : EditorWindow
{
    private string prefabName = "MageFireballPrimary";
    private float fireballSize = 0.5f;
    private float projectileSpeed = 25f;
    private float lifetime = 5f;
    private Color fireColor = new Color(1f, 0.5f, 0f); // Orange
    
    [MenuItem("Tools/Create Mage Fireball Prefab")]
    public static void ShowWindow()
    {
        GetWindow<FireballPrefabCreator>("Fireball Creator");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Mage Fireball Prefab Creator", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        prefabName = EditorGUILayout.TextField("Prefab Name:", prefabName);
        fireballSize = EditorGUILayout.Slider("Fireball Size:", fireballSize, 0.1f, 2f);
        projectileSpeed = EditorGUILayout.Slider("Speed:", projectileSpeed, 10f, 50f);
        lifetime = EditorGUILayout.Slider("Lifetime:", lifetime, 1f, 10f);
        fireColor = EditorGUILayout.ColorField("Fire Color:", fireColor);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Create Fireball Prefab", GUILayout.Height(40)))
        {
            CreateFireballPrefab();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("This will create:", EditorStyles.helpBox);
        GUILayout.Label("• GameObject with Rigidbody");
        GUILayout.Label("• Sphere Collider");
        GUILayout.Label("• MageFireballProjectile script");
        GUILayout.Label("• Trail Renderer");
        GUILayout.Label("• Point Light");
        GUILayout.Label("• Visual sphere");
    }
    
    private void CreateFireballPrefab()
    {
        // Create main GameObject
        GameObject fireball = new GameObject(prefabName);
        
        // Add Rigidbody
        Rigidbody rb = fireball.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        
        // Add Sphere Collider
        SphereCollider collider = fireball.AddComponent<SphereCollider>();
        collider.radius = fireballSize;
        collider.isTrigger = false;
        
        // Add MageFireballProjectile script
        MageFireballProjectile projectile = fireball.AddComponent<MageFireballProjectile>();
        
        // Use reflection to set private fields (for editor-time setup)
        var speedField = typeof(Projectile).GetField("speed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (speedField != null) speedField.SetValue(projectile, projectileSpeed);
        
        var lifetimeField = typeof(Projectile).GetField("lifetime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (lifetimeField != null) lifetimeField.SetValue(projectile, lifetime);
        
        // Add Trail Renderer
        TrailRenderer trail = fireball.AddComponent<TrailRenderer>();
        trail.time = 0.5f;
        trail.widthMultiplier = 0.3f;
        trail.startWidth = 0.3f;
        trail.endWidth = 0f;
        
        // Set trail color gradient
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(fireColor, 0.0f), 
                new GradientColorKey(Color.red, 1.0f) 
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(1.0f, 0.0f), 
                new GradientAlphaKey(0.0f, 1.0f) 
            }
        );
        trail.colorGradient = gradient;
        
        // Create default material for trail
        Material trailMaterial = new Material(Shader.Find("Sprites/Default"));
        trailMaterial.color = fireColor;
        trail.material = trailMaterial;
        
        // Create visual child object
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visual.name = "FireVisual";
        visual.transform.SetParent(fireball.transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = Vector3.one * fireballSize * 2;
        
        // Remove collider from visual (main object has collider)
        DestroyImmediate(visual.GetComponent<Collider>());
        
        // Add material to visual
        Material fireMaterial = new Material(Shader.Find("Standard"));
        fireMaterial.SetColor("_Color", fireColor);
        fireMaterial.SetColor("_EmissionColor", fireColor * 2f);
        fireMaterial.EnableKeyword("_EMISSION");
        visual.GetComponent<Renderer>().material = fireMaterial;
        
        // Add Point Light
        GameObject lightObj = new GameObject("FireLight");
        lightObj.transform.SetParent(fireball.transform);
        lightObj.transform.localPosition = Vector3.zero;
        
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = fireColor;
        light.intensity = 2f;
        light.range = 5f;
        
        // Set layer to Projectile if it exists
        int projectileLayer = LayerMask.NameToLayer("Projectile");
        if (projectileLayer != -1)
        {
            fireball.layer = projectileLayer;
        }
        
        // Create prefab folder if it doesn't exist
        string folderPath = "Assets/Prefabs";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        string projectileFolderPath = "Assets/Prefabs/Projectiles";
        if (!AssetDatabase.IsValidFolder(projectileFolderPath))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "Projectiles");
        }
        
        // Save as prefab
        string prefabPath = $"{projectileFolderPath}/{prefabName}.prefab";
        bool success = false;
        
        // Save the prefab
        GameObject prefabAsset = PrefabUtility.SaveAsPrefabAsset(fireball, prefabPath, out success);
        
        if (success)
        {
            Debug.Log($"<color=green>✓ Fireball prefab created successfully at: {prefabPath}</color>");
            Debug.Log($"<color=yellow>→ Now assign this prefab to your MageSpellCaster component!</color>");
            
            // Select the prefab in project
            Selection.activeObject = prefabAsset;
            EditorGUIUtility.PingObject(prefabAsset);
        }
        else
        {
            Debug.LogError("Failed to create fireball prefab!");
        }
        
        // Clean up scene object
        DestroyImmediate(fireball);
        
        Close();
    }
}

