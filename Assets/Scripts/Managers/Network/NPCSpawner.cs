using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NPCSpawner : NetworkBehaviour
{

    [SerializeField]
    GameObject prefab;
    [SerializeField]
    int numberOfNPCs;
    
    public override void OnStartServer()
    {
        for (int i = 0; i < numberOfNPCs; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-8f, 8f), 0f, Random.Range(-8f, 8f));
            Quaternion spawnRotation = Quaternion.Euler(0f, Random.Range(0f, 180f), 0);

            GameObject NPC = (GameObject)Instantiate(prefab, spawnPosition, spawnRotation);

            NetworkServer.Spawn(NPC);
        }
    }
}
