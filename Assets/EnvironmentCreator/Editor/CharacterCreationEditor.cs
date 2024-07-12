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
            
            NPCMovement npcmovement = ComponentHelper.GetOrAddComponent<NPCMovement>(npcGameObject);
            if (movementType == MovementType.Random)
            {
                npcmovement.SetValues(movementSpeed, moveFrequency, setPositions, false, waitTime);
            }
            else
            {
                npcmovement.SetValues(movementSpeed, moveFrequency, setPositions, true, waitTime);
            }
            
            string prefabPath = "Assets/EnvironmentCreator/Prefabs/NPC";
            Animator animator = ComponentHelper.GetOrAddComponent<Animator>(npcGameObject);
            SpriteRenderer npcSprite = ComponentHelper.GetOrAddComponent<SpriteRenderer>(npcGameObject);
            npcSprite.sprite = GetFirstSpriteFromAnimation(npcAnimations[4]);


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
            NPCInteract npcinteract = ComponentHelper.GetOrAddComponent<NPCInteract>(npcGameObject);
            npcinteract.popup = interactionPopupBox;
            npcinteract.popuptext = interactionText;
        }

        //Add Rigidbody and colliders for collisions
        Rigidbody2D rigidbody2D = ComponentHelper.GetOrAddComponent<Rigidbody2D>(npcGameObject);
        BoxCollider2D boxCollider2D = ComponentHelper.GetOrAddComponent<BoxCollider2D>(npcGameObject);

        //Set Rigidbody settings so it doesnt have physics
        rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        rigidbody2D.isKinematic = true;
        rigidbody2D.gravityScale = 0;


        // If the tag doesn't exist, create it
        if (!TagHelper.TagExists("NPC"))
        {
            TagHelper.AddTag("NPC", npcGameObject);
        } else
        {
            npcGameObject.tag = "NPC";
        }

        //If we should update the prefab too, update prefab
        if(updatePrefab)
        {
            UpdateAllNPCsOfType();
        }

        Debug.Log("NPC updated! Please check it's BoxCollider2D component and shape it to the sprite.");
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
        playerCharacterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath + "/PlayerCharacter.prefab");

        if (playerCharacterPrefab == null)
        {
            playerCharacterPrefab = new GameObject("PlayerCharacter");
        }


        Animator animator = ComponentHelper.GetOrAddComponent<Animator>(playerCharacterPrefab);
        PlayerMovement playerMovement = ComponentHelper.GetOrAddComponent<PlayerMovement>(playerCharacterPrefab);
        CharacterData characterData = ComponentHelper.GetOrAddComponent<CharacterData>(playerCharacterPrefab);
        //I don't think I need to use this in code, but putting it here

        SpriteRenderer charSprite = ComponentHelper.GetOrAddComponent<SpriteRenderer>(playerCharacterPrefab);
        charSprite.sprite = GetFirstSpriteFromAnimation(animations[4]);

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
            TagHelper.AddTag("PlayerCharacter", playerCharacterPrefab);
        } else
        {
            playerCharacterPrefab.tag = "PlayerCharacter";
        }
        // Assign default values
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
        Rigidbody2D rigidbody2D = ComponentHelper.GetOrAddComponent<Rigidbody2D>(playerCharacterPrefab);
        BoxCollider2D boxCollider2D = ComponentHelper.GetOrAddComponent<BoxCollider2D>(playerCharacterPrefab);

        //Set Rigidbody settings so it doesnt have physics
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        rigidbody2D.isKinematic = false;
        rigidbody2D.gravityScale = 0;

        PrefabUtility.SaveAsPrefabAsset(playerCharacterPrefab, prefabPath + "/PlayerCharacter.prefab");
        //DestroyImmediate(playerCharacterPrefab);

        //Add Camera Movement Script onto camera
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        if (camera == null)
        {
            Debug.LogError("Camera has been deleted. Please define a Main Camera with the tag [MainCamera] and add component [CameraMovement].");
            return;
        }

        CameraMovement camMovement = ComponentHelper.GetOrAddComponent<CameraMovement>(camera);
        camMovement.player = playerCharacterPrefab.transform;

        Debug.Log("Player Character updated! Please check it's BoxCollider2D component and shape it to the sprite.");
    }

    private AnimatorController CreateStatesAndTransitions(AnimatorController controller, AnimationClip[] animations)
    {
        AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;

        // Create a blend tree
        AnimatorState blendTreeState = rootStateMachine.AddState("MovementBlendTree");
        blendTreeState.motion = CreateBlendTree(controller, animations);

        // Create an Idle state
        AnimatorState idleState = rootStateMachine.AddState("Idle");
        idleState.motion = animations[4]; // Assuming 4 is the index for Idle animation

        // Create a transition from Idle to BlendTree and vice versa
        AddTransition(rootStateMachine, idleState, blendTreeState, "Speed", 0.15f, false);
        AddTransition(rootStateMachine, blendTreeState, idleState, "Speed", 0.15f, true);

        return controller;
    }


    private BlendTree CreateBlendTree(AnimatorController controller, AnimationClip[] animations)
    {
        BlendTree blendTree = new BlendTree();
        blendTree.name = "MovementBlendTree";
        blendTree.blendType = BlendTreeType.SimpleDirectional2D;
        blendTree.blendParameter = "Horizontal";
        blendTree.blendParameterY = "Vertical";

        blendTree.useAutomaticThresholds = false;

        // Add Idle animation
        blendTree.AddChild(animations[4], new Vector2(0, 0)); // Idle

        // Add movement animations if they exist
        if (animations[0] != null) blendTree.AddChild(animations[0], new Vector2(-1, 1)); // MoveUpLeft
        if (animations[1] != null) blendTree.AddChild(animations[1], new Vector2(0, 1));  // MoveUp
        if (animations[2] != null) blendTree.AddChild(animations[2], new Vector2(1, 1));  // MoveUpRight
        if (animations[3] != null) blendTree.AddChild(animations[3], new Vector2(-1, 0)); // MoveLeft
        if (animations[5] != null) blendTree.AddChild(animations[5], new Vector2(1, 0));  // MoveRight
        if (animations[6] != null) blendTree.AddChild(animations[6], new Vector2(-1, -1)); // MoveDownLeft
        if (animations[7] != null) blendTree.AddChild(animations[7], new Vector2(0, -1)); // MoveDown
        if (animations[8] != null) blendTree.AddChild(animations[8], new Vector2(1, -1)); // MoveDownRight

        return blendTree;
    }



    private void AddTransition(AnimatorStateMachine stateMachine, AnimatorState fromState, AnimatorState toState, string param1, float threshold1, bool isLess1)
    {
        AnimatorStateTransition transition = fromState.AddTransition(toState);
        transition.hasExitTime = false;
        transition.hasFixedDuration = true;
        transition.exitTime = 0;
        transition.duration = 0;

        AnimatorConditionMode mode1 = isLess1 ? AnimatorConditionMode.Less : AnimatorConditionMode.Greater;
        transition.AddCondition(mode1, threshold1, param1);
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

    private Sprite GetFirstSpriteFromAnimation(AnimationClip clip)
    {
        // Get all bindings in the animation clip
        if (clip == null)
        {
            return null;
        }
        var bindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);

        // Find the binding that corresponds to the SpriteRenderer
        foreach (var binding in bindings)
        {
            if (binding.type == typeof(SpriteRenderer) && binding.propertyName == "m_Sprite")
            {
                var keyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                if (keyframes.Length > 0)
                {
                    // Return the first sprite
                    return (Sprite)keyframes[0].value;
                }
            }
        }

        return null;
    }

}



