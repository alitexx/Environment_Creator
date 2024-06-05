using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;


public enum TileCategory
{
    Default,

    //NPCS spawn on these tiles
    NPCSpawner,

    //Items spawn on these tiles
    ItemSpawner,

    //Tomes spawn on these tiles
    TomeSpawner
}



public class tileCategory : MonoBehaviour
{
    public TileCategory TileCategory;
    public bool CanCollide;

    private Spawner npcSpawner = new NPCSpawner();
    private Spawner itemSpawner = new ItemSpawner();
    private Spawner tomeSpawner = new TomeSpawner();

    //User defined spawners above

    void Start()
    {
        //// Initialize spawners
        //enemySpawner = new EnemySpawner();
        //itemSpawner = new ItemSpawner();
        //tomeSpawner = new TomeSpawner();

        RunTileCategory("");
    }

    public void SetValuesWhenPlaced(TileCategory tileCategory, bool canCollide, string objSpawnName)
    {
        this.CanCollide = canCollide;
        // If we should be colliding and there is not already a collider
        // NOTE: Maybe in the future, make a box collider variable.
        if (CanCollide && gameObject.GetComponent<BoxCollider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
        else if (!CanCollide && gameObject.GetComponent<BoxCollider2D>() != null)
        {
            Destroy(gameObject.GetComponent<BoxCollider2D>());
        }
        if (tileCategory == this.TileCategory)
        {
            // There may be instances where we want to change the values on the fly.
            //This is here so we don't redo code? Putting this here in case something changes in the future
            return;
        }
        this.TileCategory = tileCategory;
        RunTileCategory(objSpawnName);
    }

    // A very poorly named function that houses a switch statement, the function called depending on the tile category.

    private void RunTileCategory(string objSpawnName)
    {
        switch (TileCategory)
        {
            case TileCategory.Default:
                break;
            case TileCategory.NPCSpawner:
                npcSpawner.Spawn(objSpawnName, transform.position);
                break;
            case TileCategory.ItemSpawner:
                itemSpawner.Spawn(objSpawnName, transform.position);
                break;
            case TileCategory.TomeSpawner:
                tomeSpawner.Spawn(objSpawnName, transform.position);
                break;
            default:
                break;
        }
    }
}
