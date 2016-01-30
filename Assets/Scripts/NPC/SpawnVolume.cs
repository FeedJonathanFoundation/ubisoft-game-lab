using UnityEngine;
using System.Collections;

public class SpawningVolume : MonoBehaviour
{

    public GameObject[] fishesToSpawn;      // Fish prefabs to be spawned
    public float spawnTime = 3f;            // Time between spawns
    public Transform[] spawnPoints;         // Array of possible spawn points
    public int minFishCount;                // Minimum number of fish in level
    public int maxFishCount;                // Maximum number of fish in level
    public int minFishDifficulty;           // Mass of smallest fish
    public int maxFishDifficulty;           // Mass of largest fish
    
    private int fishCount;
    
	void Start() 
    {
        fishCount = 0;
        
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
        
        // Choose random index within number of spawn points
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);
        
        // Choose random fish type to spawn
        int spawnTypeIndex = Random.Range(0, fishesToSpawn.Length);
        
        // Create instance of fish prefab at spawn point and rotation
        Instantiate(
            fishesToSpawn[spawnTypeIndex], spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation
            );
        
        // Increment number of fish spawned
        fishCount++;
    }
    
}
