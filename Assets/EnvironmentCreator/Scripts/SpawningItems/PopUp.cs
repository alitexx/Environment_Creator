using UnityEngine;

public class PopUp : MonoBehaviour
{
    private GameObject player;
    private float range;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerCharacter");
        range = GameSettings.interactRange;
    }

    void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);

            if (distance <= range)
            {
                gameObject.SetActive(true);
            }
            else if (distance > range + 1)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
