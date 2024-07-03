using UnityEngine;
using UnityEditor;
using System.Linq;

public static class TagHelper
{
    public static void AddTag(string tag, GameObject assignTag = null)
    {
        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if ((asset != null) && (asset.Length > 0))
        {
            SerializedObject so = new SerializedObject(asset[0]);
            SerializedProperty tags = so.FindProperty("tags");

            for (int i = 0; i < tags.arraySize; ++i)
            {
                if (tags.GetArrayElementAtIndex(i).stringValue == tag)
                {
                    return;     // Tag already present, nothing to do.
                }
            }

            tags.InsertArrayElementAtIndex(tags.arraySize);
            tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
            so.ApplyModifiedProperties();
            so.Update();

            //I do this here because there were issues with the assignment happening before the tag was implemented
            if(assignTag != null)
            {
                assignTag.tag = tag;
            }
        }
    }

    public static bool TagExists(string tag)
    {
        return UnityEditorInternal.InternalEditorUtility.tags.Contains(tag);
    }
}
