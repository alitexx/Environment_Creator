using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player;  // Reference to the player's transform
    public Vector3 offset;    // Offset from the player position

    void Start()
    {
        // If you haven't set an offset in the inspector, set a default one
        if (offset == Vector3.zero)
        {
            offset = new Vector3(0, 10, 0);  // Example offset for a top-down view
        }
    }

    void LateUpdate()
    {
        // Move the camera to follow the player, applying the offset
        if (player != null)
        {
            transform.position = player.position + offset;
        }
    }
}

