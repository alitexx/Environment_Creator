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
            offset = new Vector3(0, 0, -10);  // Example offset for a top-down view
        }
        if (offset.z >= 0)
        {
            Debug.LogWarning("The Z axis should always be less than 0 in Unity 2D, or else the items in the scene will appear invisible. You can change the offset on the Main Camera object under the Camera Movement script.");
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

