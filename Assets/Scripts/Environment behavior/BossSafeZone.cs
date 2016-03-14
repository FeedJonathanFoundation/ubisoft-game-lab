    using UnityEngine;
using System.Collections;

public class BossSafeZone : MonoBehaviour 
{

	// Use this for initialization
    //STILL UNDER DEVELOPMENT
    void OnTriggerEnter(Collider col) 
    {
        if(col.tag == "Fish") 
        {
            FishBoss fishBoss = col.GetComponent<FishBoss>();
            //fishBoss.atSafeZone = true;
        }
    }
}
