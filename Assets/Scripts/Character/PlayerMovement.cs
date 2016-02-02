using UnityEngine;
using System.Collections;

public class PlayerMovement 
{ 
    private Transform massEjectionTransform;
    private GameObject lightBallPrefab;   
    private float thrustForce;
    private Transform transform; 
    private Rigidbody rigidbody;

	/// <summary>
    /// Public constructor
    /// </summary>
    public PlayerMovement(Transform massEjectionTransform, GameObject lightBallPrefab, float thrustForce, Transform transform, Rigidbody rigidbody) 
    {
        this.massEjectionTransform = massEjectionTransform;
        this.lightBallPrefab = lightBallPrefab; 
        this.thrustForce = thrustForce;
        
        this.transform = transform;
        this.rigidbody = rigidbody;
    }
    
    /// <summary>
    /// Makes the character follow the left stick's rotation.
    /// </summary>
    public void FollowLeftStickRotation()
    {
        // Get the direction the left stick is pointing to
        Vector2 leftStickDirection = InputManager.GetLeftStick();
        
        if (leftStickDirection.sqrMagnitude > 0.01f)
        {
            Debug.Log("Move " + Time.time); 
            // Get the angle the left stick is pointing in
            float leftStickAngle = 0.0f;
            if (leftStickDirection.x != 0 || leftStickDirection.y != 0)
            {
                // 90-degree offset to ensure angle is relative to +y-axis
                leftStickAngle = Mathf.Atan2(leftStickDirection.y,leftStickDirection.x) * Mathf.Rad2Deg - 90;
            }
            
            // Make the character face in the direction of the left stick
            transform.localEulerAngles = new Vector3(0,0,leftStickAngle);
        }
    }
    
    /// <summary>
    /// Ejects mass in the given direction, pushing the gameObject in the opposite direction.
    /// </summary>
    public void EjectMass(Vector2 direction)
    {
        // Spawn a light mass at the position of the character's mass ejector
        GameObject lightMass = GameObject.Instantiate(lightBallPrefab);
        lightMass.transform.position = massEjectionTransform.position;
        // Push the light mass in the given direction
        lightMass.GetComponent<Rigidbody>().AddForce(thrustForce * direction, ForceMode.Impulse);
        
        
        // Push the character in the opposite direction that the mass was ejected
        rigidbody.AddForce(-thrustForce * direction, ForceMode.Impulse);
    }
}
