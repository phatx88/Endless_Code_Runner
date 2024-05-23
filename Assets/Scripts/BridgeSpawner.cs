using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{
    // Array of prefabs to be used for the bridge segments
    public GameObject[] bridgePrefabs;

    // Enum to define different types of bridge segments
    enum enType
    {
        L_Corner,
        Straight,
        R_Corner,
    }

    // Enum to define possible directions for the segments
    enum enDirection
    {
        North,
        East,
        West,
    }

    // Class to hold information about each bridge segment
    class Segments
    {
        public GameObject segPrefab;
        public enType segType;

        public Segments(GameObject segPrefab, enType segType)
        {
            this.segPrefab = segPrefab;
            this.segType = segType;
        }
    };

    // List to keep track of active segments in the scene
    List<GameObject> activeSegments = new List<GameObject>();

    // Current segment being created
    Segments segment;
    
    // Starting coordinate for spawning segments
    Vector3 spawnCoord = new Vector3(0, 0, -6.0f);
    
    // Current and next direction for segment placement
    enDirection segCurrentdirection = enDirection.North;
    enDirection segNextDirection = enDirection.North;
    
    // Reference to the player's transform
    Transform playerTransform;

    float segLenght = 6.0f;
    float segWidth = 3.0f;
    int segsOnScreen = 10;
    bool stopGame = false;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the first segment
        segment = new Segments(bridgePrefabs[0], enType.Straight);

        // Find the player's transform in the scene
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        
        // Initialize the segments
        InitializeSegments();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player has triggered a segment change
        PlayerTrigger();
    }

    // Method to initialize segments at the start
    void InitializeSegments()
    {
        for (int i = 0; i < segsOnScreen; i++)
        {
            SpawnSegments();
        }
    }

    // Method to determine the type and prefab of the next segment
    void CreateSegments()
    {
        switch (segCurrentdirection)
        {
            case enDirection.North:
                segment.segType = (enType)Random.Range(0, 3);
                if (segment.segType == enType.Straight)
                {
                    segment.segPrefab = bridgePrefabs[Random.Range(0, 11)];
                }
                else if (segment.segType == enType.L_Corner)
                {
                    segment.segPrefab = bridgePrefabs[12];
                }
                else if (segment.segType == enType.R_Corner)
                {
                    segment.segPrefab = bridgePrefabs[11];
                }
                break;
            case enDirection.East:
                segment.segType = (enType)Random.Range(0, 2);
                if (segment.segType == enType.Straight)
                {
                    segment.segPrefab = bridgePrefabs[Random.Range(0, 11)];
                }
                else if (segment.segType == enType.L_Corner)
                {
                    segment.segPrefab = bridgePrefabs[12];
                }
                break;
            case enDirection.West:
                segment.segType = (enType)Random.Range(1, 3);
                if (segment.segType == enType.Straight)
                {
                    segment.segPrefab = bridgePrefabs[Random.Range(0, 11)];
                }
                else if (segment.segType == enType.R_Corner)
                {
                    segment.segPrefab = bridgePrefabs[11];
                }
                break;
        }
    }

    // Method to spawn a new segment in the scene
    void SpawnSegments()
    {
        GameObject prefabToInstatiate = segment.segPrefab;
        Quaternion prefabRotation = Quaternion.identity;
        Vector3 offSet = new Vector3(0, 0, 0);

        switch (segCurrentdirection)
        {
            case enDirection.North:
                if (segment.segType == enType.Straight)
                {
                    prefabRotation = Quaternion.Euler(0, 0, 0);
                    segNextDirection = enDirection.North;
                    spawnCoord.z += segLenght;
                }
                else if (segment.segType == enType.R_Corner)
                {
                    prefabRotation = Quaternion.Euler(0, 0, 0);
                    segNextDirection = enDirection.East;
                    spawnCoord.z += segLenght;
                    offSet.z += segLenght + segWidth / 2;
                    offSet.x += segWidth / 2;
                }
                else if (segment.segType == enType.L_Corner)
                {
                    prefabRotation = Quaternion.Euler(0, 0, 0);
                    segNextDirection = enDirection.West;
                    spawnCoord.z += segLenght;
                    offSet.z += segLenght + segWidth / 2;
                    offSet.x -= segWidth / 2;
                }
                break;

            case enDirection.East:
                if (segment.segType == enType.Straight)
                {
                    prefabRotation = Quaternion.Euler(0, 90, 0);
                    segNextDirection = enDirection.East;
                    spawnCoord.x += segLenght;
                }
                else if (segment.segType == enType.L_Corner)
                {
                    prefabRotation = Quaternion.Euler(0, 90, 0);
                    segNextDirection = enDirection.North;
                    spawnCoord.x += segLenght;
                    offSet.z += segWidth / 2;
                    offSet.x += segLenght + segWidth / 2;
                }
                break;

            case enDirection.West:
                if (segment.segType == enType.Straight)
                {
                    prefabRotation = Quaternion.Euler(0, -90, 0);
                    segNextDirection = enDirection.West;
                    spawnCoord.x -= segLenght;
                }
                else if (segment.segType == enType.R_Corner)
                {
                    prefabRotation = Quaternion.Euler(0, -90, 0);
                    segNextDirection = enDirection.North;
                    spawnCoord.x -= segLenght;
                    offSet.z += segWidth / 2;
                    offSet.x -= segLenght + segWidth / 2;
                }
                break;
        }

        if (prefabToInstatiate != null)
        {
            GameObject spawnedPrefab = Instantiate(prefabToInstatiate, spawnCoord, prefabRotation);
            activeSegments.Add(spawnedPrefab);
            spawnedPrefab.transform.parent = this.transform;
        }

        segCurrentdirection = segNextDirection;
        spawnCoord += offSet;
    }

    // Method to remove the oldest segment from the scene
    void RemoveSegments()
    {
        Destroy(activeSegments[0]);
        activeSegments.RemoveAt(0);
    }

    // Method to handle the player triggering segment changes
    void PlayerTrigger()
    {
        if (stopGame)
            return; 

        GameObject go = activeSegments[0];

        // If the player is far enough from the first segment, create and spawn new segments, then remove the old one
        if (Mathf.Abs(Vector3.Distance(playerTransform.position, go.transform.position)) > 16.0f)
        {
            CreateSegments();
            SpawnSegments();
            RemoveSegments();
        }
    }

    public void CleanTheScene()
    {
        stopGame = true;

        // Destroy and remove all active segments
        for (int j = activeSegments.Count - 1; j >= 0; j--)
        {
            Destroy(activeSegments[j]);
            activeSegments.RemoveAt(j);
        }

        // Reset spawn coordinates and directions
        spawnCoord = new Vector3(0, 0, -6);
        segCurrentdirection = enDirection.North;
        segNextDirection = enDirection.North;

        // Initialize the first segment
        segment = new Segments(bridgePrefabs[0], enType.Straight);
        InitializeSegments();

        stopGame = false;
    }
}
