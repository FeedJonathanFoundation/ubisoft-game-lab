using UnityEngine;
using System.Collections;

public class BossEatFlare : MonoBehaviour
{
    private GameObject player;
    
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }
    
    void OnTriggerEnter(Collider col)
    {

        if (col.CompareTag("Flare"))
        {
            Destroy(col.transform.parent.gameObject);
            Debug.Log("eat flare");
            //col.transform.parent.gameObject.SetActive(false);
            Transform flareSpawner = player.transform.FindChild("FlareSpawn");
            flareSpawner.GetComponent<FlareSpawner>().EatFlare();
        }
    }
}
