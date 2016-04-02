using UnityEngine;
using System.Collections;

/// <summary>
/// Collision Listener class is responsible for notifying the spawn volume
/// when there is a collision.
///
/// @author - Stella L.
/// @version - 1.0.0
///
/// </summary>
[RequireComponent(SphereCollider))]
public class CollisionListener : MonoBehaviour
{

    private SpawnVolume spawnVolume;
    private Vector3 center;

    /// <summary>
    /// Initializes the spawn volume and the collider's center value.
    /// </summary>
    void Awake()
    {
        spawnVolume = gameObject.GetComponentInParent<SpawnVolume>();
        center = this.gameObject.GetComponent<SphereCollider>().transform.position;
    }

    /// <summary>
    /// Notifies the spawn volume parent upon collision
    /// which collider was hit and whether it should be disabled.
    /// </summary>
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
