using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float leftBoundary = -5f;
    public float rightBoundary = 5f;

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float horizontalInput = 0f;

        // Check for arrow key input
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalInput = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            horizontalInput = 1f;
        }

        // Calculate new position
        Vector3 currentPosition = transform.position;
        float newX = currentPosition.x + (horizontalInput * moveSpeed * Time.deltaTime);

        // Clamp position within boundaries
        newX = Mathf.Clamp(newX, leftBoundary, rightBoundary);

        // Apply the new position
        transform.position = new Vector3(newX, currentPosition.y, currentPosition.z);
    }
}