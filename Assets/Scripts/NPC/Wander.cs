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
	public float circleDistance;
    
	/// <summary>
	/// The greater the radius, the stronger the wander force, and the more likely the entity will change directions
	/// </summary>
    [Tooltip("Wander: The greater the radius, the stronger the wander force, and the more likely the entity will change directions")]
	public float circleRadius;
    
	/// <summary>
	/// The maximum angle in degrees that the wander force can change between two frames
	/// </summary>
    [Tooltip("Wander: The maximum angle in degrees that the wander force can change between two frames")]
	public float angleChange;
    
    public float strengthMultiplier = 9.9f;
    
    public Wander(int priority) : base(priority) 
    {
    }
    
    // Need to be able to set these floats elsewhere;
    
	public override void Execute(Steerable steerable) 
    {
        circleDistance = 1f;
        circleRadius = .5f;
        angleChange = 30f;
        
        steerable.AddWanderForce(circleDistance, circleRadius, angleChange, strengthMultiplier);
    }
}
