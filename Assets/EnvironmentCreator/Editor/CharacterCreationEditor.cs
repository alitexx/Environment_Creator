using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class CharacterCreationEditor : EditorWindow
{
    private bool includeMC = false;
    private GameObject playerCharacterPrefab;
    private AnimationClip[] animations = new AnimationClip[9];
    private float walkSpeed = 1f;
    private float reach = 1f;
    private RectTransform startPosition;

    [MenuItem("Window/Environment Creator/Character Creation")]
    public static void ShowWindow()
    {
        GetWindow<CharacterCreationEditor>("Character Creation");
    }

    private void OnGUI()
    {

        foreach (var existingTag in UnityEditorInternal.InternalEditorUtility.tags)
        {
            if (existingTag == "PlayerCharacter")
            {
                playerCharacterPrefab = GameObject.FindGameObjectWithTag("PlayerCharacter");
            }
        }

        GUILayout.Label("Character Creation", EditorStyles.boldLabel);

        includeMC = EditorGUILayout.Toggle("Include Main Character", includeMC);

        if (includeMC)
        {
            if (GUILayout.Button("Create Player Character Prefab"))
            {
                CreatePlayerCharacterPrefab();
            }

            GUILayout.Label("Animations", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical();

            for (int i = 0; i < 3; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < 3; j++)
                {
                    int index = i * 3 + j;
                    displayLabel(index);
                    animations[index] = (AnimationClip)EditorGUILayout.ObjectField(animations[index], typeof(AnimationClip), false);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            walkSpeed = EditorGUILayout.FloatField("Walk Speed", walkSpeed);
            reach = EditorGUILayout.FloatField("Reach", reach);
            startPosition = (RectTransform)EditorGUILayout.ObjectField("Start Position", startPosition, typeof(RectTransform), true);
        }
        else
        {
            if (playerCharacterPrefab != null)
            {
                Destroy(playerCharacterPrefab);
            }
        }


    }

    private void displayLabel(int index)
    {
        switch(index)
        {
            case 0:
                GUILayout.Label("North West");
                break;
            case 1:
                GUILayout.Label("North Most");
                break;
            case 2:
                GUILayout.Label("North East");
                break;
            case 3:
                GUILayout.Label("West Most");
                break;
            case 4:
                GUILayout.Label("Idle Anim");
                break;
            case 5:
                GUILayout.Label("East Most ");
                break;
            case 6:
                GUILayout.Label("South West");
                break;
            case 7:
                GUILayout.Label("South Most");
                break;
            case 8:
                GUILayout.Label("South East");
                break;
        }
    }

    private void CreatePlayerCharacterPrefab()
    {
        string prefabPath = "Assets/EnvironmentCreator/Prefabs/Player Character";
        playerCharacterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath + "/PlayerCharacter.prefab");

        if (playerCharacterPrefab == null)
        {
            playerCharacterPrefab = new GameObject("PlayerCharacter");
        }
        PlayerMovement playerMovement = playerCharacterPrefab.AddComponent<PlayerMovement>();
        Animator animator = playerCharacterPrefab.AddComponent<Animator>();
        CharacterData characterData = playerCharacterPrefab.AddComponent<CharacterData>();
        AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(prefabPath + "/PlayerAnimController.controller");
        if (animatorController != null)
        {
            ClearAnimatorController(animatorController);
        } else
        {
            // Create a new Animator Controller
            animatorController = AnimatorController.CreateAnimatorControllerAtPath(prefabPath + "/PlayerAnimController.controller");
        }


        // Add float parameters to the Animator Controller
        animatorController.AddParameter("Horizontal", AnimatorControllerParameterType.Float);
        animatorController.AddParameter("Vertical", AnimatorControllerParameterType.Float);
        animatorController.AddParameter("Speed", AnimatorControllerParameterType.Float);

        CreateStatesAndTransitions(animatorController);

        // Assign the new Animator Controller to the Animator component
        animator.runtimeAnimatorController = animatorController;

        // If the tag doesn't exist, create it
        if (!TagHelper.TagExists("PlayerCharacter"))
        {
            TagHelper.AddTag("PlayerCharacter");
        }
        playerCharacterPrefab.tag = "PlayerCharacter";

        // Assign default values
        characterData.reach = reach;
        characterData.animations = animations;
        characterData.startPosition = startPosition;

        playerMovement.walkSpeed = walkSpeed;

        //Move player to where they need to be according to startPosition
        if(startPosition == null)
        {
            Debug.LogWarning("Start position has not been defined. Player will start at the origin [0, 0, 0].");
        } else
        {
            playerCharacterPrefab.transform.position = startPosition.transform.position;
        }

        PrefabUtility.SaveAsPrefabAsset(playerCharacterPrefab, prefabPath + "/PlayerCharacter.prefab");
        //DestroyImmediate(playerCharacterPrefab);

        //Add Camera Movement Script onto camera
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        if (camera == null)
        {
            Debug.LogError("Camera has been deleted. Please define a Main Camera with the tag [MainCamera] and add component [CameraMovement].");
            return;
        }
        CameraMovement camMovement = camera.AddComponent<CameraMovement>();
        camMovement.player = playerCharacterPrefab.transform;

    }

    private void CreateStatesAndTransitions(AnimatorController controller)
    {
        AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;

        // Create states
        AnimatorState[] states = new AnimatorState[animations.Length];

        //Define Idle animation state first (default anim state)

        states[0] = rootStateMachine.AddState(FindAnimationName(4));
        states[0].motion = animations[4];
        int offsetAdd = 1;
        //Define all animation states in animator
        for (int i = 0; i < animations.Length; i++)
        {
            if (i == 4)
            {
                offsetAdd = 0;
                continue;
            }
            states[i+offsetAdd] = rootStateMachine.AddState(FindAnimationName(i));
            states[i+offsetAdd].motion = animations[i];
        }

        // Create transitions based on conditions
        AddTransition(rootStateMachine, rootStateMachine.defaultState, states[0], "Speed", 0.15f, true);                          // Idle
        AddTransition(rootStateMachine, rootStateMachine.defaultState, states[1], "Horizontal", -0.5f, true, "Vertical", 0.5f, false);  // North West
        AddTransition(rootStateMachine, rootStateMachine.defaultState, states[2], "Vertical", 0.5f, false);                          // North
        AddTransition(rootStateMachine, rootStateMachine.defaultState, states[3], "Horizontal", 0.5f, false, "Vertical", 0.5f, false); // North East
        AddTransition(rootStateMachine, rootStateMachine.defaultState, states[4], "Horizontal", -0.5f, false);                        // West
        AddTransition(rootStateMachine, rootStateMachine.defaultState, states[5], "Horizontal", 0.5f, false);                        // East
        AddTransition(rootStateMachine, rootStateMachine.defaultState, states[6], "Horizontal", -0.5f, true, "Vertical", -0.5f, false); // South West
        AddTransition(rootStateMachine, rootStateMachine.defaultState, states[7], "Vertical", -0.5f, false);                          // South
        AddTransition(rootStateMachine, rootStateMachine.defaultState, states[8], "Horizontal", 0.5f, false, "Vertical", -0.5f, false); // South East

        // Add transitions to idle state from any state
        for (int i = 0; i < states.Length; i++)
        {
            if (i == 0)
            {
                continue;
            }
            AddTransitionToIdle(rootStateMachine, states[i], states[0]);
        }
    }

    private void AddTransitionToIdle(AnimatorStateMachine stateMachine, AnimatorState fromState, AnimatorState idleState)
    {
        AnimatorStateTransition transition = fromState.AddTransition(idleState);
        transition.hasExitTime = false;
        transition.hasFixedDuration = true;
        transition.exitTime = 0;
        transition.duration = 0;

        // Set condition for transitioning to idle state (e.g., Speed == 0)
        transition.AddCondition(AnimatorConditionMode.Less, 0.15f, "Speed");
    }

    private void AddTransition(AnimatorStateMachine stateMachine, AnimatorState fromState, AnimatorState toState, string param1, float threshold1, bool isLess1 = false, string param2 = null, float threshold2 = 0, bool isLess2 = false)
    {
        AnimatorStateTransition transition = fromState.AddTransition(toState);
        transition.hasExitTime = false;
        transition.hasFixedDuration = true;
        transition.exitTime = 0;
        transition.duration = 0;

        AnimatorConditionMode mode1 = isLess1 ? AnimatorConditionMode.Less : AnimatorConditionMode.Greater;
        transition.AddCondition(mode1, threshold1, param1);

        if (!string.IsNullOrEmpty(param2))
        {
            AnimatorConditionMode mode2 = isLess2 ? AnimatorConditionMode.Less : AnimatorConditionMode.Greater;
            transition.AddCondition(mode2, threshold2, param2);
        }
    }

    private void ClearAnimatorController(AnimatorController animatorController)
    {
        // Clear all states and transitions
        AnimatorStateMachine rootStateMachine = animatorController.layers[0].stateMachine;
        rootStateMachine.states = new ChildAnimatorState[0];
        rootStateMachine.anyStateTransitions = new AnimatorStateTransition[0];
        rootStateMachine.entryTransitions = new AnimatorTransition[0];

        // Clear all parameters
        animatorController.parameters = new AnimatorControllerParameter[0];
    }

    private string FindAnimationName(int iteration)
    {
        switch (iteration)
        {
            case 0:
                return "North West";
            case 1:
                return "North";
            case 2:
                return "North East";
            case 3:
                return "West";
            case 4:
                return "Idle";
            case 5:
                return "East";
            case 6:
                return "South West";
            case 7:
                return "South";
            case 8:
                return "South East";
            default:
                return "???";
        }
    }

}
