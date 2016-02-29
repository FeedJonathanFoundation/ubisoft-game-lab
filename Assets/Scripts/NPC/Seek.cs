using UnityEngine;
using System.Collections;

[System.Serializable]
public class Seek : NPCActionable
{   
    private Transform targetTransform;
    
    public Seek(int priority, Transform transform) : base(priority)
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
                    steerable.AddSeekForce(targetTransform.position, strengthMultiplier);
                }
            }
            else
            {
                steerable.AddSeekForce(targetTransform.position, strengthMultiplier);
            }
            
        }
    }
}
