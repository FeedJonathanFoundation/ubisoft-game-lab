using UnityEngine;
using System.Collections;

[System.Serializable]
public class Flee : NPCActionable
{   
    private Transform targetTransform;
    
    public float strengthMultiplier = 9.9f;
    
    public Flee(int priority, Transform transform) : base(priority)
    {
        targetTransform = transform;
    }
    
	public override void Execute(Steerable steerable) 
    {
        if (targetTransform)
        {
            steerable.AddFleeForce(targetTransform.position, strengthMultiplier);
        }
    }
    
}
