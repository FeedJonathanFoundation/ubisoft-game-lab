using UnityEngine;
using System.Collections.Generic;

public class VortexPull : MonoBehaviour {
    [Tooltip("Increases the pull to the center of the whirlpool")]
    public float strengthOfAttraction;
    [Tooltip("Increases the roation of elements inside the whirlpool")]
    public float strengthOfRotation;
    private List<GameObject> objectsInVortex = new List<GameObject>();
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        
        foreach(GameObject objectsAttracted in objectsInVortex)
        {
            Vector3 offset = this.transform.position - objectsAttracted.GetComponent<Rigidbody>().transform.position;
            offset.z = 0;
            float magsqr = offset.sqrMagnitude;
            
            if (magsqr > 0.0001f)
            {
                objectsAttracted.GetComponent<Rigidbody>().AddForce(strengthOfAttraction * offset.normalized / magsqr, ForceMode.Acceleration);
                objectsAttracted.transform.RotateAround(this.transform.position, Vector3.forward, strengthOfRotation * Time.deltaTime);
            }
        }
	}
    
    void OnTriggerEnter(Collider col)
     {
         //need to put code to not pull on walls
         //Debug.Log("In field");
         if(col.gameObject.tag == "Player")
         {
             objectsInVortex.Add(col.gameObject);
         }
         
        /*if(col.transform.tag == "guest")
             count++;*/
     }
     
     void OnTriggerExit(Collider col) 
     {
        // Destroy everything that leaves the trigger
        //Debug.Log("Out field");
        objectsInVortex.Remove(col.gameObject);
    }
}
