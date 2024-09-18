using UnityEngine;

public class PopUp : MonoBehaviour
{
    private GameObject player;
    private float range;
    private SpriteRenderer spriteRenderer;
    private Transform associatedObjPos;
    private Transform[] childTransforms;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerCharacter");
        range = GameSettings.interactRange;
        spriteRenderer = GetComponent<SpriteRenderer>();
        childTransforms = GetComponentsInChildren<Transform>();
        associatedObjPos = this.GetComponent<FolderPlacement>().associatedGameObject.transform;
    }

    void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(player.transform.position, associatedObjPos.position);
            if (distance <= range)
            {
                SetChildrenActive(true);
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = true;
                }
            }
            else if (distance > range + (range / 10))
            {
                SetChildrenActive(false);
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = false;
                }
            }
        }
    }

    private void SetChildrenActive(bool isActive)
    {
        foreach (Transform child in childTransforms)
        {
            if (child != transform)
            {
                child.gameObject.SetActive(isActive);
            }
        }
    }
}
