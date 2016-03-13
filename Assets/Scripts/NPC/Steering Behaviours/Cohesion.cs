using UnityEngine;
using System.Collections;

[System.Serializable]
public class Cohesion : NPCActionable
{       
    public Cohesion(int priority, string id) : base(priority, id)
    {
    }
    
	public override void Execute(Steerable steerable) 
    {
        base.Execute(steerable);
        
        steerable.AddCohesionForce(strengthMultiplier);
    }
    
}
