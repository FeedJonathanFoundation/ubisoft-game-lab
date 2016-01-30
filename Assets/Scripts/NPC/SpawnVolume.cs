using UnityEngine;
using System.Collections;

public class SpawningVolume : MonoBehaviour
{

    public GameObject fish;         // Fish prefab to be spawned
    public float spawnTime = 3f;    // Time between spawns
    public Transform[] spawnPoints; // Array of possible spawn points
    
	void Start() 
    {
       // Spawn after a delay of spawnTime and continue to call after the same amount of time
	   InvokeRepeating ("Spawn", spawnTime, spawnTime);
	}
	
    void Spawn() 
    {
        // if (player.isDead) { return; // exit function }
        
        // Choose random index within number of spawn points
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);
        
        // Create instance of fish prefab at spawn point and rotation
        Instantiate(fish, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
    }
    
}
