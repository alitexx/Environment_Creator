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
            if (currentHolder.transform.childCount >= maxItemsPerHolder)
            {
                currentHolder = CreateNewItemHolder();
            }

            GameObject button = Instantiate(buttonPrefab, currentHolder.transform);
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
        RemoveCollectible(button, image);
    }

    //Called when an item is obtained, changes UI
    public void ObtainCollectible(GameObject collectible)
    {

    }

    //called at start of the game. could also be called if a collectible is removed from the player
    public void RemoveCollectible(GameObject button, Image image)
    {
        
    }

}
