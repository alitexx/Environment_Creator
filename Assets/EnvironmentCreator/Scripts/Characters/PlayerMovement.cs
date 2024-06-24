using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 1f;
    public Animator animator;
    private Vector2 movement;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Get input from horizontal and vertical axis
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

        // Normalize the movement vector to prevent faster diagonal movement
        Vector3 move = new Vector3(movement.x, movement.y, 0).normalized;

        // If there is movement input
        if (move != Vector3.zero)
        {
            // Move the character
            transform.Translate(move * walkSpeed * Time.deltaTime, Space.World);

            // Update animator parameters
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", move.sqrMagnitude);
        }
        else
        {
            // If there is no movement, set speed to 0 in animator
            animator.SetFloat("Speed", 0);
        }
    }

    public void Initialize(float speed, AnimationClip[] clips)
    {
        walkSpeed = speed;
        // Assign animation clips to animator here
        // I did this elsewhere oops
    }
}
