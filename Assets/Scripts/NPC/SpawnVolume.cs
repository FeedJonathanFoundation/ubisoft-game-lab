using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// implement probabilities to spawn certain fish types

public class SpawnVolume : MonoBehaviour
{
    
    [SerializeField]
    [Tooltip("Delay between spawns")]
    private float spawnTime;
    
    [SerializeField]
    [Tooltip("Array of possible NPCs to spawn")]
    private GameObject[] spawnObject;
    
    [SerializeField]
    [Tooltip("NPC probability of spawning (in order of spawnObject)")]
    private float[] probabilities;
    
    [SerializeField]
    [Tooltip("If true, uses specific spawn points. Else, uses randomly generated spawn points.")]
    private bool overrideSpawnLocations = false;
    
    [SerializeField]
    [Tooltip("If true, spawns randomly within a circle. Else, generates in a circle pattern.")]
    private bool spawnRandom = false;
    
    [SerializeField]
    [Tooltip("Specific spawn points.")]
    private Transform[] spawnPoints;
    
    [SerializeField]
    [Tooltip("Minimum number of fish to be spawned.")]
    private int minFishCount;
    
    [SerializeField]
    [Tooltip("Maximum number of fish to be spawned.")]
    private int maxFishCount; 
    
    [SerializeField]
    [Tooltip("Mass of smallest fish.")]
    private int minFishDifficulty;
    /// <summary>
    /// Mass of largest fish.
    /// </summary>
    [Tooltip("Mass of largest fish.")]
    private int maxFishDifficulty;

    [SerializeField]
    [Tooltip("Max distance between player and fish before fish is disabled.")]
    private float maxDistance = 60f;

    private Transform player;
    private int fishCount;
    private bool disabled;
    private bool initialized;
    
    private List<GameObject> fishes;
   
    /// <summary>
    /// Initializes fish count to 0,
    /// Spawns the minimum number of fish without delay
    /// Spawns the maximum number of fish incrementally
    /// </summary>
	void Start() 
    {
        Reset();
        fishes = new List<GameObject>();
        fishCount = 0;

        player = GameObject.FindWithTag("Player").transform;
    }
    
    void Update()
    {
        if (disabled)
        {
            return;
        }
        if (!initialized) 
        {
            Initialize();
        }
        else if (fishes.Count > 0 && initialized)
        {
            for (int i = 0; i < fishes.Count; i++)
            {
                if (fishes[i] != null)
                {
                    float distance = Vector3.Distance(fishes[i].transform.position, player.position);
                    if (distance > maxDistance)
                    {
                        fishes[i].SetActive(false);
                        fishes.Remove(fishes[i]);
                    }
                }
            }
        }
    }
    
    private void Initialize()
    {
        SpawnCircle(minFishCount);    
        
        // Spawn after a delay of spawnTime
        if (spawnRandom)
        {
            InvokeRepeating ("Spawn", spawnTime, spawnTime);
        }
        initialized = true;
    }

    /// <summary>
    /// If player is dead or max number of fish have been spawned,
    /// exit function
    /// If spawn location is valid, spawn fish
    /// </summary>
    private void Spawn()
    {
        if (disabled) { return; }
        if (fishCount >= maxFishCount) { return; }
        if (!spawnRandom) { return; }
        
        int spawnTypeIndex = ChooseFish();

        Vector3 spawnLocation = GenerateValidSpawnPoint(spawnTypeIndex);
        if (spawnLocation == new Vector3 (-999,-999,-999)) { 
            return; 
        }
        // If valid spawn point location,
        // Create instance of fish prefab at spawn point and rotation
        GameObject fish = (GameObject)Instantiate(spawnObject[spawnTypeIndex], spawnLocation, Quaternion.identity);
        fishes.Add(fish);
        fishCount++;
    }
    
    private void SpawnCircle(int numberOfFish)
    {
        for (int i = 0; i < numberOfFish; i++)
        {
            int spawnTypeIndex = ChooseFish();
            float radius = GetSpawnVolumeRadius() / 2;
            float angle = i * Mathf.PI * 2 / numberOfFish;
            Vector3 spawnLocation = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius + transform.position;
            if (spawnObject.Length > 0 && spawnObject[spawnTypeIndex] != null)
            {
                GameObject fish = (GameObject)Instantiate(spawnObject[spawnTypeIndex], spawnLocation, Quaternion.identity);
                fishes.Add(fish);
                fishCount++;
            }
        }
    }
    
    private int ChooseFish()
    {
        // If probabilities array not used, choose random fish type to spawn
        if (probabilities.Length < 1) 
        {
            return Random.Range(0, spawnObject.Length);
        }
        
        // Choose fish based on probabilities
        float total = 0;
        for (int i = 0; i < spawnObject.Length; i++)
        {
            total += probabilities[i];
        }

        float randomFish = Random.value * total;
        for (int i= 0; i < probabilities.Length; i++) {
            if (randomFish < probabilities[i]) {
                return i;
            }
            else {
                randomFish -= probabilities[i];
            }
        }
        return (probabilities.Length - 1);
    }
    
    private Vector3 GenerateValidSpawnPoint(int spawnIndex)
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
                potentialSpawnPoint.z = 0f;

                if (IsValidSpawnPoint(potentialSpawnPoint, spawnIndex)) 
                {
                    spawnLocation = potentialSpawnPoint;
                    break;
                }
            }
        }   
        return spawnLocation;
    }
    
    private bool IsValidSpawnPoint(Vector3 spawnPoint, int spawnIndex)
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
    
    // Checks whether the spawn point is occupied within the radius
    private bool IsOccupied(Vector3 spawnPoint, float spawnRadius)
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
        if (other.gameObject.CompareTag("SpawnSignal")) 
        {
            disabled = false;
        }
        if (other.gameObject.CompareTag("Player")) 
        {
            disabled = true;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("SpawnSignal"))
        {
            Reset();
        }
    }
    
    private void Reset()
    {
        initialized = false;
            disabled = true;
    }
    
    private float GetSpawnVolumeRadius()
    {
        float radius = gameObject.GetComponent<SphereCollider>().radius;
        return radius;
    }
    
    // Following functions used if NPC is added/destroyed
    public int GetFishCount()
    {
        return fishCount;
    }
}
