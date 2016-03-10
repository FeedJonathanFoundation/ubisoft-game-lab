using UnityEngine;
using System.Collections;

public class BossSafeZone : MonoBehaviour {

	// Use this for initialization
	void OnTriggerEnter(Collider col) 
    {
        if(col.tag == "Fish") // have to put this because LightAbsorber has a player tag also...why does it have a player tag?
        {
            FishBoss fishBoss = col.GetComponent<FishBoss>();
            fishBoss.atSafeZone = true;
        }
    }
}
