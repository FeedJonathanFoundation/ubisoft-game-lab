using UnityEngine;
using System.Collections;

[System.Serializable]
public class Alignment : NPCActionable
{       
    public Alignment(int priority, int id) : base(priority, id)
    {
    }
    
	public override void Execute(Steerable steerable) 
    {
        steerable.AddAlignmentForce(strengthMultiplier);
    }
    
}