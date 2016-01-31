using UnityEngine;
using System.Collections;

public class SpawningVolume : MonoBehaviour
{

    public GameObject[] fishesToSpawn;      // Fish prefabs to be spawned
    public float spawnTime = 3f;            // Time between spawns
    public Transform[] spawnPoints;         // Array of possible spawn points
    public float spawnPointRadius;          // Radius of sphere to check for spawned fish
    
    public int minFishDifficulty;           // Mass of smallest fish
    public int maxFishDifficulty;           // Mass of largest fish
    public int minFishCount;                // Minimum number of fish in level
    public int maxFishCount;                // Maximum number of fish in level
    public int minSchoolPopulation;         // Minimum number of fish in one school
    public int maxSchoolPopulation;         // Maximum number of fish in one school
    public float spaceBetweenFish;          // Space between fish in a school
    
    private int fishCount;

    void Start() 
    {
        fishCount = 0;
        
        // Spawn minimum number of fish
        while (fishCount < minFishCount)
        {
            InvokeRepeating("SpawnIndividual", 0, 0);
        }
        
       // Spawn after a delay of spawnTime and continue to call after the same amount of time
       while (fishCount < maxFishCount)
       {
           InvokeRepeating ("SpawnIndividual", spawnTime, spawnTime);
           InvokeRepeating("SpawnSchool", spawnTime, spawnTime);
       }
    }
	
    void SpawnIndividual() 
    {
        // if (player.isDead) { return; // exit function }
        
        // Choose random index within number of spawn points
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);        
        // Choose random fish type to spawn
        int spawnTypeIndex = Random.Range(0, fishesToSpawn.Length);
        // Calculate spawn radius
        float spawnRadius = calculateRadius(fishesToSpawn[spawnTypeIndex], 1);
        
        // If spawn point is not occupied, spawn fish
        if (!isOccupied(spawnPoint, spawnRadius))
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
        int schoolPopulation = Random.Range(minSchoolPopulation, maxSchoolPopulation);
        // Choose random fish type to spawn
        int spawnTypeIndex = Random.Range(0, fishesToSpawn.Length);
        // Calculate spawn radius
        float spawnRadius = calculateRadius(fishesToSpawn[spawnTypeIndex], 1);
        float schoolSpawnRadius = calculateRadius(fishesToSpawn[spawnTypeIndex], schoolPopulation);
        
        // If spawn point is not occupied, spawn fish
        if (!isOccupied(spawnPoint, schoolSpawnRadius))
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
    bool isOccupied(Vector3 spawnPoint, float spawnPointRadius) 
    {
        // Array of all colliders touching or inside sphere
        Collider[] hitColliders = Physics.OverlapSphere(spawnPoint, spawnPointRadius);
        if (hitColliders.Length < 1) { return false; }
        return true;
    }
    
    // Calculates the radius of a sphere around the fish
    float calculateRadius(GameObject fish, int numberOfFish)
    {
        float height = getHeight(fish);
        float width = getWidth(fish);
        float max;
        
        if (height > width) { max = height; }
        else { max = width; }

        if (numberOfFish == 1) { return max / 2; }
        return ((max + spaceBetweenFish) * numberOfFish) / 2;  
    }
    
    // Returns the height of a fish
    float getHeight(GameObject fish)
    {
        return transform.lossyScale.y;
    }
    
    // Returns the width of a fish
    float getWidth(GameObject fish)
    {
        return transform.lossyScale.x;
    }
    
}
