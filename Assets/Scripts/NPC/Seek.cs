using UnityEngine;
using System.Collections;

[System.Serializable]
public class Seek : NPCActionable
{   
    private Transform targetTransform;
    
    public float strengthMultiplier = 9.9f;
    
    public Seek(Transform transform)
    {
        targetTransform = transform;
    }
    
	public override void Execute(Steerable steerable) 
    {
        //targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        steerable.AddSeekForce (targetTransform.position, strengthMultiplier);
    }
}
