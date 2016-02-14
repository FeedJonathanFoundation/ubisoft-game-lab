using UnityEngine;
using System.Collections;

[System.Serializable]
public class Seek : NPCActionable
{   
    private Transform targetTransform;
    
    public float strengthMultiplier = 9.9f;
    
    public Seek(int priority, Transform transform) 
    {
        this.priority = priority;
        targetTransform = transform;
    }
    
	public override void Execute(Steerable steerable) 
    {
        steerable.AddSeekForce (targetTransform.position, strengthMultiplier);
    }
    
}
