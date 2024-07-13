using System.Collections;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform targetLocation; // The location to teleport to
    public bool useFadeTransition = true; // Whether to use fade transition
    public Color fadeColor = Color.black; // Color of the fade transition

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerCharacter"))
        {
            if(targetLocation == null)
            {
                Debug.LogError("Please set a teleport location, it is currently set to null.");
                return;
            }

            if (useFadeTransition)
            {
                StartCoroutine(FadeAndTeleport(other.gameObject));
            }
            else
            {
                TeleportPlayer(other.gameObject);
            }
        }
    }

    private void TeleportPlayer(GameObject player)
    {
        player.transform.position = targetLocation.position;
    }

    private IEnumerator FadeAndTeleport(GameObject player)
    {
        yield return FadeManager.Instance.FadeOut(fadeColor);

        TeleportPlayer(player);

        yield return FadeManager.Instance.FadeIn();
    }
}
