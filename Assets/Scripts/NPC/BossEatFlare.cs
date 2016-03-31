using UnityEngine;
using System.Collections;

public class BossEatFlare : MonoBehaviour
{
    private GameObject player;
    
    void Start()
    {
        player = GameObject.Find("Player");
    }
    
    void OnTriggerEnter(Collider col)
    {

        if (col.CompareTag("Flare"))
        {
            Destroy(col.transform.parent.gameObject);
            AkSoundEngine.PostEvent("BossEat", this.gameObject);
            player.GetComponent<FlareSpawner>().EatFlare();
        }
    }
}
