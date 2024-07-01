using UnityEngine;

public class Collectible : MonoBehaviour
{
    // Simple properties that can be extended
    public string itemName;
    //public string itemDescription;
    public bool isCollectible = true;
    private CollectiblesMenuBuilder collectiblesMenu;

    // This function is called when the script instance is being loaded
    void Awake()
    {
        itemName = transform.name;
        collectiblesMenu = GameObject.FindGameObjectWithTag("CollectiblesMenu").GetComponent<CollectiblesMenuBuilder>();
    }

    // Called every frame. Checks if the item has been clicked, and if so, tries to collect the item
    private void Update()
    {
        //Use the keycode defined in GameSettings
        if (Input.GetKeyDown(GameSettings.interactKey))
        {
            Collect();
        }
    }

    // This function can be called to "collect" the item
    public void Collect()
    {
        if (isCollectible)
        {
            //If you wanted to add some form of particle effects, etc. it would go here!
            SpriteRenderer test1 = gameObject.GetComponent<SpriteRenderer>();
            string test2 = test1.sprite.name;
            //Takes the sprite's name to compare to all other buttons
            collectiblesMenu.ObtainCollectible(test2); // Notify the menu that the item is collected
            Destroy(gameObject); // Remove the item from the scene
        }
        else
        {
            Debug.Log(itemName + " cannot be collected.");
        }
    }
}
