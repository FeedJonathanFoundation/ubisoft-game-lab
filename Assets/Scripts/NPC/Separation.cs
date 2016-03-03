using UnityEngine;
using System.Collections;

[System.Serializable]
public class Separation : NPCActionable
{       
    public Separation(int priority, int id) : base(priority, id)
    {
    }
    
	public override void Execute(Steerable steerable) 
    {
        base.Execute(steerable);   
        
        steerable.AddSeparationForce(strengthMultiplier);   
    }
    
}
