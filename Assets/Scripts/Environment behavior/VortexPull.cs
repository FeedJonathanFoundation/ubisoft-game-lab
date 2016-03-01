using UnityEngine;
using System.Collections.Generic;

public class VortexPull : MonoBehaviour 
{
    [Tooltip("Increases the pull to the center of the whirlpool")]
    public float strengthOfAttraction;
    [Tooltip("Increases the rotation of elements inside the whirlpool")]
    public float strengthOfRotation;
    private List<GameObject> objectsInVortex = new List<GameObject>(); //this might change if the player is the only one attracted to vortex
	// Update is called once per frame
	void Update () 
    {
        
        foreach(GameObject objectsAttracted in objectsInVortex.ToArray())
        {
            if(rayChecker(objectsAttracted))
            {
                //this part creates the pull and rotation on the player
                Vector3 offset = this.transform.position - objectsAttracted.GetComponent<Rigidbody>().transform.position;
                offset.z = 0;
                float magsqr = offset.sqrMagnitude;
                
                if (magsqr > 0.0001f)
                {
                    objectsAttracted.GetComponent<Rigidbody>().AddForce(strengthOfAttraction * offset.normalized / magsqr, ForceMode.Acceleration);
                    objectsAttracted.transform.RotateAround(this.transform.position, Vector3.forward, strengthOfRotation * Time.deltaTime);
                }
            }
            else
            {
                colliderObjectRemover(objectsAttracted);
            }
        }
	}
    
    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player" && !objectsInVortex.Contains(col.gameObject))
        {
            if(rayChecker(col.gameObject))
            {
                objectsInVortex.Add(col.gameObject);
            }
         }
    }
    
    void OnTriggerExit(Collider col) 
    {
        colliderObjectRemover(col.gameObject);
    }
    
    void OnTriggerStay(Collider col)
    {
        if(col.gameObject.transform.tag == "Player" && !objectsInVortex.Contains(col.gameObject))
        {
            if(rayChecker(col.gameObject))
            {
                objectsInVortex.Add(col.gameObject);
            }
         }
    }
    
    bool rayChecker(GameObject gameObject) 
    {
        Ray ray = new Ray();
        RaycastHit hit;
        ray.origin = this.transform.position;
        ray.direction = gameObject.transform.position - this.transform.position;
        //checks if raycast hits a target and if the target is not a wall
        
        if(Physics.Raycast(ray, out hit) && hit.transform.tag != "Wall")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void colliderObjectRemover(GameObject gameObject)
    {
        //removes objects from list
        if(objectsInVortex.Contains(gameObject))
        {
            objectsInVortex.Remove(gameObject);
            gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; //sets the attraction to vortex to 0 but keeps rotation
        }
    }
}
