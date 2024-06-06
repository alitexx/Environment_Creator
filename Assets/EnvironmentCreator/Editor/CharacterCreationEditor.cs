using UnityEditor;
using UnityEditor.U2D.Animation;
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
                    animations[index] = (AnimationClip)EditorGUILayout.ObjectField(animations[index], typeof(AnimationClip), false);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            walkSpeed = EditorGUILayout.FloatField("Walk Speed", walkSpeed);
            reach = EditorGUILayout.FloatField("Reach", reach);
            startPosition = (RectTransform)EditorGUILayout.ObjectField("Start Position", startPosition, typeof(RectTransform), true);
        }
    }

    private void CreatePlayerCharacterPrefab()
    {
        string prefabPath = "Assets/EnvironmentCreator/Prefabs/Player Character/PlayerCharacter.prefab";
        playerCharacterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (playerCharacterPrefab == null)
        {
            playerCharacterPrefab = new GameObject("PlayerCharacter");
            PlayerMovement playerMovement = playerCharacterPrefab.AddComponent<PlayerMovement>();
            Animator animator = playerCharacterPrefab.AddComponent<Animator>();
            CharacterData characterData = playerCharacterPrefab.AddComponent<CharacterData>();

            // Assign default values
            characterData.walkSpeed = walkSpeed;
            characterData.reach = reach;
            characterData.animations = animations;
            characterData.startPosition = startPosition;

            PrefabUtility.SaveAsPrefabAsset(playerCharacterPrefab, prefabPath);
            DestroyImmediate(playerCharacterPrefab);
        }
        else
        {
            CharacterData characterData = playerCharacterPrefab.GetComponent<CharacterData>();
            if (characterData != null)
            {
                characterData.walkSpeed = walkSpeed;
                characterData.reach = reach;
                characterData.animations = animations;
                characterData.startPosition = startPosition;
            }
        }
    }

}
