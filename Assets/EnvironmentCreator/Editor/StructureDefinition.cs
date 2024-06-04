using UnityEngine;
using UnityEditor;
using System.IO;


public class StructureDefinition : EditorWindow
{
    private string objectName = "New Structure";
    private int width = 1;
    private int height = 1;
    private GameObject selectedObject;

    [MenuItem("Window/Environment Creator/Structure Definition")]
    public static void ShowWindow()
    {
        GetWindow<StructureDefinition>("Structure Definition");
    }

    void OnGUI()
    {
        GUILayout.Label("Define a New Structure", EditorStyles.boldLabel);

        selectedObject = (GameObject)EditorGUILayout.ObjectField("Game Object", selectedObject, typeof(GameObject), true);

        objectName = EditorGUILayout.TextField("Object Name", objectName);

        width = EditorGUILayout.IntField("Width (Tiles)", width);
        height = EditorGUILayout.IntField("Height (Tiles)", height);

        if (GUILayout.Button("Save Structure"))
        {
            if (selectedObject != null)
            {
                SaveStructure();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please select a game object.", "OK");
            }
        }
    }

    void SaveStructure()
    {
        // Save the prefab
        string prefabPath = $"Assets/EnvironmentCreator/Prefabs/Tiles/{objectName}.prefab";
        if (!Directory.Exists("Assets/EnvironmentCreator/Prefabs/Tiles"))
        {
            Directory.CreateDirectory("Assets/EnvironmentCreator/Prefabs/Tiles");
        }
        PrefabUtility.SaveAsPrefabAsset(selectedObject, prefabPath);

        // Capture the screenshot
        string imagePath = $"Assets/EnvironmentCreator/Images/{objectName}.png";
        if (!Directory.Exists("Assets/EnvironmentCreator/Images"))
        {
            Directory.CreateDirectory("Assets/EnvironmentCreator/Images");
        }
        CaptureScreenshot(imagePath);
    }

    void CaptureScreenshot(string path)
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            Debug.LogError("No main camera found!");
            return;
        }

        RenderTexture rt = new RenderTexture(256, 256, 24);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(256, 256, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();
    }
}

