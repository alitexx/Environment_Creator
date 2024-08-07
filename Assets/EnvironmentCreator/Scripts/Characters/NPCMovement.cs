using System.Collections;
using UnityEditor;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    private bool isMoving = true;
    public bool isPaused;
    public Transform[] setPositions;
    public float movementSpeed = 1f;
    public float movementFrequency = 1f;
    public float waitTime = 1f;
    public bool isSetPositions;
    private Vector3 referenceSpace;
    private Vector3 randomDirection;

    private int currentSetPositionIndex = 0;
    private Vector3 targetPosition;

    private Animator animator;
    private Rigidbody2D rb;

    private void Start()
    {
        referenceSpace = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/EnvironmentCreator/Prefabs/ReferenceTile.prefab").transform.localScale;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (isSetPositions)
        {
            if (setPositions.Length > 0)
                targetPosition = setPositions[0].position;
            StartCoroutine(MoveToSetPositions());
        }
        else
        {
            StartCoroutine(MoveRandomly());
        }
    }

    public void SetValues(float moveSpeed, float moveFrequency, Transform[] setPositions, bool isSetPositions, float waitTime)
    {
        movementSpeed = moveSpeed;
        movementFrequency = moveFrequency;
        this.setPositions = setPositions;
        this.isSetPositions = isSetPositions;
        this.waitTime = waitTime;
    }

    private IEnumerator MoveRandomly()
    {
        while (true)
        {
            if (!isPaused && isMoving)
            {
                yield return new WaitForSeconds(movementFrequency);
                randomDirection = GetRandomDirection();
                targetPosition = transform.position + randomDirection * referenceSpace.x;
                yield return MoveToPosition(targetPosition);
            }
            yield return null;
        }
    }

    private IEnumerator MoveToSetPositions()
    {
        while (true)
        {
            if (!isPaused && isMoving && setPositions.Length > 0)
            {
                targetPosition = setPositions[currentSetPositionIndex].position;
                yield return MoveToPosition(targetPosition);

                // Check if the next position is the same, wait for waitTime
                if (setPositions.Length > 1 && currentSetPositionIndex < setPositions.Length - 1 &&
                    setPositions[currentSetPositionIndex].position == setPositions[currentSetPositionIndex + 1].position)
                {
                    yield return new WaitForSeconds(waitTime);
                }

                currentSetPositionIndex = (currentSetPositionIndex + 1) % setPositions.Length;
            }
            yield return null;
        }
    }

    private Vector3 GetRandomDirection()
    {
        int randomIndex = Random.Range(0, 4);
        switch (randomIndex)
        {
            case 0: return Vector3.up;
            case 1: return Vector3.down;
            case 2: return Vector3.left;
            case 3: return Vector3.right;
        }
        return Vector3.forward;
    }

    private IEnumerator MoveToPosition(Vector3 position)
    {
        while (Vector3.Distance(transform.position, position) > 0.01f)
        {
            if (isPaused)
                yield return null;

            Vector3 direction = (position - transform.position).normalized;
            rb.MovePosition(transform.position + direction * movementSpeed * Time.deltaTime);

            // Update animator parameters
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
            animator.SetFloat("Speed", direction.magnitude);

            yield return null;
        }
        transform.position = position;

        // Reset speed when NPC stops
        animator.SetFloat("Speed", 0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //According to this code, the only thing that NPCs will not bounce off of are the player. If you wanted it to bounce off of fellow NPCs, you would add another and statement here.
        if (!isSetPositions && !collision.collider.CompareTag("PlayerCharacter"))
        {
            // Reverse the current direction
            randomDirection = -randomDirection;
            targetPosition = transform.position + randomDirection * (referenceSpace.x / 2);
            StopAllCoroutines();
            StartCoroutine(MoveToPosition(targetPosition));
            StartCoroutine(MoveRandomly());
        }
    }

    // In case the NPC needs to be frozen (pause screen)
    public void SetPause(bool pause)
    {
        isPaused = pause;
    }
}
