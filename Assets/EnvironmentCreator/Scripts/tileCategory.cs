using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;


public enum TileCategory
{
    Default,

    //Enemies spawn on these tiles
    EnemySpawner,

    //Items spawn on these tiles
    ItemSpawner,

    //Tomes spawn on these tiles
    TomeSpawner
,
    //NewCategory spawn on these tiles (USER DEFINED TILE CATEGORY)
    NewCategorySpawner
,
    //Testing spawn on these tiles (USER DEFINED TILE CATEGORY)
    TestingSpawner
}



public class tileCategory : MonoBehaviour
{
    public TileCategory TileCategory;
    public bool CanCollide;

    private Spawner enemySpawner = new EnemySpawner();
    private Spawner itemSpawner = new ItemSpawner();
    private Spawner tomeSpawner = new TomeSpawner();

    private Spawner newcategorySpawner = new NewCategorySpawner();


    private Spawner testingSpawner = new TestingSpawner();

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
            case TileCategory.EnemySpawner:
                enemySpawner.Spawn(objSpawnName, transform.position);
                break;
            case TileCategory.ItemSpawner:
                itemSpawner.Spawn(objSpawnName, transform.position);
                break;
            case TileCategory.TomeSpawner:
                tomeSpawner.Spawn(objSpawnName, transform.position);
                break;
            case TileCategory.NewCategorySpawner:
                newcategorySpawner.Spawn(objSpawnName, transform.position);
                break;
            case TileCategory.TestingSpawner:
                testingSpawner.Spawn(objSpawnName, transform.position);
                break;
            default:
                break;
        }
    }
}
