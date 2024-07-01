using UnityEngine;

public class TriggeredSound : MonoBehaviour
{
    public enum TriggerType { Interaction, Colliding, Timed }
    public TriggerType triggerType;
    public float interval;

    private AudioSource audioSource;
    private float timer;

    private void Start()
    {
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (triggerType == TriggerType.Colliding && collision.gameObject.CompareTag("PlayerCharacter"))
        {
            audioSource.Play();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (triggerType == TriggerType.Interaction && other.CompareTag("PlayerCharacter"))
        {
            if (Input.GetKeyDown(GameSettings.interactKey))
            {
                audioSource.Play();
            }
        }
    }
}
