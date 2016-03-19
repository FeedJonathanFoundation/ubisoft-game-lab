using UnityEngine;
using System.Collections;

public class CollisionListener : MonoBehaviour
{

    private SpawnVolume spawnVolume;
    private Vector3 center;

    void Awake()
    {
        spawnVolume = gameObject.GetComponentInParent<SpawnVolume>();
        center = this.gameObject.GetComponent<SphereCollider>().transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SpawnSignal")) 
        {
            spawnVolume.UpdateSpawnVolume(center, false);
        }
        if (other.gameObject.CompareTag("Player")) 
        {
            spawnVolume.UpdateSpawnVolume(center, true);
        }
    }
}
