using UnityEngine;
using UnityEditor;

public class SoundEffectEditorWindow : EditorWindow
{
    private AudioClip selectedClip;
    private bool isAmbient;
    private bool isTriggered;
    private bool loop;
    private float volume = 1f;
    private float interval;
    private string[] triggerTypes = new string[] { "Interaction", "Colliding", "Timed" };
    private int selectedTrigger;
    private GameObject selectedObject;

    [MenuItem("Window/Environment Creator/Sound Effect Editor")]
    public static void ShowWindow()
    {
        GetWindow<SoundEffectEditorWindow>("Sound Effect Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Sound Effect Settings", EditorStyles.boldLabel);

        selectedClip = (AudioClip)EditorGUILayout.ObjectField("Sound Effect", selectedClip, typeof(AudioClip), false);

        if (selectedClip != null)
        {
            isAmbient = GUILayout.Toggle(isAmbient, "Ambient");
            if (isAmbient)
            {
                loop = GUILayout.Toggle(loop, "Loop");
                volume = EditorGUILayout.Slider("Volume", volume, 0f, 1f);
            }

            isTriggered = GUILayout.Toggle(isTriggered, "Triggered");
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

            selectedObject = (GameObject)EditorGUILayout.ObjectField("Game Object", selectedObject, typeof(GameObject), true);

            if (GUILayout.Button("Update Game Object"))
            {
                UpdateGameObject();
            }

            if (GUILayout.Button("Update Prefab"))
            {
                UpdatePrefab();
            }
        }
    }

    private void UpdateGameObject()
    {
        if (selectedObject != null && selectedClip != null)
        {
            AudioSource audioSource = selectedObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = selectedObject.AddComponent<AudioSource>();
            }

            audioSource.clip = selectedClip;
            audioSource.loop = loop;
            audioSource.volume = volume;

            if (isTriggered)
            {
                audioSource.playOnAwake = false;
                TriggeredSound triggeredSound = selectedObject.GetComponent<TriggeredSound>();
                if (triggeredSound == null)
                {
                    triggeredSound = selectedObject.AddComponent<TriggeredSound>();
                }
                triggeredSound.triggerType = (TriggeredSound.TriggerType)selectedTrigger;
                triggeredSound.interval = interval;
                if((TriggeredSound.TriggerType)selectedTrigger == TriggeredSound.TriggerType.Colliding)
                {
                    BoxCollider2D boxcollider;
                    try
                    {
                        boxcollider = selectedObject.GetComponent<BoxCollider2D>();
                        boxcollider.isTrigger = true;
                    }
                    catch
                    {
                        boxcollider = selectedObject.AddComponent<BoxCollider2D>();
                        boxcollider.isTrigger = true;
                    }
                }
            }
        }
    }

    private void UpdatePrefab()
    {
        // Implement prefab update logic here
    }
}
