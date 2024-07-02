using UnityEngine;

public class TriggeredSound : MonoBehaviour
{
    public enum TriggerType { Interaction, Colliding, Timed }
    public TriggerType triggerType;
    public float interval;

    private AudioSource audioSource;
    private float timer;

    private GameObject player;
    private float range;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerCharacter");
        range = GameSettings.interactRange;
        audioSource = GetComponent<AudioSource>();

        if (triggerType == TriggerType.Timed)
        {
            timer = interval;
        }
    }

    private void Update()
    {
        if (triggerType == TriggerType.Timed)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                audioSource.Play();
                timer = interval;
            }
        }
        else if (player != null && triggerType == TriggerType.Interaction)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance <= range && Input.GetKeyDown(GameSettings.interactKey))
            {
                audioSource.Play();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggerType == TriggerType.Colliding && collision.gameObject.CompareTag("PlayerCharacter"))
        {
            audioSource.Play();
        }
    }
}
