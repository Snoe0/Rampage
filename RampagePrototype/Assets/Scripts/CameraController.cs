using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float leftBoundary = -5f;
    public float rightBoundary = 5f;

    [Header("Input Settings")]
    public bool useArduinoInput = true;
    public bool allowKeyboardInput = true; // Keep keyboard as fallback
    private ArduinoAccelerometerInput arduinoInput;

    void Start()
    {
        if (useArduinoInput)
        {
            arduinoInput = GetComponent<ArduinoAccelerometerInput>();
            if (arduinoInput == null)
            {
                Debug.LogWarning("ArduinoAccelerometerInput not found. Add it to the GameObject.");
            }
        }
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float horizontalInput = 0f;

        // Get Arduino input if enabled
        if (useArduinoInput && arduinoInput != null)
        {
            horizontalInput = arduinoInput.GetHorizontalInput();
        }

        // Allow keyboard input as override or fallback
        if (allowKeyboardInput && Mathf.Approximately(horizontalInput, 0f))
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                horizontalInput = -1f;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                horizontalInput = 1f;
            }
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