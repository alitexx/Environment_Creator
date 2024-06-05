using System.Collections.Generic;  // Required for using Dictionary
using UnityEngine;

public static class tileCategoryManagement
{
    //Key = The name of the tile
    //Value = the folder

    // This is where we can find what folder is associated with the tile category
    private static Dictionary<string, string> TCfolderDict = new Dictionary<string, string>();

    // Function to set a value in the dictionary
    public static void SetValue(string key, string value)
    {
        //If there is nothing in this list, first set the default values
        if (TCfolderDict.ContainsKey(key))
        {
            TCfolderDict[key] = value;
        }
        else
        {
            TCfolderDict.Add(key, value);
        }
    }

    //Should only be empty at the very beginning since default must always be there
    public static void checkIfEmpty()
    {
        if (TCfolderDict.Count == 0)
        {
            SetValue("Default", "");
            SetValue("Item", "Items");
            SetValue("Tome", "Tomes");
            //NOTE TO KATIE: COME BACK HERE AND SET THIS IF MAKING A NEW TILE CATEGORY!!!
        }
    }

    // Function to get a value from the dictionary
    public static string GetValue(string key)
    {
        if (TCfolderDict.TryGetValue(key, out string value))
        {
            Debug.Log($"Retrieved value: {value} for key: {key}");
            return value;
        }
        else
        {
            //Debug.LogError("Cannot find TileCategory named [" + key + "] in dictionary.");
            return null;
        }
    }

    // Function to delete a value from the dictionary
    public static void DeleteValue(string key)
    {
        if (TCfolderDict.Remove(key))
        {
            Debug.Log($"Removed key: {key} from the dictionary.");
        }
        else
        {
            Debug.Log($"Key: {key} not found in the dictionary.");
        }
    }
}
