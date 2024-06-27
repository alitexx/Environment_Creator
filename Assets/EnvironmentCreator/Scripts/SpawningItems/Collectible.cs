using UnityEngine;

/*
 * 
 * This is all placeholder code, but it can be edited to better fit your needs!
 * Currently it only knows the item name and if it is collected.
 * 
 * 
 * This script is attached to all ITEM objects in the scene.
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */

public class Item : MonoBehaviour
{
    // Simple properties that can be extended
    public string itemName;
    //public string itemDescription;
    public bool isCollectible = true;

    // This function is called when the script instance is being loaded
    void Awake()
    {
        itemName = transform.name;
        Debug.Log(itemName + " has been created.");
    }

    // Called every frame. Checks if the item has been clicked, and if so, tries to collect the item
    private void Update()
    {
        //This is left click. If you want to change it to a key, just do KeyCode.X and replace X with whatever key you'd like!
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Collect();
        }
    }



    // This function can be called to "collect" the item
    public void Collect()
    {
        if (isCollectible)
        {
            Debug.Log(itemName + " collected.");
            // Additional logic for collecting the item can go here
            Destroy(gameObject); // Remove the item from the scene
        }
        else
        {
            Debug.Log(itemName + " cannot be collected.");
        }
    }

    // Optional: Function to describe the item (could be used in a UI later)
    public string GetDescription()
    {
        return "This is a " + itemName;
    }
}
