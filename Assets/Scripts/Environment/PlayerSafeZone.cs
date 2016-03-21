using UnityEngine;
using System.Collections;

public class PlayerSafeZone : MonoBehaviour 
{
    void OnTriggerEnter(Collider col) 
    {
        if(col.tag == "Player") // have to put this because LightAbsorber has a player tag
        {
            Player player = col.GetComponent<Player>();
            if(player)
            {
                player.isSafe = true;
                //Debug.Log(player.isSafe);
            }
        }
    }
    
    void OnTriggerExit(Collider col) 
    {
        if(col.tag == "Player") // have to put this because LightAbsorber has a player tag
        {
            Player player = col.GetComponent<Player>();
            if(player)
            {
                player.isSafe = false;
                //Debug.Log(player.isSafe);
            }
        }
    }
}
