using UnityEngine;
using System.Collections;

[System.Serializable]
public class Pursue : NPCActionable
{   
    private Transform targetTransform;
    
    /// <summary>
	/// The steerable that this steering behavior is targetting
	/// </summary>
	public Steerable targetSteerable;
    
    public Pursue(int priority, Steerable targetSteerable) : base(priority)
    {
        this.targetSteerable = targetSteerable;
    }
    
	public override void Execute(Steerable steerable) 
    {
        if (targetSteerable)
        {
            steerable.AddPursueForce(targetSteerable, strengthMultiplier);
        }
    }
    
}
