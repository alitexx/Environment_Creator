using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Tome : MonoBehaviour
{
    public string tomeContent = "This is the content of the tome.";
    public static GameObject uiPanel; // Reference to the UI panel (Canvas)
    public Text contentText; // Reference to the text component in the panel

    private GameObject uiPanelPrefab;

    void Start()
    {
        //Looks for UI Panel in the scene. If it is not in the scene, creates it
        uiPanel = GameObject.FindGameObjectWithTag("UI");
        
        if (uiPanel != null)
        {
            // This code will break if Tome Text is renamed.
            // If you are getting an error about this line of code, it is likely because Tome Text has been moved.
            // Either move it back, rename the game object, or change the "Tome Text" parameter to the new name for the game object.
            contentText = uiPanel.transform.Find("Tome Text").GetComponent<Text>();
            uiPanel.SetActive(false); // Hide the UI panel initially
        } else
        {
            //create a UI panel from a prefab
            uiPanelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/EnvironmentCreator/Prefabs/UI/Tome UI.prefab");
            if (uiPanelPrefab == null)
            {
                Debug.LogWarning("There is no prefab for the UI [Tome UI]. The UI for tomes can be found in EnvironmentCreator/Prefabs/UI and must be named [Tome UI].");
            }
            // Instantiate the object at the valid position
            uiPanel = Instantiate(uiPanelPrefab);
            uiPanel.transform.parent = GameObject.Find("UI").transform;
        }
    }

    // Opens UI
    // To find the close UI script, go to EnvironmentCreator/Scripts/TomeUI
    void OnMouseDown()
    {
        if (uiPanel != null && contentText != null && uiPanel.activeSelf == false)
        {
            contentText.text = tomeContent; // Set the text box content
            uiPanel.SetActive(true); // Show the UI panel
        }
    }
}

