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
    /// Mass of smallest fish.
    /// </summary>
    [Tooltip("Mass of smallest fish.")]
    public int minFishDifficulty;
    /// <summary>
    /// Mass of largest fish.
    /// </summary>
    [Tooltip("Mass of largest fish.")]
    public int maxFishDifficulty;
    
    /// <summary>
    /// Tracks number of fish spawned.
    /// </summary>
    private int fishCount;
    
    private bool disabled;

    /// <summary>
    /// Initializes fish count to 0,
    /// Spawns the minimum number of fish without delay
    /// Spawns the maximum number of fish incrementally
    /// </summary>
	void Start () 
    {
        disabled = false;
        
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
        if (disabled) { return; }
        if (fishCount >= maxFishCount) { return; }
        
        int spawnTypeIndex = ChooseFish();

        Vector3 spawnLocation = GenerateValidSpawnPoint(spawnTypeIndex);
        if (spawnLocation == new Vector3 (-999,-999,-999)) { 
            return; 
        }
        // If valid spawn point location,
        // Create instance of fish prefab at spawn point and rotation
        Instantiate(spawnObject[spawnTypeIndex], spawnLocation, Quaternion.identity);
        fishCount++;
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
    
    Vector3 GenerateValidSpawnPoint(int spawnIndex)
    {
        Vector3 spawnLocation = new Vector3 (-999,-999,-999);
        
        // Use specific spawn points
        // else use random spawn point
        if (overrideSpawnLocations)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (IsValidSpawnPoint(spawnPoints[i].position, spawnIndex)) 
                {
                    spawnLocation = spawnPoints[i].position;
                    break;
                }
            }
        }
        else
        {
            int timeout = 10;       
            // generate random point in sphere collider
            for (int i = 0; i < timeout; i++)
            {
                Vector3 potentialSpawnPoint = transform.position + (Random.insideUnitSphere * GetSpawnVolumeRadius());

                if (IsValidSpawnPoint(potentialSpawnPoint, spawnIndex)) 
                {
                    spawnLocation = potentialSpawnPoint;
                    break;
                }
            }
        }   
        return spawnLocation;
    }
    
    bool IsValidSpawnPoint(Vector3 spawnPoint, int spawnIndex)
    {
        // Calculate spawn radius of fish
        GameObject obj = spawnObject[spawnIndex];
        float spawnRadius = obj.GetComponent<SphereCollider>().radius + 1;
        
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
        Collider[] colliders = Physics.OverlapSphere(spawnPoint, spawnRadius);
        if (colliders.Length == 1)
        {
            return false; 
        }
        return true;
    }
    
    // If player enters spawn volume, disable spawning
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            disabled = true;
        }
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

