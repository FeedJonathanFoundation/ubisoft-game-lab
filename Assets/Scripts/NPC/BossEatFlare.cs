using UnityEngine;
using System.Collections;

public class BossEatFlare : MonoBehaviour 
{
    [Tooltip("Size of the mouth of the boss fish")]
    public float mouthSize;

    void Start() 
    {
        // Set the radius
        this.GetComponent<SphereCollider>().radius = mouthSize;
    }
    
    void OnTriggerEnter(Collider col) 
    {
        // LightAbsorber has a player tag
        if(col.tag == "Flare")
        {
            col.transform.parent.gameObject.SetActive(false);
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; i++)
            {
                FlareSpawner flare = players[i].GetComponent<FlareSpawner>();
                if (flare != null)
                {
                    flare.EatFlare();
                }
            }
            
        }
    }
}
