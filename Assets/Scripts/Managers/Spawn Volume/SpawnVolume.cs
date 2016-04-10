using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

/// <summary>
/// SpawnVolume class is responsible for spawning game objects
/// within sphere collider trigger(s)
///
/// @author - Stella L.
/// @author - Jonathan L.A
/// @version - 1.0.0
///
/// </summary>
[RequireComponent(typeof(ObjectPooler))]
public class SpawnVolume : NetworkBehaviour
{
    
    [SerializeField]
    [Tooltip("Spawn volume colliders - each sphere collider must be in its own game object.)")]
    private Collider[] colliders;
    
    [SerializeField]
    [Tooltip("Specify which game objects should be spawned in schools (in order of spawnObjects in Object Pool.)")]
    private bool[] schools;
    
    [SerializeField]
    [Tooltip("NPC probability of spawning (in order of spawnObjects in Object Pool.)")]
    private float[] probabilities;
    
    [SerializeField]
    [Range(0,1)]
    [Tooltip("Percent of maximum variance from default energy. Allows spawning of fish of different sizes.")]
    private float energyVariance;
    
    [SerializeField]
    [Tooltip("Number of fish to be spawned.")]
    private int fishCount;

    [SerializeField]
    [Tooltip("Number of fish in school.")]
    private int schoolSize;
    
    [SerializeField]
    [Tooltip("The initial angle at which the fish swim. Relative to the +y-axis, clockwise, in degrees.")]
    private float initialSwimAngle;

    [SerializeField]
    [Tooltip("Max distance between player and fish before fish is disabled.")]
    private float maxDistance;
    private float maxDistanceSquared;
    
    [SerializeField]
    [Tooltip("If true, when player travels backwards, spawn volumes respawn fish.")]
    private bool automaticReset;
    
    // Amount of space between fish in a school
    private float schoolSpacing = 0.3f;
    private List<Transform> players;
    // Tracks which spawn volumes are disabled
    private bool[] disabled;
    // Tracks which spawn volumes are initialized
    private bool[] initialized;
    private int numberOfTypes;
    private int colliderCount;
    // Holds all fish in the spawn volume
    private List<GameObject> fishes;
    // Holds all fish in the object pool
    private ObjectPooler pool;

    /// <summary>
    /// Initializes fish list and spawn volumes
    /// </summary>
    void Start() 
    {
        fishes = new List<GameObject>();
        players = new List<Transform>();
        // player = GameObject.Find("Player").transform;
        pool = ObjectPooler.current;
        numberOfTypes = pool.PooledObjectCount;
        colliderCount = colliders.Length;
        disabled = new bool[colliderCount];
        initialized = new bool[colliderCount];
        maxDistanceSquared = maxDistance * maxDistance;
        Reset();
        
        #if UNITY_EDITOR
            this.ValidateInputs();
        #endif
    }
    
    /// <summary>
    /// Iterate through colliders to see which should be initialized.
    /// Check and update existing fish status.
    /// </summary>
    void Update()
    {
        if (players.Count < 2)
        {
            Debug.Log("need to adjust to find correct number of players!");
            GameObject[] currentPlayers = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject current in currentPlayers)
            {
                if (current.name != "LightAbsorber")
                {
                    players.Add(current.transform);
                }
            }
        }
        // Iterate through the colliders
        // If not disabled and not intialized, initialized
        for (int i = 0; i < colliderCount; i++)
        {
            if (disabled[i]) { continue; }
            if (!initialized[i])
            {
                Initialize(i);
                
                // Reset the collider the player has passed
                if (automaticReset)
                {
                    Reset(i - 1);
                }
            }
        }
        // Check if fish should be active or deactivated
        if (fishes.Count > 0)
        {
            for (int i = 0; i < fishes.Count; i++)
            {
                if (fishes[i] != null)
                {
                    // REMOVE THIS - for DEBUGGING
                    fishes[i].SetActive(true);

                    CheckDistanceToPlayer(fishes[i].GetComponent<AbstractFish>());
                }
            }
        }
    }
    
    /// <summary>
    /// Checks the distance from the fish to the player.
    /// Activates the fish if sufficiently close to the player,
    /// and deactivates it otherwise.
    /// </summary>
    private void CheckDistanceToPlayer(AbstractFish fish)
    {
<<<<<<< HEAD
        // if (fish == null) { return; }
=======
        if (fish == null) { return; }
>>>>>>> df50156b499206d3c21d290b56cb1ceb18d897ce
        // for (int i = 0; i < players.Count; i++)
        // {
        //     float distanceSquared = (fish.transform.position - players[i].position).sqrMagnitude;
        //     if (distanceSquared > maxDistanceSquared)
        //     {
        //         fish.gameObject.SetActive(false);
        //     }
        //     else if (fish.gameObject.activeSelf == false && !fish.Dead)
        //     {
<<<<<<< HEAD
        //         fish.gameObject.SetActive(true);
=======
                fish.gameObject.SetActive(true);
>>>>>>> df50156b499206d3c21d290b56cb1ceb18d897ce
        //     }
        // }
    }
    
    /// <summary>
    /// Spawns fish in the newly initialized collider.
    /// </summary>
    private void Initialize(int colliderIndex)
    {
        initialized[colliderIndex] = true;
        Spawn(fishCount, colliderIndex);
    }

    private int count = 0;

    /// <summary>
    /// Spawns fish in a circular pattern in the specified collider.
    /// </summary>
    private void Spawn(int numberOfFish, int colliderIndex)
    {
        if (numberOfFish < 1) { return; }
        for (int i = 0; i < numberOfFish; i++)
        {
            if (numberOfTypes > -1)
            {
                int spawnTypeIndex = ChooseFish();
                // Select a point in a circle within the collider to spawn the fish
                float radius = GetSpawnVolumeRadius(colliderIndex) / 2;
                float angle = i * Mathf.PI * 2 / numberOfFish;
                Vector3 spawnLocation = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius + GetSpawnVolumeCenter(colliderIndex);
                // If fish should be spawned as a school (group of fish)
                if (schools[spawnTypeIndex])
                {
                    SpawnSchool(spawnTypeIndex, spawnLocation);
                    i += schoolSize - 1;
                }
                else
                {
                    SpawnFish(spawnTypeIndex, spawnLocation);
                }
            }
        }
    }
    
    /// <summary>
    /// Spawns an individual fish.
    /// </summary>
    private void SpawnFish(int spawnTypeIndex, Vector3 spawnLocation)
    {
        GameObject fish = pool.GetPooledObject(spawnTypeIndex);
        if (fish == null) { return; }
        fish.transform.position = spawnLocation;
        fish.transform.rotation = Quaternion.identity;
        fish.SetActive(true);

        LightSource lightSource = fish.GetComponent<LightSource>();
        AbstractFish abstractFish = fish.GetComponent<AbstractFish>();
        if (lightSource != null)
        {
            float variance = Random.Range(0, lightSource.LightEnergy.CurrentEnergy * energyVariance);
            lightSource.LightEnergy.Deplete(variance);
        }
        if (abstractFish != null)
        {
            // Override the fish's default swim direction
            abstractFish.DefaultWanderAngle = initialSwimAngle;
        }
        fishes.Add(fish);
        // NetworkServer.Spawn(fish);
        NPCID npcID = fish.GetComponent<NPCID>();
        if (npcID != null)
        {
            string identity =  lightSource.LightSourceID;
            npcID.npcID = identity;
        }
    }
    
    /// <summary>
    /// Spawns an individual fish.
    /// </summary>
    
    private void SpawnSchool(int spawnTypeIndex, Vector3 spawnLocation)
    {
        Vector3 modifiedLocation = spawnLocation;
        for (int i = 0; i < schoolSize; i++)
        {
            if (i % 2 == 0)
            {
                modifiedLocation += new Vector3(schoolSpacing * (i / 2), 0, 0);
            }
            else
            {
                modifiedLocation += new Vector3(0, schoolSpacing * i, 0);
            }
            SpawnFish(spawnTypeIndex, modifiedLocation);
        }
    }
    
    /// <summary>
    /// Returns the index to which fish should be spawned based on the specified probabilities.
    /// </summary>
    private int ChooseFish()
    {
        // If probabilities array not used, choose random fish type to spawn
        if (probabilities.Length < 1) 
        {
            return Random.Range(0, numberOfTypes);
        }
        float total = 0;
        for (int i = 0; i < numberOfTypes; i++)
        {
            total += probabilities[i];
        }
        float randomFish = Random.value * total;
        for (int i = 0; i < probabilities.Length; i++)
        {
            if (randomFish < probabilities[i])
            {
                return i;
            }
            else {
                randomFish -= probabilities[i];
            }
        }
        return (probabilities.Length - 1);
    }
    
    /// <summary>
    /// Resets all spawn volumes as uninitialized and disabled.
    /// </summary>
    private void Reset()
    {
        for (int i = 0; i < colliderCount; i++)
        {
            initialized[i] = false;
            disabled[i] = true;
        }
    }
    
    /// <summary>
    /// Resets the specified spawn volume as uninitialized and disabled.
    /// </summary>
    private void Reset(int index)
    {
        if (index < 0) { return; }
        initialized[index] = false;
        disabled[index] = true;
    }
    
    /// <summary>
    /// Returns the radius of the specified spawn volume collider.
    /// </summary>
    private float GetSpawnVolumeRadius(int colliderIndex)
    {
        float radius = colliders[colliderIndex].GetComponent<SphereCollider>().radius;
        return radius;
    }
    
    /// <summary>
    /// Returns the center point of the specified spawn volume collider.
    /// </summary>
    private Vector3 GetSpawnVolumeCenter(int colliderIndex)
    {
        Vector3 center = colliders[colliderIndex].transform.position;
        return center;
    }
    
    /// <summary>
    /// Returns the spawn volume index that contains the specified point.
    /// </summary>
    private int GetSpawnVolumeIndex(Vector3 point)
    {
        for (int i = 0; i < colliderCount; i++)
        {
            Vector3 colliderCenter = colliders[i].GetComponent<SphereCollider>().transform.position;

            if (colliderCenter == point)
            {
                return i;
            }
        }
        Debug.Log("Spawn volume index not found.");
        return -1;
    }
    
    /// <summary>
    /// Updates the specified spawn volume and updates the disable status.
    /// </summary>
    public void UpdateSpawnVolume(Vector3 center, bool disable)
    {
        int index = GetSpawnVolumeIndex(center);
        if (index > -1)
        {
            disabled[index] = disable;
        }
    }
    
    private void ValidateInputs()
    {
        #if UNITY_EDITOR
            if (fishCount == 0)
            {
                UnityEditor.EditorApplication.isPlaying = false;
                Debug.LogError("Spawn volume is currently set to spawn zero fish!");
            }
            if (maxDistance < 1)
            {
                UnityEditor.EditorApplication.isPlaying = false;
                Debug.LogError("Spawn volume max distance is too low! Fish will never activate!");
            }
            if (colliders.Length < 1)
            {
                UnityEditor.EditorApplication.isPlaying = false;
                Debug.LogError("Please add sphere colliders to the spawn volume Colliders array.");
            }
            if (probabilities.Length < 1)
            {
                Debug.LogError("Spawn volume probabilities are not set. Fish will spawn at equal probabilities.");
            }
            if (schools.Length < numberOfTypes)
            {
                Debug.LogError("Please set the Schools array size to " + numberOfTypes);
            }
        #endif
    }
    
}
