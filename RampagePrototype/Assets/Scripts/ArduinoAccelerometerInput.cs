using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ArduinoAccelerometerInput : MonoBehaviour
{
    [Header("Serial Port Settings")]
    public string portName = "COM4"; // Change to your Arduino port
    public int baudRate = 9600;

    [Header("Accelerometer Settings")]
    public float leftThreshold = -15f; // Pitch degrees to go left
    public float rightThreshold = 15f; // Pitch degrees to go right
    public bool invertDirection = false;

    [Header("Debug Settings")]
    public bool showDebugReadings = true;

    private SerialPort serialPort;
    private float currentPitch = 0f;
    private string lastRawData = "";

    void Start()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.Open();
            serialPort.ReadTimeout = 50;
            Debug.Log("Connected to Arduino on " + portName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to open serial port: " + e.Message);
        }
    }

    void Update()
    {
        if (serialPort == null)
        {
            Debug.LogError("SerialPort is null!");
            return;
        }

        if (!serialPort.IsOpen)
        {
            Debug.LogError("SerialPort is not open!");
            return;
        }

        try
        {
            if (serialPort.BytesToRead > 0)
            {
                string data = serialPort.ReadLine().Trim();
                lastRawData = data;

                if (showDebugReadings)
                {
                    Debug.Log($"Received: '{data}'");
                }

                // Expecting format: "PITCH:45.5" for pitch in degrees
                if (data.StartsWith("PITCH:"))
                {
                    string pitchString = data.Substring(6);
                    if (float.TryParse(pitchString, out currentPitch))
                    {
                        if (invertDirection) currentPitch = -currentPitch;

                        if (showDebugReadings)
                        {
                            Debug.Log($"Pitch: {currentPitch:F1}° | Input: {GetHorizontalInput():F2}");
                        }
                    }
                    else if (showDebugReadings)
                    {
                        Debug.LogWarning($"Failed to parse pitch from: '{pitchString}'");
                    }
                }
                else if (showDebugReadings)
                {
                    Debug.LogWarning($"Data doesn't start with 'PITCH:' - got: '{data}'");
                }
            }
        }
        catch (System.Exception e)
        {
            if (showDebugReadings)
                Debug.LogError($"Serial error: {e.Message}");
        }
    }

    public float GetHorizontalInput()
    {
        // Check if pitch exceeds thresholds
        if (currentPitch >= rightThreshold)
        {
            return 1f; // Go right
        }
        else if (currentPitch <= leftThreshold)
        {
            return -1f; // Go left
        }

        return 0f; // No movement
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
