using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [Header("Block Settings")]
    public GameObject blockPrefab; // Assign your block prefab in the inspector
    public Vector3[] spawnRows = new Vector3[3]; // Assign 3 spawn positions as Vector3 coordinates
    
    [Header("Movement Settings")]
    public float initialMoveSpeed = 5f;
    public float initialSpawnInterval = 2f;
    public float speedIncreaseRate = 0.1f; // How much speed increases per second
    public float spawnRateIncreaseRate = 0.05f; // How much spawn rate increases per second
    public float minSpawnInterval = 0.3f; // Minimum time between spawns
    
    [Header("Cleanup Settings")]
    public float deleteDistance = -10f; // Distance behind camera to delete blocks

    [Header("Floor Settings")]
    public GameObject floorObject; // Assign the floor GameObject in the inspector

    [Header("Collision Settings")]
    public GameObject cameraBody; // Assign the Body GameObject attached to the camera
    
    private float currentMoveSpeed;
    private float currentSpawnInterval;
    private List<GameObject> activeBlocks = new List<GameObject>();
    private Camera mainCamera;
    private float gameTime = 0f;
    private Renderer floorRenderer;
    private Material floorMaterial;
    private bool gameActive = true;
    
    void Start()
    {
        // Initialize values
        currentMoveSpeed = initialMoveSpeed;
        currentSpawnInterval = initialSpawnInterval;
        mainCamera = Camera.main;

        // Initialize floor texture components
        if (floorObject != null)
        {
            floorRenderer = floorObject.GetComponent<Renderer>();
            if (floorRenderer != null)
            {
                floorMaterial = floorRenderer.material;
            }
        }
        
        // Validate setup
        if (blockPrefab == null)
        {
            Debug.LogError("Block prefab is not assigned!");
            return;
        }
        
        if (spawnRows.Length != 3)
        {
            Debug.LogError("Please assign 3 spawn row positions!");
            return;
        }
        
        // Start spawning blocks
        StartCoroutine(SpawnBlocks());
    }
    
    void Update()
    {
        if (!gameActive) return;

        // Check for collisions with camera body
        CheckCollisions();

        if (!gameActive) return;

        // Update game time and difficulty
        gameTime += Time.deltaTime;
        UpdateDifficulty();

        // Move all active blocks
        MoveBlocks();

        // Move floor texture
        MoveFloorTexture();

        // Clean up blocks that are behind the camera
        CleanupBlocks();
    }

    void CheckCollisions()
    {
        if (cameraBody == null) return;

        // Get collider from camera body
        Collider bodyCollider = cameraBody.GetComponent<Collider>();
        if (bodyCollider == null) return;

        // Check collision with each active block
        foreach (GameObject block in activeBlocks)
        {
            if (block != null)
            {
                Collider blockCollider = block.GetComponent<Collider>();
                if (blockCollider != null && bodyCollider.bounds.Intersects(blockCollider.bounds))
                {
                    // Collision detected - stop the game
                    gameActive = false;
                    Debug.Log("Game Over! Camera body collided with a block.");
                    return;
                }
            }
        }
    }

    void UpdateDifficulty()
    {
        // Increase movement speed over time
        currentMoveSpeed = initialMoveSpeed + (gameTime * speedIncreaseRate);
        
        // Decrease spawn interval (increase spawn rate) over time
        currentSpawnInterval = Mathf.Max(
            minSpawnInterval, 
            initialSpawnInterval - (gameTime * spawnRateIncreaseRate)
        );
    }
    
    IEnumerator SpawnBlocks()
    {
        while (gameActive)
        {
            yield return new WaitForSeconds(currentSpawnInterval);
            if (gameActive) // Check again in case game ended during wait
            {
                SpawnBlock();
            }
        }
    }
    
    void SpawnBlock()
    {
        // Choose a random row (0, 1, or 2)
        int randomRow = Random.Range(0, 3);
        Vector3 spawnPosition = spawnRows[randomRow];

        // Spawn the block at the selected row position
        GameObject newBlock = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
        
        // Add to active blocks list
        activeBlocks.Add(newBlock);

        // Optional: Add a tag to identify spawned blocks
        newBlock.tag = "SpawnedBlock";
    }
    
    void MoveBlocks()
    {
        // Move all active blocks toward the camera (negative Z direction)
        foreach (GameObject block in activeBlocks)
        {
            if (block != null)
            {
                block.transform.Translate(Vector3.back * currentMoveSpeed * Time.deltaTime);
            }
        }
    }

    void MoveFloorTexture()
    {
        // Move floor texture at same speed as blocks to simulate running
        if (floorMaterial != null)
        {
            // Calculate texture offset based on movement speed and time
            float textureOffset = currentMoveSpeed * -0.1f * Time.time;

            // Apply offset to the material's main texture (usually the Y axis for forward movement)
            floorMaterial.mainTextureOffset = new Vector2(0, textureOffset);
        }
    }
    
    void CleanupBlocks()
    {
        // Create a list to store blocks that need to be removed
        List<GameObject> blocksToRemove = new List<GameObject>();
        
        foreach (GameObject block in activeBlocks)
        {
            if (block != null)
            {
                // Check if block is behind the camera (or beyond delete distance)
                float blockZ = block.transform.position.z;
                float cameraZ = mainCamera.transform.position.z;
                
                if (blockZ < cameraZ + deleteDistance)
                {
                    blocksToRemove.Add(block);
                }
            }
            else
            {
                // Block was destroyed elsewhere, mark for removal from list
                blocksToRemove.Add(block);
            }
        }
        
        // Remove and destroy blocks
        foreach (GameObject blockToRemove in blocksToRemove)
        {
            activeBlocks.Remove(blockToRemove);
            if (blockToRemove != null)
            {
                Destroy(blockToRemove);
            }
        }
    }
    
    // Optional: Method to reset the spawner
    public void ResetSpawner()
    {
        // Clear all active blocks
        foreach (GameObject block in activeBlocks)
        {
            if (block != null)
            {
                Destroy(block);
            }
        }
        activeBlocks.Clear();
        
        // Reset difficulty
        gameTime = 0f;
        currentMoveSpeed = initialMoveSpeed;
        currentSpawnInterval = initialSpawnInterval;
    }
    
    // Optional: Get current stats for UI display
    public float GetCurrentSpeed()
    {
        return currentMoveSpeed;
    }
    
    public float GetCurrentSpawnRate()
    {
        return 1f / currentSpawnInterval;
    }
    
    public int GetActiveBlockCount()
    {
        return activeBlocks.Count;
    }
}