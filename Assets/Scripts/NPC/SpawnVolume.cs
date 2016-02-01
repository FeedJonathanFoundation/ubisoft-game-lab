using UnityEngine;
using System.Collections;

public class SpawningVolume : MonoBehaviour
{

    public AbstractFish[] fishesToSpawn;        // Fish prefabs to be spawned
    public float spawnTime = 3f;                // Time between spawns    
    public bool overrideSpawnLocations = false; // Determines usage of random or specific spawn points
    public Transform[] spawnPoints;             // Array of possible spawn points
    public float spawnPointRadius;              // Radius of sphere to check for spawned fish
    public int minFishDifficulty;               // Mass of smallest fish
    public int maxFishDifficulty;               // Mass of largest fish
    public int minFishCount;                    // Minimum number of fish in level
    public int maxFishCount;                    // Maximum number of fish in level
    
    private int fishCount;

    void Start() 
    {
        fishCount = 0;
        
        // Spawn minimum number of fish
        while (fishCount < minFishCount)
        {
            InvokeRepeating("Spawn", 0, 0);
        }
        
       // Spawn after a delay of spawnTime and continue to call after the same amount of time
       while (fishCount < maxFishCount)
       {
           InvokeRepeating ("Spawn", spawnTime, spawnTime);
       }
    }
    
    void Spawn()
    {
        // if (player.isDead) { return; // exit function }
        
        Vector3 spawnLocation;
       
        // Choose random fish type to spawn
        int spawnTypeIndex = Random.Range(0, fishesToSpawn.Length);
        
        // Use specific spawn points
        if (overrideSpawnLocations) 
        {
            // Choose random index within number of spawn points
            int spawnPointIndex = Random.Range(0, spawnPoints.Length); 
            spawnLocation = spawnPoints[spawnPointIndex].position;
        }
        // Use random spawn point
        else
        {
            // generate random point
        }
        
        // If spawn point is not occupied, spawn fish
        if (IsValidSpawnPoint(spawnPoints[spawnPointIndex].position))
        {
            // Create instance of fish prefab at spawn point and rotation
            Instantiate(fishesToSpawn[spawnTypeIndex], spawnPoints[spawnPointIndex].position, Quaternion.identity);
            // Increment number of fish spawned
            fishCount++;
        }
        
    }

    void SpawnSchool() 
    {
        // if (player.isDead) { return; // exit function }
        
        // Choose random index within number of spawn points
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);  
        
        // Choose random fish type to spawn
        int spawnTypeIndex = Random.Range(0, fishesToSpawn.Length);
        
        School newSchool = new School(fishesToSpawn[spawnTypeIndex]);
        int schoolPopulation = newSchool.GetSchoolPopulation();
        
        // If spawn point is not occupied, spawn fish
        if (IsValidSpawnPoint(spawnPoints[spawnPointIndex].position))
        {
            // Create instance of fish prefab at spawn point and rotation
            for (int i = 0; i < schoolPopulation; i++) 
            {
                // Calculate school pattern
                Vector3 pos = spawnPoints[spawnPointIndex].position + spacing * i;
                Instantiate(fishesToSpawn[spawnTypeIndex], pos, Quaternion.identity);
                
                // Increment number of fish spawned
                fishCount++;
            }
        }
    }
    
    // Checks whether the spawn point is occupied within the radius
    bool IsOccupied(Vector3 spawnPoint, float spawnPointRadius) 
    {
        // Array of all colliders touching or inside sphere
        Collider[] hitColliders = Physics.OverlapSphere(spawnPoint, spawnPointRadius);
        if (hitColliders.Length < 1) { return false; }
        return true;
    }
    
        
    bool IsValidSpawnPoint(Vector3 spawnPoint)
    {
        // Calculate spawn radius
        float spawnRadius = calculateRadius(fishesToSpawn[spawnTypeIndex]);
        
        if (IsOccupied(spawnPoint, spawnRadius))
        {
            return false;
        }
        return true;
    }
  
}
