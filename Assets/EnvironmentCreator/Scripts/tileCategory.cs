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

    //Collectibles spawn on these tiles
    CollectibleSpawner,

    //Tomes spawn on these tiles
    TomeSpawner,

    //PopUp spawn on these tiles (USER DEFINED TILE CATEGORY)
    PopUpSpawner,

    //Teleport spawn on these tiles (USER DEFINED TILE CATEGORY)
    TeleportSpawner
}



public class tileCategory : MonoBehaviour
{
    private TileCategory TileCategory;
    private bool CanCollide;

    private Spawner npcSpawner = new NPCSpawner();
    private Spawner collectibleSpawner = new CollectibleSpawner();
    private Spawner tomeSpawner = new TomeSpawner();
    private Spawner popupSpawner = new PopUpSpawner();
    private Spawner teleportSpawner = new TeleportSpawner();

    //User defined spawners above

    void Start()
    {
        //// Initialize spawners
        //enemySpawner = new EnemySpawner();
        //collectibleSpawner = new CollectibleSpawner();
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
            Debug.Log("Added a collider to the placed game object. Please check it's BoxCollider2D component and shape it to the sprite.");
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
                npcSpawner.Spawn(objSpawnName, transform.position, this.gameObject);
                break;
            case TileCategory.CollectibleSpawner:
                collectibleSpawner.Spawn(objSpawnName, transform.position, this.gameObject);
                break;
            case TileCategory.TomeSpawner:
                tomeSpawner.Spawn(objSpawnName, transform.position, this.gameObject);
                break;
            case TileCategory.PopUpSpawner:
                popupSpawner.Spawn(objSpawnName, transform.position, this.gameObject);
                break;
            case TileCategory.TeleportSpawner:
                teleportSpawner.Spawn(objSpawnName, transform.position, this.gameObject);
                break;
            default:
                break;
        }
    }
}
