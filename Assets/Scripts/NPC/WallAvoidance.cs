using UnityEngine;
using System.Collections;

[System.Serializable]
public class WallAvoidance : NPCActionable
{   
    /** The amount of force applied in order to avoid the nearest obstacle. */
    public float avoidanceForce;
    /** Only obstacles within 'maxViewDistance' meters of the steerable can be avoided */
    public float maxViewDistance;
    /** The layer which contains the colliders that can be avoided. */
    public LayerMask obstacleLayer;
    
    public WallAvoidance(int priority, int id) : base(priority, id)
    {
    }
    
	public override void Execute(Steerable steerable) 
    {
        steerable.AddWallAvoidanceForce(avoidanceForce, maxViewDistance, obstacleLayer, strengthMultiplier);
    }
    
}
