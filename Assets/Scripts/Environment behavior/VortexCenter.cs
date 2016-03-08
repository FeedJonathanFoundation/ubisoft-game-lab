using UnityEngine;
using System.Collections;

public class VortexCenter : MonoBehaviour 
{
    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            Player player;
            player = col.gameObject.GetComponent<Player>();
            player.LightEnergy.Deplete(player.LightEnergy.CurrentEnergy+10);
        }
    }
}
