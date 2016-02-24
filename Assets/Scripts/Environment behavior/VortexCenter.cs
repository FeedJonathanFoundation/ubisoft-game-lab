using UnityEngine;
using System.Collections;

public class VortexCenter : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider col)
     {
         //need to put code to not pull on walls
         //Debug.Log("In field");
         if(col.gameObject.tag == "Player")
         {
             Player player;
             player = col.gameObject.GetComponent<Player>();
             player.LightEnergy.Deplete(player.LightEnergy.CurrentEnergy+10);
         }
         
        /*if(col.transform.tag == "guest")
             count++;*/
     }
}
