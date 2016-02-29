using UnityEngine;
using System.Collections;

public class JellyfishMovement : MonoBehaviour 
{
    [Tooltip("Distance jellyfish travels from its initial ^point")]
    public float jellyfishTravelDistance;
    [Tooltip("Speed of the jellyfish movement")]
    public float jellyfishSpeed;
    private Vector3 jellyfishStart;
    private bool goingUp; //decide if he goes up or down
    private Vector3 movement;
    private Rigidbody parenRigidbody; //make sure the root parent has a rigidbody!!!
	// Use this for initialization
	void Start () 
    {
	   jellyfishStart = this.transform.position;
       goingUp = (Random.value >= 0.5? true : false); //randomly chooses true or false
       parenRigidbody = this.transform.root.GetComponent<Rigidbody>();
       SetVolocity();
	}
	
	// Update is called once per frame
	void Update () 
    {
        float distance = Vector3.Distance(jellyfishStart,this.transform.position);
        if(distance >= jellyfishTravelDistance)
        {
            goingUp = !goingUp;
            SetVolocity();
        }
	}
    
    void SetVolocity()
    {
        if(goingUp)
        {
            movement = Vector3.up * jellyfishSpeed;
        }
        else
        {
            movement = Vector3.down * jellyfishSpeed;
        }
        parenRigidbody.velocity = Vector3.zero; //rest volocity to zero
        parenRigidbody.AddForce(movement * jellyfishSpeed,ForceMode.Force); //set velocity depending on if he is going up or not
    }
}
