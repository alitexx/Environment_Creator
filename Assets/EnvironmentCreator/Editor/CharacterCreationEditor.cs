using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using static UnityEngine.UI.ScrollRect;





public class CharacterCreationEditor : EditorWindow
{
    private enum MovementType { Random, SetPositions }
    private bool includeMC = false;
    private GameObject playerCharacterPrefab;
    private AnimationClip[] animations = new AnimationClip[9];
    private float walkSpeed = 1f;
    private float reach = 1f;
    private Transform startPosition;

    // NPC Editor variables
    private GameObject npcGameObject;
    private bool canMove = false;
    private float movementSpeed = 1f;
    private float waitTime = 1f;
    private float moveFrequency = 1f;
    private AnimationClip[] npcAnimations = new AnimationClip[9];
    private bool canInteract = false;
    private GameObject interactionPopupBox;
    private string interactionText = "";
    private bool updatePrefab = false;

    //movement
    private MovementType movementType;
    private SerializedObject serializedObject;
    private SerializedProperty setPositionsProperty;

    [SerializeField]
    private Transform[] setPositions = new Transform[0];

    private void OnEnable()
    {
        serializedObject = new SerializedObject(this);
        setPositionsProperty = serializedObject.FindProperty("setPositions");

        // Ensure setPositions is initialized
        if (setPositions == null)
        {
            setPositions = new Transform[0];
        }
    }


    [MenuItem("Window/Environment Creator/Character Creation")]
    public static void ShowWindow()
    {
        GetWindow<CharacterCreationEditor>("Character Creation");
    }

    private void OnGUI()
    {
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
            startPosition = (Transform)EditorGUILayout.ObjectField("Start Position", startPosition, typeof(Transform), true);
        }
        else
        {
            if (playerCharacterPrefab != null)
            {
                Destroy(playerCharacterPrefab);
            }
        }


        ///NPC EDITOR!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


        GUILayout.Space(20);
        GUILayout.Label("NPC Editor", EditorStyles.boldLabel);

        npcGameObject = (GameObject)EditorGUILayout.ObjectField("NPC GameObject", npcGameObject, typeof(GameObject), true);

        if (npcGameObject != null)
        {
            serializedObject.Update();
            canMove = EditorGUILayout.Toggle("Can Move", canMove);

            if (canMove)
            {
                movementSpeed = EditorGUILayout.FloatField("Movement Speed", movementSpeed);

                movementType = (MovementType)EditorGUILayout.EnumPopup("Movement Type", movementType);

                if (movementType == MovementType.Random)
                {
                    moveFrequency = EditorGUILayout.FloatField("Move Frequency", moveFrequency);
                }
                if (movementType == MovementType.SetPositions)
                {
                    waitTime = EditorGUILayout.FloatField("Wait time (in seconds)", waitTime);
                    EditorGUILayout.PropertyField(setPositionsProperty, true);
                }



                GUILayout.Label("NPC Animations", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical();

                for (int i = 0; i < 3; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int j = 0; j < 3; j++)
                    {
                        int index = i * 3 + j;
                        displayLabel(index);
                        npcAnimations[index] = (AnimationClip)EditorGUILayout.ObjectField(npcAnimations[index], typeof(AnimationClip), false);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();

                serializedObject.ApplyModifiedProperties();
            }

            canInteract = EditorGUILayout.Toggle("Can Interact", canInteract);

            if (canInteract)
            {
                interactionPopupBox = (GameObject)EditorGUILayout.ObjectField("Interaction Popup Box", interactionPopupBox, typeof(GameObject), true);
                interactionText = EditorGUILayout.TextArea(interactionText, GUILayout.Height(100));
            }

            if (GUILayout.Button("Update this NPC"))
            {
                updatePrefab = false;
                UpdateNPC();
            }

            if (GUILayout.Button("Update all NPCs of this type"))
            {
                updatePrefab = true;
                UpdateNPC();
            }
        }
    }

    private void UpdateNPC()
    {
        //Check for pre-existing components. if there are pre existing components, delete them.
        if(canMove)
        {
            NPCMovement npcmovement = npcGameObject.AddComponent<NPCMovement>();
            if(movementType == MovementType.Random)
            {
                npcmovement.SetValues(movementSpeed, moveFrequency, setPositions, false, waitTime);
            }
            else
            {
                npcmovement.SetValues(movementSpeed, moveFrequency, setPositions, true, waitTime);
            }
            
            string prefabPath = "Assets/EnvironmentCreator/Prefabs/NPC";
            Animator animator;
            try
            {
                animator = npcGameObject.AddComponent<Animator>();
            }
            catch
            {
                animator = npcGameObject.GetComponent<Animator>();
            }

            // Implementation for updating this specific NPC game object
            AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(prefabPath + "/PlayerAnimController.controller");
            if (animatorController != null)
            {
                ClearAnimatorController(animatorController);
            }
            else
            {
                // Create a new Animator Controller
                animatorController = AnimatorController.CreateAnimatorControllerAtPath(prefabPath + "/PlayerAnimController.controller");
            }


            // Add float parameters to the Animator Controller
            animatorController.AddParameter("Horizontal", AnimatorControllerParameterType.Float);
            animatorController.AddParameter("Vertical", AnimatorControllerParameterType.Float);
            animatorController.AddParameter("Speed", AnimatorControllerParameterType.Float);

            CreateStatesAndTransitions(animatorController, npcAnimations);

            // Assign the new Animator Controller to the Animator component
            animator.runtimeAnimatorController = animatorController;
        }
        if(canInteract)
        {
            NPCInteract npcinteract = npcGameObject.AddComponent<NPCInteract>();
            npcinteract.popup = interactionPopupBox;
            npcinteract.popuptext = interactionText;
        }

        // If the tag doesn't exist, create it
        if (!TagHelper.TagExists("NPC"))
        {
            TagHelper.AddTag("NPC");
        }
        npcGameObject.tag = "NPC";


        //If we should update the prefab too, update prefab
        if(updatePrefab)
        {
            UpdateAllNPCsOfType();
        }
    }

    private void UpdateAllNPCsOfType()
    {
        // Implementation for updating all NPCs of this type (prefab)
        string prefabPath = "Assets/EnvironmentCreator/Prefabs/NPC";
        PrefabUtility.SaveAsPrefabAsset(npcGameObject, prefabPath + "/" + npcGameObject.name + ".prefab");

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
        Animator animator;
        playerCharacterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath + "/PlayerCharacter.prefab");

        if (playerCharacterPrefab == null)
        {
            playerCharacterPrefab = new GameObject("PlayerCharacter");
        }
        PlayerMovement playerMovement = playerCharacterPrefab.AddComponent<PlayerMovement>();
        try
        {
            animator = playerCharacterPrefab.AddComponent<Animator>();
        } catch
        {
            animator = playerCharacterPrefab.GetComponent<Animator>();
        }
        //Animator animator = playerCharacterPrefab.AddComponent<Animator>();
        CharacterData characterData = playerCharacterPrefab.AddComponent<CharacterData>();

        AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(prefabPath + "/PlayerAnimController.controller");
        if (animatorController != null)
        {
            ClearAnimatorController(animatorController);
        }
        else
        {
            // Create a new Animator Controller
            animatorController = AnimatorController.CreateAnimatorControllerAtPath(prefabPath + "/PlayerAnimController.controller");
        }

        // Add float parameters to the Animator Controller
        animatorController.AddParameter("Horizontal", AnimatorControllerParameterType.Float);
        animatorController.AddParameter("Vertical", AnimatorControllerParameterType.Float);
        animatorController.AddParameter("Speed", AnimatorControllerParameterType.Float);

        // Check if CreateStatesAndTransitions returns a valid AnimatorController
        AnimatorController newAnimatorController = CreateStatesAndTransitions(animatorController, animations);
        if (newAnimatorController == null)
        {
            Debug.LogError("CreateStatesAndTransitions returned null.");
        }
        else if (animator == null)
        {
            Debug.LogError("Animator component is null.");
        }
        else
        {
            // Assign the new Animator Controller to the Animator component
            animator.runtimeAnimatorController = newAnimatorController;
        }


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

        //Add Rigidbody and colliders for collisions
        Rigidbody2D rigidbody2D = playerCharacterPrefab.AddComponent<Rigidbody2D>();
        BoxCollider2D boxCollider2D = playerCharacterPrefab.AddComponent<BoxCollider2D>();

        PrefabUtility.SaveAsPrefabAsset(playerCharacterPrefab, prefabPath + "/PlayerCharacter.prefab");
        //DestroyImmediate(playerCharacterPrefab);

        //Add Camera Movement Script onto camera
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        if (camera == null)
        {
            Debug.LogError("Camera has been deleted. Please define a Main Camera with the tag [MainCamera] and add component [CameraMovement].");
            return;
        }

        CameraMovement camMovement = camera.GetComponent<CameraMovement>();
        if (camMovement == null)
        {
            camMovement = camera.AddComponent<CameraMovement>();
        }
        camMovement.player = playerCharacterPrefab.transform;
    }

    private AnimatorController CreateStatesAndTransitions(AnimatorController controller, AnimationClip[] animations)
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

        return controller;
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
