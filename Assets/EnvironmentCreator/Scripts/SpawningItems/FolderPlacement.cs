using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FolderPlacement : MonoBehaviour
{
    // Parent GameObject, technically the one we are currently editing
    public GameObject parentOBJ;
    private GameObject childWithTag;

    public void PlaceInFolder(string folderName)
    {
        parentOBJ = GameObject.FindGameObjectWithTag("Editing");
        if (parentOBJ != null)
        {
            try
            {
                childWithTag = FindChildWithTag(parentOBJ.transform, folderName);
            }
            catch
            {
                Debug.LogError("Tag [" + folderName + "] is not defined. Please define this tag in the editor and try again.");
            }

            if (childWithTag != null)
            {
                //Found child with tag!!
                this.gameObject.transform.parent = childWithTag.transform;
            }
            else
            {
                //Make a new game object 
                childWithTag = new GameObject(folderName);
                childWithTag.transform.parent = parentOBJ.transform;
                this.gameObject.transform.parent = childWithTag.transform;
            }
        } else
        {
            Debug.LogWarning("Cannot find parent object.");
        }

    }

    GameObject FindChildWithTag(Transform parent, string tag)
    {
        // Iterate through all children of the parent transform
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
            {
                return child.gameObject;
            }

            // I don't think we need recursion here, but I may be wrong. Leaving the code just in case.
            // Recursively search in child's children
            //GameObject result = FindChildWithTag(child, tag);
            //if (result != null)
            //{
            //    return result;
            //}
        }

        // No child with the specified tag was found
        return null;
    }
}
