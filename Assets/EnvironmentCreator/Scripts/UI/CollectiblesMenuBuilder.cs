using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class CollectiblesMenuBuilder : MonoBehaviour
{
    public string collectiblesFolderPath = "Assets/EnvironmentCreator/Prefabs/Collectible";
    public GameObject buttonPrefab;
    public GameObject itemHolderPrefab;
    public GameObject defaultImagePrefab;
    public Transform menuRoot;
    public int maxItemsPerHolder = 4;

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
        Image image = button.GetComponentInChildren<Image>();
        SpriteRenderer spriteRenderer = collectible.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            image.sprite = spriteRenderer.sprite;
        }
        else
        {
            GameObject defaultImage = Instantiate(defaultImagePrefab, button.transform);
            image = defaultImage.GetComponent<Image>();
            // You can set the default sprite here if needed
        }
    }
}
