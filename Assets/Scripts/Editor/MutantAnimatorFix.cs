using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// Tool to rebuild the Mutant Animator Controller properly
/// This will fix the animation issue by creating a fresh controller
/// </summary>
public class MutantAnimatorFix : EditorWindow
{
    [MenuItem("Tools/Fix Mutant Animations! 🎬")]
    public static void ShowWindow()
    {
        GetWindow<MutantAnimatorFix>("Fix Mutant Animations");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Mutant Animation Fixer", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "This will rebuild the Mutant Animator Controller from scratch.\n\n" +
            "This fixes the issue where animations aren't playing even though triggers are being called.", 
            MessageType.Info);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("REBUILD MUTANT ANIMATOR CONTROLLER", GUILayout.Height(50)))
        {
            RebuildAnimatorController();
        }
    }
    
    [MenuItem("Tools/QUICK: Fix Mutant Animations NOW! 🎬 %#M")] // Ctrl+Shift+M
    public static void QuickFix()
    {
        RebuildAnimatorController();
    }
    
    private static void RebuildAnimatorController()
    {
        string controllerPath = "Assets/Animations/Enemies/Mutant/Mutant_AnimatorController.controller";
        
        // Delete old controller if it exists
        if (AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath) != null)
        {
            AssetDatabase.DeleteAsset(controllerPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Deleted old Mutant_AnimatorController");
        }
        
        // Create new Animator Controller
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        
        // Add parameters
        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("IsAttacking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Idle", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Run", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Punch", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Swiping", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Dying", AnimatorControllerParameterType.Trigger);
        
        Debug.Log("Added animator parameters");
        
        // Get the root state machine
        AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;
        
        // Load animation clips
        AnimationClip idleClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/Enemies/Mutant/Mutant_Idle.fbx");
        AnimationClip runClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/Enemies/Mutant/Mutant_Run.fbx");
        AnimationClip punchClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/Enemies/Mutant/Mutant_Punch.fbx");
        AnimationClip swipingClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/Enemies/Mutant/Mutant_Swiping.fbx");
        AnimationClip dyingClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Animations/Enemies/Mutant/Mutant_Dying.fbx");
        
        if (idleClip == null || runClip == null || punchClip == null || swipingClip == null || dyingClip == null)
        {
            Debug.LogError("Failed to load one or more animation clips! Check if the .fbx files exist.");
            EditorUtility.DisplayDialog("Error", 
                "Failed to load animation clips!\n\n" +
                "Make sure all Mutant animation FBX files exist in:\n" +
                "Assets/Animations/Enemies/Mutant/", 
                "OK");
            return;
        }
        
        Debug.Log("Loaded all animation clips");
        
        // Create states
        AnimatorState idleState = rootStateMachine.AddState("Idle", new Vector3(300, 50, 0));
        idleState.motion = idleClip;
        
        AnimatorState runState = rootStateMachine.AddState("Run", new Vector3(300, 150, 0));
        runState.motion = runClip;
        
        AnimatorState punchState = rootStateMachine.AddState("Punch", new Vector3(500, 50, 0));
        punchState.motion = punchClip;
        
        AnimatorState swipingState = rootStateMachine.AddState("Swiping", new Vector3(500, 150, 0));
        swipingState.motion = swipingClip;
        
        AnimatorState dyingState = rootStateMachine.AddState("Dying", new Vector3(300, 250, 0));
        dyingState.motion = dyingClip;
        
        // Set Idle as default state
        rootStateMachine.defaultState = idleState;
        
        Debug.Log("Created animator states");
        
        // Create transitions from Idle to Run
        AnimatorStateTransition idleToRun = idleState.AddTransition(runState);
        idleToRun.AddCondition(AnimatorConditionMode.If, 0, "Run");
        idleToRun.hasExitTime = false;
        idleToRun.duration = 0.25f;
        
        // Create transitions from Run to Idle
        AnimatorStateTransition runToIdle = runState.AddTransition(idleState);
        runToIdle.AddCondition(AnimatorConditionMode.If, 0, "Idle");
        runToIdle.hasExitTime = false;
        runToIdle.duration = 0.25f;
        
        // Create transitions from Any State to Punch
        AnimatorStateTransition anyToPunch = rootStateMachine.AddAnyStateTransition(punchState);
        anyToPunch.AddCondition(AnimatorConditionMode.If, 0, "Punch");
        anyToPunch.hasExitTime = false;
        anyToPunch.duration = 0.1f;
        
        // Create transitions from Punch back to Idle
        AnimatorStateTransition punchToIdle = punchState.AddTransition(idleState);
        punchToIdle.hasExitTime = true;
        punchToIdle.exitTime = 0.9f;
        punchToIdle.duration = 0.25f;
        
        // Create transitions from Any State to Swiping
        AnimatorStateTransition anyToSwiping = rootStateMachine.AddAnyStateTransition(swipingState);
        anyToSwiping.AddCondition(AnimatorConditionMode.If, 0, "Swiping");
        anyToSwiping.hasExitTime = false;
        anyToSwiping.duration = 0.1f;
        
        // Create transitions from Swiping back to Idle
        AnimatorStateTransition swipingToIdle = swipingState.AddTransition(idleState);
        swipingToIdle.hasExitTime = true;
        swipingToIdle.exitTime = 0.9f;
        swipingToIdle.duration = 0.25f;
        
        // Create transitions from Any State to Dying
        AnimatorStateTransition anyToDying = rootStateMachine.AddAnyStateTransition(dyingState);
        anyToDying.AddCondition(AnimatorConditionMode.If, 0, "Dying");
        anyToDying.hasExitTime = false;
        anyToDying.duration = 0.1f;
        
        Debug.Log("Created all transitions");
        
        // Save the controller
        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("<color=green>✓ Mutant Animator Controller rebuilt successfully!</color>");
        
        // Try to find and update the Mutant enemy in the scene
        UpdateMutantInScene();
        
        EditorUtility.DisplayDialog("Success!", 
            "✓ Mutant Animator Controller rebuilt!\n\n" +
            "• All animation clips linked\n" +
            "• All transitions set up\n" +
            "• Parameters configured\n\n" +
            "Test the mutant enemy now - animations should work!", 
            "Awesome!");
    }
    
    private static void UpdateMutantInScene()
    {
        // Find MutantEnemy in scene
        MutantEnemy mutant = Object.FindObjectOfType<MutantEnemy>();
        if (mutant != null)
        {
            // Get the animator
            Animator animator = mutant.GetComponentInChildren<Animator>();
            if (animator != null)
            {
                // Reload the controller
                AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(
                    "Assets/Animations/Enemies/Mutant/Mutant_AnimatorController.controller");
                    
                animator.runtimeAnimatorController = controller;
                EditorUtility.SetDirty(animator);
                
                Debug.Log($"<color=green>✓ Updated animator controller on {mutant.name}</color>");
            }
        }
    }
}

