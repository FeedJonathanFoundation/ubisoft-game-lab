using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{

    public static ObjectPooler current;
    [SerializeField]
    private GameObject pooledObject;
    [SerializeField]
    private int pooledAmount = 40;
    [SerializeField]
    private bool extensible;

    private List<GameObject> pooledObjects;

    void Awake()
    {
        current = this;
    }
    
    void Start()
    {
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject gameobject = (GameObject)Instantiate(pooledObject);
            gameobject.SetActive(false);
            pooledObjects.Add(gameobject);
        }
    }
	
	public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        if (extensible)
        {
            GameObject gameobject = (GameObject)Instantiate(pooledObject);
            pooledObjects.Add(gameobject);
            return gameobject;
        }
        return null;
    }
}
