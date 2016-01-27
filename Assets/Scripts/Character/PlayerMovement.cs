using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour 
{
    /// <summary>
    /// The position at which the character ejects mass. 
    /// </summary> 
    public Transform massEjectionTransform;
    
    /// <summary>
    /// The amount of force applied on the player when ejecting one piece of mass.
    /// </summary>
    public float thrustForce;

    /** Caches the player's components */
    private new Rigidbody rigidbody;

	// Use this for initialization
	void Start () 
    {
	   rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	   if(Input.GetButtonDown("Thrust"))
       {
           
       }
       
       
           
       Debug.Log("Eject Mass in direction: " + InputManager.GetLeftStick());

       

	}
    
    /// <summary>
    /// Ejects mass in the given direction, pushing the gameObject in the opposite direction.
    /// </summary>
    private void EjectMass(Vector2 direction)
    {
        
        
        // Push the character in the opposite direction that the mass was ejected
        rigidbody.AddForce(-thrustForce * direction, ForceMode.Impulse);
    }
}
