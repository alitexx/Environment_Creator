using UnityEngine;
using TMPro;

public class NPCInteract : MonoBehaviour
{
    public GameObject popup; //the prefab
    public string characterName;
    public string popuptext;
    private GameObject player;
    private GameObject popupInstance;
    private float range;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("MainCharacter");
        range = player.GetComponent<CharacterData>().reach;
    }

    private void Update()
    {
        // Check the distance between the player and this NPC
        float distance = Vector3.Distance(player.transform.position, transform.position);
        // If within distance and the player presses the interaction key, toggle its active state
        if (distance <= range && Input.GetKeyDown(GameSettings.interactKey))
        {
            //popupInstance.SetActive(!popupInstance.activeSelf);
            popupInstance = Instantiate(popup, transform.position, Quaternion.identity);
            TextBubble textTyper = popupInstance.GetComponent<TextBubble>();
            //send pop up text to text typer for it to display
            textTyper.Initialize(popuptext);
        }
    }
}
