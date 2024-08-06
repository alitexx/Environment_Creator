using UnityEditor;
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
    private SpriteRenderer spriteRenderer;
    private GameObject outlineOBJ;
    private SpriteRenderer outlineSpriteRenderer;
    private Material outlineMaterial;
    //Users can define this
    public Color outlineColor;

    // This function is called when the script instance is being loaded
    void Awake()
    {
        
        outlineMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/EnvironmentCreator/URP Assets/OutlineMaterial.mat");

        player = GameObject.FindGameObjectWithTag("PlayerCharacter");
        range = GameSettings.interactRange;

        spriteRenderer = GetComponent<SpriteRenderer>();

        // Create the outline object
        outlineOBJ = new GameObject("Outline");
        outlineOBJ.transform.SetParent(transform, false); // Use world position stays false
        outlineOBJ.transform.localPosition = Vector3.zero;

        // Add a SpriteRenderer to the outline object
        outlineSpriteRenderer = outlineOBJ.AddComponent<SpriteRenderer>();
        outlineSpriteRenderer.sprite = spriteRenderer.sprite;
        outlineSpriteRenderer.color = outlineColor;
        outlineSpriteRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
        outlineSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 1; // Render behind the original sprite
        outlineSpriteRenderer.material = outlineMaterial;

        // Initially disable the outline
        outlineOBJ.SetActive(false);

        itemName = transform.name;
        collectiblesMenu = GameObject.FindGameObjectWithTag("CollectiblesMenu").GetComponent<CollectiblesMenuBuilder>();
    }

    // Called every frame. Checks if the item has been clicked, and if so, tries to collect the item
    private void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);

            if (distance <= range)
            {
                outlineOBJ.SetActive(true);
                if (Input.GetKeyDown(GameSettings.interactKey))
                {
                    Collect();
                }
            } else if (distance > range)
            {
                outlineOBJ.SetActive(false);
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
