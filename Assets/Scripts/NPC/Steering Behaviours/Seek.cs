using UnityEngine;
using System.Collections;

[System.Serializable]
public class Seek : NPCActionable
{   
    private Transform targetTransform;
    
    public Seek(int priority, int id, Transform transform) : base(priority, id)
    {
        targetTransform = transform;
    }
    
	public override void Execute(Steerable steerable) 
    {
        base.Execute(steerable);
        
        if (targetTransform)
        {
            // Override the steerable's min/max speed
            if (overrideSteerableSpeed)
            {
                steerable.MinSpeed = minSpeed;
                steerable.MaxSpeed = maxSpeed;
            }
            // Override the steerable's max force
            if (overrideMaxForce)
            {
                steerable.MaxForce = maxForce;
            }
            
            // If player's lights are on, seek player
            if (targetTransform.gameObject.CompareTag("Player")) 
            {
                Player player = targetTransform.gameObject.GetComponent<Player>();
                if (player.IsDetectable())
                {
                    steerable.AddSeekForce(targetTransform.position, strengthMultiplier);
                }
            }
            else
            {
                steerable.AddSeekForce(targetTransform.position, strengthMultiplier);
            }
            
        }
    }
    
    /// <summary>
    /// The transform to seek
    /// </summary>
    public Transform TargetTransform
    {
        get { return targetTransform; }
        set { targetTransform = value; }
    }
}
