using UnityEngine;
using System.Collections;

[System.Serializable]
public class Flocking : NPCActionable
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
    
    public Flocking(int priority, string id) : base(priority, id)
    {
        this.SetPriority(priority);
        this.SetID(id);
    }
    
    public void SetPriority(int priority)
    {
        this.priority = priority;
        
        wander.priority = priority;
        wallAvoidance.priority = priority;
        alignment.priority = priority;
        cohesion.priority = priority;
        separation.priority = priority;
    }
    
    public void SetID(string id)
    {
        this.id = id;
        
        wander.id = id;
        wallAvoidance.id = id;
        alignment.id = id;
        cohesion.id = id;
        separation.id = id;
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
        
        cohesion.Execute(steerable);
        alignment.Execute(steerable);
        separation.Execute(steerable);
        wallAvoidance.Execute(steerable);
        wander.Execute(steerable);
    }
} 