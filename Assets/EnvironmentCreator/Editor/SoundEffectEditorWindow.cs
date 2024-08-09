using UnityEngine;
using UnityEditor;

public class SoundEffectEditorWindow : EditorWindow
{
    private string prefabpath = "Assets/EnvironmentCreator/Prefabs";

    private AudioClip selectedClip;
    private bool isAmbient;
    private bool isTriggered;
    private bool loop;
    private float volume = 1f;
    private float interval;
    private string[] triggerTypes = new string[] { "Interaction", "Colliding", "Timed" };
    private int selectedTrigger;
    private GameObject selectedObject;

    [MenuItem("Window/Environment Creator/Sound Editor")]
    public static void ShowWindow()
    {
        GetWindow<SoundEffectEditorWindow>("Sound Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Sound Settings", EditorStyles.boldLabel);

        selectedObject = (GameObject)EditorGUILayout.ObjectField("Game Object", selectedObject, typeof(GameObject), true);

        selectedClip = (AudioClip)EditorGUILayout.ObjectField("Sound", selectedClip, typeof(AudioClip), false);

        if (selectedClip != null)
        {
            isAmbient = GUILayout.Toggle(isAmbient, "Ambient");
            if (isAmbient)
            {
                loop = GUILayout.Toggle(loop, "Loop");
                volume = EditorGUILayout.Slider("Volume", volume, 0f, 1f);
            }

            isTriggered = GUILayout.Toggle(isTriggered, "Triggered/SFX");
            if (isTriggered)
            {
                loop = GUILayout.Toggle(loop, "Loop");
                volume = EditorGUILayout.Slider("Volume", volume, 0f, 1f);
                selectedTrigger = EditorGUILayout.Popup("Trigger Type", selectedTrigger, triggerTypes);

                if (selectedTrigger == 2) // Timed
                {
                    interval = EditorGUILayout.FloatField("Interval (seconds)", interval);
                }
            }

            if (GUILayout.Button("Update only Game Object"))
            {
                UpdateGameObject();
            }

            if (GUILayout.Button("Update all Game Objects of this type"))
            {
                UpdatePrefab();
            }
        }
    }

    private GameObject UpdateGameObject()
    {
        if (selectedObject == null)
        {
            Debug.LogError("Please enter a game object to attach the sound clip to.");
            return null;
        }
        if (selectedClip == null)
        {
            Debug.LogError("Please enter a clip to attach to a game object.");
            return null;
        }
        AudioSource audioSource = ComponentHelper.GetOrAddComponent<AudioSource>(selectedObject);

        audioSource.clip = selectedClip;
        audioSource.loop = loop;
        audioSource.volume = volume;

        if (isTriggered)
        {
            audioSource.playOnAwake = false;
            TriggeredSound triggeredSound = ComponentHelper.GetOrAddComponent<TriggeredSound>(selectedObject);
            triggeredSound.triggerType = (TriggeredSound.TriggerType)selectedTrigger;
            triggeredSound.interval = interval;
            if ((TriggeredSound.TriggerType)selectedTrigger == TriggeredSound.TriggerType.Colliding)
            {
                PolygonCollider2D collider2D = ComponentHelper.GetOrAddComponent<PolygonCollider2D>(selectedObject);
                collider2D.isTrigger = true;
                //Debug.Log("Game Object updated! Please check it's BoxCollider2D component and shape it to the sprite.");
            }
        }
        return selectedObject;
    }

    private void UpdatePrefab()
    {
        //This should technically do nothing, but it ensures that the game object was updated before assinging it to a prefab
        selectedObject = UpdateGameObject();
        if (selectedObject == null)
        {
            Debug.LogError("Please enter a game object to attach the sound clip to.");
            return;
        }

        string fullPrefabPath = $"{prefabpath}/Game Objects with Sound Effects/{selectedObject.name}.prefab";

        // Check if the selected object is a prefab instance
        GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(selectedObject);

        if (prefab != null)
        {
            //Debug.Log($"The selected object is already a prefab: {AssetDatabase.GetAssetPath(prefab)}");
            PrefabUtility.SaveAsPrefabAsset(selectedObject, fullPrefabPath);
            Debug.Log($"Updated existing prefab at {fullPrefabPath}");
        }
        else
        {
            // Check if the prefab already exists in the target folder
            if (AssetDatabase.LoadAssetAtPath<GameObject>(fullPrefabPath) != null)
            {
                Debug.Log($"A prefab with the name {selectedObject.name} already exists at {fullPrefabPath}");
                return;
            }

            // Ensure the target folder exists
            if (!AssetDatabase.IsValidFolder($"{prefabpath}/Game Objects with Sound Effects"))
            {
                AssetDatabase.CreateFolder(prefabpath, "Game Objects with Sound Effects");
            }

            // Create the prefab
            PrefabUtility.SaveAsPrefabAsset(selectedObject, fullPrefabPath);
            Debug.Log($"Prefab created at {fullPrefabPath}");
        }
    }

}
