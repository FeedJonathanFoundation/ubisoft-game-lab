using UnityEngine;
using System.Collections;

public class JellyfishMovement : MonoBehaviour 
{
    public float jellyfishTravelDistance;
    public float jellyfishSpeed;
    private Vector3 jellyfishStart;
    private bool goingUp;
    private Vector3 mouvement;
    private Rigidbody parenRigidbody; //make sure the root parent has a rigidbody!!!
	// Use this for initialization
	void Start () 
    {
	   jellyfishStart = this.transform.position;
       goingUp = false;
       parenRigidbody = this.transform.root.GetComponent<Rigidbody>();
       SetVolocity();
	}
	
	// Update is called once per frame
	void Update () 
    {
        float distance = Vector3.Distance(jellyfishStart,this.transform.position);
        Debug.Log(distance.ToString());
        if(distance >= jellyfishTravelDistance)
        {
            Debug.Log("distance >=");
            goingUp = !goingUp;
            SetVolocity();
        }
	}
    
    void SetVolocity()
    {
        if(goingUp)
        {
            Debug.Log("up");
            mouvement = Vector3.up * jellyfishSpeed;
        }
        else
        {
            Debug.Log("down");
            mouvement = Vector3.down * jellyfishSpeed;
        }
        Debug.Log(mouvement.ToString());
        parenRigidbody.velocity = Vector3.zero;
        parenRigidbody.AddForce(mouvement * jellyfishSpeed,ForceMode.Force);
    }
}
