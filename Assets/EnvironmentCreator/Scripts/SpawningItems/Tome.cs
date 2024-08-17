using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tome : MonoBehaviour
{
    public string tomeContent = "This is the content of the tome.";
    public GameObject uiPanel; // Reference to the UI panel (Canvas)
    public TextMeshProUGUI contentText; // Reference to the text component in the panel

    private GameObject player;
    private float range;
    private SpriteRenderer spriteRenderer;
    private GameObject outlineOBJ;
    private SpriteRenderer outlineSpriteRenderer;
    private Material outlineMaterial;
    //Users can define this
    public Color outlineColor;

    private GameObject uiPanelPrefab;

    void Start()
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


        //If the user has not attached a different UI, look for UI
        if (uiPanel == null)
        {
            uiPanel = GameObject.FindGameObjectWithTag("UI");
        }
        if (contentText == null)
        {
            contentText = uiPanel.transform.Find("Tome Text").GetComponent<TextMeshProUGUI>();
        }
        //Looks for UI Panel in the scene. If it is not in the scene, creates it
        if (uiPanel != null)
        {

            uiPanel.SetActive(false); // Hide the UI panel initially
        }
        else
        {
            //create a UI panel from a prefab
            uiPanelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/EnvironmentCreator/Prefabs/UI/Tome UI.prefab");
            if (uiPanelPrefab == null)
            {
                Debug.LogWarning("There is no prefab for the UI [Tome UI]. The UI for tomes can be found in EnvironmentCreator/Prefabs/UI and must be named [Tome UI].");
            }
            // Instantiate the object at the valid position
            uiPanel = Instantiate(uiPanelPrefab);
            try
            {
                uiPanel.transform.SetParent(GameObject.Find("UI").transform);
            }
            catch
            {
                uiPanel.transform.SetParent(new GameObject("UI").transform);
            }
            contentText = uiPanel.transform.Find("Tome Text").GetComponent<TextMeshProUGUI>();
            uiPanel.SetActive(false);

        }


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
                    OpenMenu();
                }
            }
            else if (distance > range)
            {
                outlineOBJ.SetActive(false);
            }

        }
    }

    // Opens UI
    // To find the close UI script, go to EnvironmentCreator/Scripts/TomeUI
    void OpenMenu()
    {
        if (contentText == null)
        {
            Debug.LogError("Text component not found in Tome Text or any of its children!");
            return;
        }
        if (uiPanel != null && uiPanel.activeSelf == false)
        {
            uiPanel.SetActive(true); // Show the UI panel
            contentText.text = tomeContent; // Set the text box content
            PauseManager.FreezeAllMovingObject(true); //Freeze NPCs and Players
        }
    }
}


