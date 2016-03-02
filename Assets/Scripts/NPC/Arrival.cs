using UnityEngine;
using System.Collections;

[System.Serializable]
public class Arrival : NPCActionable
{   
    
    /// <summary>
	/// The Transform that this steering behavior is targetting
	/// </summary>
	private Transform targetTransform;
    
    /// <summary>
    /// When the entity gets this close to his target, he will start slowing down
    /// </summary>
    public float slowingRadius;
    
    public Arrival(int priority, int id, Transform targetTransform) : base(priority, id)
    {
        this.targetTransform = targetTransform;
    }
    
	public override void Execute(Steerable steerable) 
    {
        base.Execute(steerable);
        
        Debug.Log("Arrival.Execute(): targetTransform: " + targetTransform);
        
        if (targetTransform)
        {
            steerable.AddArrivalForce(targetTransform.position, slowingRadius, strengthMultiplier);
        }
        else
        {
            Debug.Log("Arrival.Execute(): targetTransform is null");
            
            // If the target transform is null, there is nothing to seek. Thus, stop the action
            ActionCompleted();
        }
    }
    
    /// <summary>
	/// The Transform that this steering behavior is targetting
	/// </summary>
	public Transform TargetTransform
    {
        get { return targetTransform; }
        set { targetTransform = value; }
    }
    
}
