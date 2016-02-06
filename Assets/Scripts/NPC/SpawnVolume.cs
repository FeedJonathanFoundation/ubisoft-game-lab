using UnityEngine;
using System.Collections;

public class SpawnVolume : MonoBehaviour {
    
    /// <summary>
    /// Delay between spawns
    /// </summary>
    [Tooltip("Delay between spawns")]
    public float spawnTime;
    
    /// <summary>
    /// Array of possible NPCs to spawn
    /// </summary>
    [Tooltip("Array of possible NPCs to spawn")]
    public GameObject[] spawnObject;
    
    /// <summary>
    /// If true, uses specific spawn points.
    /// Else, uses randomly generated spawn points.
    /// </summary>
    [Tooltip("If true, uses specific spawn points. Else, uses randomly generated spawn points.")]
    public bool overrideSpawnLocations = false;
    /// <summary>
    /// Specific spawn points.
    /// Only used if overrideSpawnLocations == true
    /// </summary>
    [Tooltip("Specific spawn points.")]
    public Transform[] spawnPoints;             // Array of possible spawn points
    
    /// <summary>
    /// Radius of sphere to check for spawned fish.
    /// </summary>
    [Tooltip("Radius of sphere to check for spawned fish.")]
    public float spawnPointRadius;

    
    /// <summary>
    /// Minimum number of fish to be spawned.
    /// </summary>
    [Tooltip("Minimum number of fish to be spawned.")]
    public int minFishCount;
    /// <summary>
    /// Maximum number of fish to be spawned.
    /// </summary>
    [Tooltip("Maximum number of fish to be spawned.")]
    public int maxFishCount;
    
    /// <summary>
    /// Tracks number of fish spawned.
    /// </summary>
    private int fishCount;
    
    public int minFishDifficulty;               // Mass of smallest fish
    public int maxFishDifficulty;               // Mass of largest fish

    /// <summary>
    /// Initializes fish count to 0,
    /// Spawns the minimum number of fish without delay
    /// Spawns the maximum number of fish incrementally
    /// </summary>
	void Start () 
    {
        fishCount = 0;
        for (int i = 0; i < minFishCount; i++)
        {
            Invoke("Spawn", 0f);                            // Spawn minimum number of fish
        }     
        InvokeRepeating ("Spawn", spawnTime, spawnTime);    // Spawn after a delay of spawnTime
	}

    /// <summary>
    /// If player is dead or max number of fish have been spawned,
    /// exit function
    /// If spawn location is valid, spawn fish
    /// </summary>
    void Spawn()
    {
        // if (player.isDead) { return; // exit function }
        if (fishCount >= maxFishCount) { return; }

        Vector3 spawnLocation = GenerateValidSpawnPoint();
        if (spawnLocation == new Vector3 (-999,-999,-999)) { return; }
        
        int spawnTypeIndex = ChooseFish();

        // If valid spawn point location,
        // Create instance of fish prefab at spawn point and rotation
        // if (IsValidSpawnPoint(spawnLocation))
        // {
            Instantiate(spawnObject[spawnTypeIndex], spawnLocation, Quaternion.identity);
            fishCount++;
        // }
    }
    
    void SpawnSchool()
    {
        // generate
                // if (player.isDead) { return; // exit function }

        // Vector3 spawnLocation;
        // // Choose random index within number of spawn points
        // int spawnPointIndex = Random.Range(0, spawnPoints.Length);

        // // Use specific spawn points
        // if (overrideSpawnLocations)
        // {
        //     spawnLocation = spawnPoints[spawnPointIndex].position;
        // }
        // // Use random spawn point
        // else
        // {
        //     // generate random point in sphere collider
        //     spawnLocation = Random.insideUnitSphere * GetRadius();
        // }

        // // Choose random fish type to spawn
        // int spawnTypeIndex = Random.Range(0, fishesToSpawn.Length);

        // School newSchool = new School(fishesToSpawn[spawnTypeIndex]);
        // int schoolPopulation = newSchool.GetSchoolPopulation();

        // // If spawn point is not occupied, spawn fish
        // if (IsValidSpawnPoint(spawnLocation))
        // {
        //     // Create instance of fish prefab at spawn point and rotation
        //     for (int i = 0; i < schoolPopulation; i++)
        //     {
        //         // Calculate school pattern
        //         // Vector3 pos = spawnLocation + spacing * i;  // NEEDS TO BE FIXED
        //         // Instantiate(fishesToSpawn[spawnTypeIndex], pos, Quaternion.identity);
        //         // fishCount++;
        //     }
        // }
    }
    
    int ChooseFish()
    {
        // Choose random fish type to spawn
        return Random.Range(0, spawnObject.Length);
        
        // Choose specific fish to spawn
        // Make flag for specific fish?
    }
    
    Vector3 GenerateValidSpawnPoint()
    {
        Vector3 spawnLocation = new Vector3 (-999,-999,-999);
        
        // Use specific spawn points
        // else use random spawn point
        if (overrideSpawnLocations)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                spawnLocation = spawnPoints[i].position;
                if (IsValidSpawnPoint(spawnLocation)) {
                    break;
                }
            }
        }
        else
        {
            // generate random point in sphere collider
            int timeout = 10;
            spawnLocation = Random.insideUnitSphere * GetSpawnVolumeRadius();
            while (!IsValidSpawnPoint(spawnLocation) && timeout > 0)
            {
                spawnLocation = Random.insideUnitSphere * GetSpawnVolumeRadius();
                timeout--;
            }
        }
        return spawnLocation;
    }
    
    bool IsValidSpawnPoint(Vector3 spawnPoint)
    {
        // Calculate spawn radius of fish
        // int spawnTypeIndex = Random.Range(0, fishesToSpawn.Length);
        // float spawnRadius = spawnObject.GetHeight() / 2;
        float spawnRadius = 1f;

        if (IsOccupied(spawnPoint, spawnRadius))
        {
            return false;
        }
        return true;
    }
    
    // UNDER CONSTRUCTION
    // Checks whether the spawn point is occupied within the radius
    bool IsOccupied(Vector3 spawnPoint, float spawnRadius)
    {
        // Array of all colliders touching or inside sphere
        Collider[] hitColliders = Physics.OverlapSphere(spawnPoint, spawnRadius);
        if (hitColliders.Length < 1) { return false; }
        return true;
    }
    
    public float GetSpawnVolumeRadius()
    {
        float radius = this.GetComponent<SphereCollider>().radius;
        return radius;
    }
    
    // Following functions used if NPC is added/destroyed
    public int GetFishCount()
    {
        return fishCount;
    }
    
    public void IncrementFishCount()
    {
        fishCount++;
    }
    
    public void DecrementFishCount()
    {
        fishCount--;
    }
    
}

/// implement probabilities to spawn certain fish types

