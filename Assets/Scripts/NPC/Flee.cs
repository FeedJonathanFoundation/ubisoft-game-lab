using UnityEngine;
using System.Collections;

[System.Serializable]
public class Flee : NPCActionable
{   
    private Transform targetTransform;
    
    public Flee(int priority, int id, Transform transform) : base(priority, id)
    {
        targetTransform = transform;
    }
    
	public override void Execute(Steerable steerable) 
    {
        if (targetTransform)
        {            
            // If player's lights are on, seek player
            if (targetTransform.gameObject.CompareTag("Player")) 
            {
                Player player = targetTransform.gameObject.GetComponent<Player>();
                if (player.IsDetectable())
                {
                    // Override the steerable's min/max speed
                    if (overrideSteerableSpeed)
                    {
                        steerable.MinSpeed = minSpeed;
                        steerable.MaxSpeed = maxSpeed;
                    }
                    
                    steerable.AddFleeForce(targetTransform.position, strengthMultiplier);
                }
                else
                {
                    // The player is hidden. Thus, the fish should stop fleeing
                    //ActionCompleted();
                }
            }
            else
            {
                steerable.AddFleeForce(targetTransform.position, strengthMultiplier);
            }
        }
    }
    
}
