using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public abstract class Spawner
{
    protected string prefabFolder;

    public Spawner(string prefabFolder)
    {
        this.prefabFolder = prefabFolder;
    }

    public void Spawn(string prefabName, Vector3 spawnPosition)
    {
        string path = "Assets/EnvironmentCreator/Prefabs/" + prefabFolder + "/" + (string.IsNullOrEmpty(prefabName) ? GetDefaultPrefab() : prefabName) + ".prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        //GameObject prefab = Resources.Load<GameObject>(path);

        if (prefab != null)
        {
            GameObject spawnedItem = Object.Instantiate(prefab, spawnPosition, Quaternion.identity);
            spawnedItem.name = prefabName;
            spawnedItem.GetComponent<FolderPlacement>().PlaceInFolder(prefabFolder);

        }
        else
        {
            Debug.LogWarning($"Prefab [{prefabName}] not found in folder [{prefabFolder}].");
        }
    }
    
    protected abstract string GetDefaultPrefab();
}

public class NPCSpawner : Spawner
{
    public NPCSpawner() : base("NPC") { }

    protected override string GetDefaultPrefab()
    {
        return "DefaultNPC";
    }
}

public class ItemSpawner : Spawner
{
    public ItemSpawner() : base("Item") { }

    protected override string GetDefaultPrefab()
    {
        return "DefaultItem";
    }
}

public class TomeSpawner : Spawner
{
    public TomeSpawner() : base("Tome") { }

    protected override string GetDefaultPrefab()
    {
        return "DefaultTome";
    }
}



//User defined Spawners go here:



