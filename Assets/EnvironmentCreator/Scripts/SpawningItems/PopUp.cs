using UnityEngine;

public class PopUp : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;  // Assign the sprite renderer in the inspector
    private GameObject player;
    private float range;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerCharacter");
        range = player.GetComponent<CharacterData>().reach;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);

            if (distance <= range)
            {
                SetActive(true);
            }
            else if (distance > range + 1)
            {
                SetActive(false);
            }
        }
    }

    private void SetActive(bool isActive)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = isActive;
        }
        else
        {
            gameObject.SetActive(isActive);
        }
    }
}
