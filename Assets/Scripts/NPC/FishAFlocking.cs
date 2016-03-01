using UnityEngine;
using System.Collections;

[System.Serializable]
public class FishAFlocking : NPCActionable
{
    [SerializeField]
    private Wander wander;
    [SerializeField]
    private WallAvoidance wallAvoidance;
    [SerializeField]
    private Alignment alignment;
    [SerializeField]
    private Cohesion cohesion;
    [SerializeField]
    private Separation separation;
    
    public FishAFlocking(int priority, int id) : base(priority, id)
    {
        wander = new Wander(priority, id);
        wallAvoidance = new WallAvoidance(priority, id);
    }
    
    /// <summary>
    /// Sets the properties for the Wander steering behaviour
    /// <param name="strengthMultiplier">The amount by which the seek force is multiplied before being added to the steerable's steering force
    /// <param name="circleDistance">The distance from the entity to the "wander" circle. The greater this value,
    /// the stronger the wander force, and the more likely the entity will change directions.</param>
    /// <param name="circleRadius">The greater this radius, the stronger the wander force, and the more likely
    /// the entity will change directions.</param>
    /// <param name="angleChange">The maximum angle in degrees that the wander force can change next frame.</param>
    /// </summary>
    public void SetWanderProperties(float strengthMultiplier, float circleRadius, float circleDistance, float angleChange)
    {
        wander.strengthMultiplier = strengthMultiplier;
        wander.circleRadius = circleRadius;
        wander.circleDistance = circleDistance;
        wander.angleChange = angleChange;
    }
    
    /// <summary>
    /// Avoids the nearest obstacle in front of the object. This works for obstacles of any size or shape, unlike
    /// Obstacle Avoidance, which approximates obstacles as spheres. Formal name: "Containment" or "Generalized Obstacle Avoidance"
    /// </summary>
    /// <param name="avoidanceForce">The amount of force applied in order to avoid the nearest obstacle.</param>
    /// <param name="maxViewDistance">Only obstacles within 'maxViewDistance' meters of this steerable can be avoided.</param>
    /// <param name="obstacleLayer">The layer which contains the colliders that can be avoided.</param>
    public void SetWallAvoidanceProperties(float strengthMultiplier, float avoidanceForce, float maxViewDistance, LayerMask obstacleLayer)
    {
        wallAvoidance.strengthMultiplier = strengthMultiplier;
        wallAvoidance.avoidanceForce = avoidanceForce;
        wallAvoidance.maxViewDistance = maxViewDistance;
        wallAvoidance.obstacleLayer = obstacleLayer;
    }
    
    public void SetAlignmentProperties(float strengthMultiplier)
    {
        alignment.strengthMultiplier = strengthMultiplier;
    }
    
    public void SetCohesionProperties(float strengthMultiplier)
    {
        cohesion.strengthMultiplier = strengthMultiplier;
    }
    
    public void SetSeparationProperties(float strengthMultiplier)
    {
        separation.strengthMultiplier = strengthMultiplier;
    }
    
    public override void Execute(Steerable steerable)
    {
        wander.Execute(steerable);
        wallAvoidance.Execute(steerable);
        alignment.Execute(steerable);
        cohesion.Execute(steerable);
        separation.Execute(steerable);
    }
} 