using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    private bool isMoving = true;
    private bool isPaused;
    private Transform[] setPositions;
    private float movementSpeed = 1f;
    private float movementFrequency = 1f;
    private float waitTime = 1f;
    private bool isSetPositions;
    private Vector3 referenceSpace;

    private int currentSetPositionIndex = 0;
    private Vector3 targetPosition;

    private void Start()
    {
        referenceSpace = Resources.Load<GameObject>("EnvironmentCreator/Prefabs/ReferenceTile").transform.localScale;
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
                Vector3 randomDirection = GetRandomDirection();
                targetPosition = transform.position + randomDirection * referenceSpace.x;
                yield return MoveToPosition(targetPosition);

                yield return new WaitForSeconds(movementFrequency);
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
            case 0: return Vector3.forward;
            case 1: return Vector3.back;
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

            transform.position = Vector3.MoveTowards(transform.position, position, movementSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = position;
    }

    public void SetPause(bool pause)
    {
        isPaused = pause;
    }
}
