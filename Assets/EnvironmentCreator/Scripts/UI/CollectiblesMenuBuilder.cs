using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class CollectiblesMenuBuilder : MonoBehaviour
{

    [Header("Folder Paths")]
    public string collectiblesFolderPath = "Assets/EnvironmentCreator/Prefabs/Collectible";

    [Header("Prefabs")]
    public GameObject buttonPrefab;
    public GameObject itemHolderPrefab;
    public Sprite defaultImage;

    [Header("Menu Settings")]
    public Transform menuRoot;
    public int maxItemsPerHolder = 4;

    [Header("Unobtained Button")]
    public Color normalColor = Color.gray;
    public Color hoverColor = Color.white;
    public Color pressedColor = Color.gray;
    public Color itemColor = Color.black;

    private Color normalColor_collected;
    private Color hoverColor_collected;
    private Color pressedColor_collected;
    private Color itemColor_collected = Color.white;

    [Header("Don't Modify")]
    [SerializeField] private GameObject buttonFolder;
    [SerializeField] private GameObject CollectibleMenu;

    private void Start()
    {
        BuildCollectiblesMenu();
    }

    private void BuildCollectiblesMenu()
    {
        List<GameObject> collectibles = LoadCollectibles();
        GameObject currentHolder = CreateNewItemHolder();

        foreach (var collectible in collectibles)
        {
            //Does not count the default
            if (collectible.name == "DefaultCollectible")
            {
                continue;
            }
            if (currentHolder.transform.childCount >= maxItemsPerHolder)
            {
                currentHolder = CreateNewItemHolder();
            }
            GameObject button = Instantiate(buttonPrefab, currentHolder.transform);

            //If this hasn't been set yet, set the collected colors
            if (normalColor_collected == null)
            {
                Button btn = button.GetComponent<Button>();
                ColorBlock colorBlock = btn.colors;
                normalColor_collected = colorBlock.normalColor;
                hoverColor_collected = colorBlock.highlightedColor;
                pressedColor_collected = colorBlock.pressedColor;
            }
            SetupButton(button, collectible);
        }
    }

    private List<GameObject> LoadCollectibles()
    {
        List<GameObject> collectibles = new List<GameObject>();
        DirectoryInfo dir = new DirectoryInfo(collectiblesFolderPath);
        FileInfo[] files = dir.GetFiles("*.prefab");

        foreach (var file in files)
        {
            string path = file.FullName.Substring(Application.dataPath.Length - 6);
            GameObject collectible = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            if (collectible != null)
            {
                collectibles.Add(collectible);
            }
        }
        return collectibles;
    }

    private GameObject CreateNewItemHolder()
    {
        return Instantiate(itemHolderPrefab, menuRoot);
    }

    private void SetupButton(GameObject button, GameObject collectible)
    {
        // Create a new GameObject for the image and set it as a child of the button
        GameObject imageObject = new GameObject("CollectibleImage");
        imageObject.transform.SetParent(button.transform);

        // Add an Image component to the new GameObject
        Image image = imageObject.AddComponent<Image>();
        imageObject.GetComponent<RectTransform>().localScale = new Vector3(0.2f, 0.2f, 1f);

        // Get the SpriteRenderer from the collectible
        SpriteRenderer spriteRenderer = collectible.GetComponent<SpriteRenderer>();

        // Set the sprite for the image
        if (spriteRenderer != null)
        {
            image.sprite = spriteRenderer.sprite;
        }
        else
        {
            // Set a default sprite if the collectible doesn't have a SpriteRenderer
            image.sprite = defaultImage;
        }

        // Optionally, adjust the RectTransform of the new image GameObject
        RectTransform rectTransform = image.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(125, 125); // Adjust the size as needed
        rectTransform.anchoredPosition = Vector2.zero; // Center the image
        RemoveCollectible(button);
    }

    //Called when an item is obtained, changes UI
    public void ObtainCollectible(string imageName)
    {


        GameObject button = FindChildByName(buttonFolder, "Uncollected_" + imageName);
        if(button == null)
        {
            Debug.LogError("Cannot find button with the name ["+ "Uncollected_" + imageName +"].");
        }

        // Change the button's color
        Button btn = button.GetComponent<Button>();
        ColorBlock colorBlock = btn.colors;
        colorBlock.normalColor = normalColor;
        colorBlock.highlightedColor = hoverColor;
        colorBlock.pressedColor = pressedColor;
        btn.colors = colorBlock;

        //All images should be called CollectibleImage
        Image image = FindChildByName(button, "CollectibleImage").GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("Cannot find button with a child named [CollectibleImage]. Please keep the name of the child image as [CollectibleImage] for the code to run properly.");
        }
        image.color = itemColor_collected;
        button.name = "Collected_" + imageName;
    }

    //called at start of the game. could also be called if a collectible is removed from the player
    public void RemoveCollectible(GameObject button)
    {
        // Change the button's color
        Button btn = button.GetComponent<Button>();
        ColorBlock colorBlock = btn.colors;
        colorBlock.normalColor = normalColor;
        colorBlock.highlightedColor = hoverColor;
        colorBlock.pressedColor = pressedColor;
        btn.colors = colorBlock;

        // Change the image's color
        Image image = FindChildByName(button, "CollectibleImage").GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("Cannot find button with a child named [CollectibleImage]. Please keep the name of the child image as [CollectibleImage] for the code to run properly.");
        }
        image.color = itemColor;
        button.name = "Uncollected_" + image.sprite.name;
    }

    //For obtaining a collectible
    public static GameObject FindChildByName(GameObject parent, string name)
    {
        if (parent.name == name)
        {
            return parent;
        }

        foreach (Transform child in parent.transform)
        {
            GameObject result = FindChildByName(child.gameObject, name);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    //Simple button functions
    //If you wanted to add an opening/closing animation of some sort, it would go here.
    public void OpenCollectibleMenu()
    {
        CollectibleMenu.SetActive(true);
        PauseManager.FreezeAllMovingObject(true); //Freeze NPCs and Players
    }

    public void CloseCollectibleMenu()
    {
        CollectibleMenu.SetActive(false);
        PauseManager.FreezeAllMovingObject(false); //Unfreeze NPCs and Players
    }
}
