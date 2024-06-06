using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 1f;
    public Animator animator;
    private Vector2 movement;

    void Update()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

        if (movement != Vector2.zero)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }
    }

    public void Initialize(float speed, AnimationClip[] clips)
    {
        walkSpeed = speed;
        // Assign animation clips to animator here
    }
}

