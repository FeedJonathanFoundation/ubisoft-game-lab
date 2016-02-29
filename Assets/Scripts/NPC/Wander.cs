using UnityEngine;
using System.Collections;

[System.Serializable]
public class Wander : NPCActionable 
{
    
    /// <summary>
	/// The distance from the entity to the wander circle. The greater this value, the stronger the wander force, 
	/// and the more likely the entity will change directions. 
	/// </summary>
    [Tooltip("Wander: The greater this value, the stronger the wander force, and the more likely the entity will change directions whilst moving.")]
	public float circleDistance = 1f;
    
	/// <summary>
	/// The greater the radius, the stronger the wander force, and the more likely the entity will change directions
	/// </summary>
    [Tooltip("Wander: The greater the radius, the stronger the wander force, and the more likely the entity will change directions")]
	public float circleRadius = .5f;
    
	/// <summary>
	/// The maximum angle in degrees that the wander force can change between two frames
	/// </summary>
    [Tooltip("Wander: The maximum angle in degrees that the wander force can change between two frames")]
	public float angleChange = 30f;
    
    public Wander(int priority, int id) : base(priority, id) 
    {
    }
    
    // Need to be able to set these floats elsewhere;
    
	public override void Execute(Steerable steerable) 
    {        
        steerable.AddWanderForce(circleDistance, circleRadius, angleChange, strengthMultiplier);
    }
}
