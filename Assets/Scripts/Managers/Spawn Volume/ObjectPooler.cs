using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{

    public static ObjectPooler current;
    [Tooltip("Game objects to pool and spawn.")]
    [SerializeField]
    private GameObject[] pooledObjects;
    [Tooltip("Number of game objects to pool (in order).")]
    [SerializeField]
    private int[] pooledAmount;
    [Tooltip("If true, more game objects will be instantiated if they 'run out' of pooled objects.")]
    [SerializeField]
    private bool extensible;

    private List<GameObject>[] pool;

    void Awake()
    {
        current = this;
    }
    
    void Start()
    {
        pool = new List<GameObject>[pooledObjects.Length];
        for (int i = 0; i < pooledObjects.Length; i++)
        {
            pool[i] = new List<GameObject>();
            for (int j = 0; j < pooledAmount[i]; j++)
            {
                GameObject gameobject = (GameObject)Instantiate(pooledObjects[i]);
                gameobject.SetActive(false);
                pool[i].Add(gameobject);
            }
        }
    }
	
	public GameObject GetPooledObject(int objectID)
    {
        if (objectID > pooledObjects.Length)
        {
            Debug.Log("Trying to rerieve a pooled object that does not exist. Add new object to pooledObjects array.");
            return null;
        }
        for (int i = 0; i < pool[objectID].Count; i++)
        {
            GameObject current = pool[objectID][i];
            if (!current.activeInHierarchy)
            {
                ReactivateObjectLight(current);
                return current;
            }
        }
        if (extensible)
        {
            GameObject gameobject = (GameObject)Instantiate(pooledObjects[objectID]);
            pool[objectID].Add(gameobject);
            return gameobject;
        }
        return null;
    }
    
    public void ResetPool()
    {
        for (int i = 0; i < pooledObjects.Length; i++)
        {
            for (int j = 0; j < pool[i].Count; j++)
            {
                GameObject current = pool[i][j];
                if (current.activeSelf == true)
                {
                    current.SetActive(false);
                }
                ReactivateObjectLight(current);
            }
        }
    }
    
    private void ReactivateObjectLight(GameObject current)
    {
        LightSource light = current.GetComponent<LightSource>();
        if (light != null)
        {
            light.LightEnergy.Add(light.DefaultEnergy);
        }
    }
    
    public int PooledObjectCount
    {
        get
        {
            return pooledObjects.Length;
        }
    }
}
