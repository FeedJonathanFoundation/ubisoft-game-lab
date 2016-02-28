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
            // If player's lights are on, seek player
            if (targetTransform.gameObject.CompareTag("Player")) 
            {
                Player player = targetTransform.gameObject.GetComponent<Player>();
                if (player.IsDetectable())
                {
                    steerable.AddFleeForce(targetTransform.position, strengthMultiplier);
                }
            }
            else
            {
                steerable.AddFleeForce(targetTransform.position, strengthMultiplier);
            }
        }
    }
    
}
