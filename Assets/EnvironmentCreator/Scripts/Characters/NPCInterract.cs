using UnityEngine;
using TMPro;

public class NPCInteract : MonoBehaviour
{
    public GameObject popup; //the prefab
    public string characterName;
    public string popuptext;
    public float textSpeedMultiplier;
    private GameObject player;
    private GameObject popupInstance;
    private float range;
    private KeyCode interactKey;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerCharacter");
        range = GameSettings.interactRange;
        interactKey = GameSettings.interactKey;
        //Weird cheat way to check if textspeedmultiplier is null or not
        if(textSpeedMultiplier == 0f)
        {
            textSpeedMultiplier = 1f;
        }
    }

    private void Update()
    {
        // Check the distance between the player and this NPC
        float distance = Vector3.Distance(player.transform.position, transform.position);

        // If within distance and the player presses the interaction key, toggle its active state
        if (distance <= range && Input.GetKeyDown(interactKey))
        {
            //popupInstance.SetActive(!popupInstance.activeSelf);
            popupInstance = Instantiate(popup, transform.position, Quaternion.identity);

            //Uncomment this code if you want the text box to appear on top of the NPC
            //popupInstance.transform.position = gameObject.transform.position;

            TextBubble textTyper = popupInstance.GetComponent<TextBubble>();
            //send pop up text to text typer for it to display
            textTyper.Initialize(popuptext, textSpeedMultiplier);

            PauseManager.FreezeAllMovingObject(true); //Freeze NPCs and Players
        }
    }
}
