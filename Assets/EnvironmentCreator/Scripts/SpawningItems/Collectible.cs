using UnityEngine;

public class Collectible : MonoBehaviour
{
    // Simple properties that can be extended
    public string itemName;
    //public string itemDescription;
    public bool isCollectible = true;
    private CollectiblesMenuBuilder collectiblesMenu;

    private GameObject player;
    private float range;

    // This function is called when the script instance is being loaded
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("PlayerCharacter");
        range = GameSettings.interactRange;

        itemName = transform.name;
        collectiblesMenu = GameObject.FindGameObjectWithTag("CollectiblesMenu").GetComponent<CollectiblesMenuBuilder>();
    }

    // Called every frame. Checks if the item has been clicked, and if so, tries to collect the item
    private void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);

            if (distance <= range && Input.GetKeyDown(GameSettings.interactKey))
            {
                Collect();
            }
        }
    }

    // This function can be called to "collect" the item
    public void Collect()
    {
        if (isCollectible)
        {
            //If you wanted to add some form of particle effects, etc. it would go here!


            //Takes the sprite's name to compare to all other buttons
            collectiblesMenu.ObtainCollectible(gameObject.GetComponent<SpriteRenderer>().sprite.name); // Notify the menu that the item is collected
            Destroy(gameObject); // Remove the item from the scene
        }
        else
        {
            Debug.Log(itemName + " cannot be collected.");
        }
    }
}
