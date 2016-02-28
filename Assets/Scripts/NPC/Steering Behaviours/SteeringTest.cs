using UnityEngine;
using System.Collections;

/// <summary>
/// The types of steering behaviors a Steerable instance can perform
/// </summary>
public enum SteeringBehaviorType
{
    Seek,
    Arrival,
    Pursue,
    Flee, 
    Evade,
    Wander,
    ObstacleAvoidance,
    WallAvoidance,
    Separation,
    Alignment,
    Cohesion,
    None
}

[RequireComponent(typeof(Steerable))]
public class SteeringTest : MonoBehaviour
{
    /** The steerable component attached to the GameObject performing this action. */
    private Steerable steerable;

    // The steering behaviors to apply every frame
    public SteeringBehavior[] steeringBehaviors;

    // The condition that must be met for this action to return 'Success'
    public StoppingCondition stoppingCondition;

    void Start()
    {
        // Cache the 'Steerable' component attached to the GameObject performing this action
        steerable = transform.GetComponent<Steerable>();

        // Set the stopping condition's transform to the same Transform that is performing this 'Steer' action.
        // This way, the stopping condition will be tested using this GameObject's position
        stoppingCondition.SetTransform(transform); 
        
        // Reset the stopping condition. The stopping condition now knows that the 'Steer' action just started.
        stoppingCondition.Init();
    }

    void FixedUpdate()
    {
        // If the stopping condition has been met, stop moving
        if(stoppingCondition.Complete ())
        {
            return;
        }
        
        // Cycle through the steering behaviors to apply on this GameObject
        for(int i = 0; i < steeringBehaviors.Length; i++)
        {
            // Stores the steering behavior to apply
            SteeringBehavior steeringBehavior = steeringBehaviors[i];

            // Switch the type of the steering behavior and add it to the GameObject
            switch(steeringBehavior.type)
            {
            case SteeringBehaviorType.Seek:
                steerable.AddSeekForce (steeringBehavior.targetTransform.position, steeringBehavior.strengthMultiplier);
                break;
            case SteeringBehaviorType.Arrival:
                steerable.AddArrivalForce (steeringBehavior.targetTransform.position, steeringBehavior.slowingRadius, steeringBehavior.strengthMultiplier);
                break;
            case SteeringBehaviorType.Pursue:
                steerable.AddPursueForce (steeringBehavior.targetSteerable, steeringBehavior.strengthMultiplier);
                break;
            case SteeringBehaviorType.Flee:
                steerable.AddFleeForce (steeringBehavior.targetTransform.position, steeringBehavior.strengthMultiplier);
                break;
            case SteeringBehaviorType.Evade:
                steerable.AddEvadeForce (steeringBehavior.targetSteerable, steeringBehavior.strengthMultiplier);
                break;
            case SteeringBehaviorType.Wander:
                steerable.AddWanderForce (steeringBehavior.circleDistance, steeringBehavior.circleRadius, 
                                          steeringBehavior.angleChange, steeringBehavior.strengthMultiplier);
                break;
            case SteeringBehaviorType.ObstacleAvoidance:
                steerable.AddObstacleAvoidanceForce (steeringBehavior.obstacleAvoidanceForce, steeringBehavior.maxObstacleViewDistance, 
                                                     steeringBehavior.obstacleLayer, steeringBehavior.strengthMultiplier);
                break;
            case SteeringBehaviorType.WallAvoidance:
                steerable.AddWallAvoidanceForce(steeringBehavior.obstacleAvoidanceForce, steeringBehavior.maxObstacleViewDistance,
                                                steeringBehavior.obstacleLayer, steeringBehavior.strengthMultiplier);
                break;
            case SteeringBehaviorType.Separation:
                steerable.AddSeparationForce(steeringBehavior.strengthMultiplier);
                break;
            case SteeringBehaviorType.Alignment:
                steerable.AddAlignmentForce (steeringBehavior.strengthMultiplier);
                break;
            case SteeringBehaviorType.Cohesion:
                steerable.AddCohesionForce (steeringBehavior.strengthMultiplier);
                break;
            }
        }
    
        // Apply the forces on the steerable that is performing this action
        steerable.ApplyForces (Time.fixedDeltaTime);
    }
}

/// <summary>
/// Denotes a steering behavior that a Steerable instance can perform
/// </summary>
[System.Serializable]
public class SteeringBehavior
{
    /// <summary>
    /// Stores the type of steering behavior to apply
    /// </summary>
    public SteeringBehaviorType type;
    /// <summary>
    /// Before the steering force is added, it is multiplied by this value so that all steering behaviors can
    /// be controlled in terms of their impact on the steerable's final velocity.
    /// </summary>
    public float strengthMultiplier = 1.0f;
    
    /// <summary>
    /// The steerable that this steering behavior is targetting
    /// </summary>
    public Steerable targetSteerable;
    /// <summary>
    /// The transform that this steering behavior is targetting
    /// </summary>
    public Transform targetTransform;
    
    /// ......................,
    ///   ARRIVAL PROPERTIES  . 
    /// ......................,
    
    /// <summary>
    /// When the entity gets this close to his target, he will start slowing down. Only applies to the 'Arrival' behavior
    /// </summary>
    [Tooltip("Arrival: the distance from the target before the steerable will start slowing down.")]
    public float slowingRadius;
    
    /// .....................,
    ///   WANDER PROPERTIES  . 
    /// .....................,
    
    /// <summary>
    /// The distance from the entity to the wander circle. The greater this value, the stronger the wander force, 
    /// and the more likely the entity will change directions. 
    /// </summary>
    [Tooltip("Wander: The greater this value, the stronger the wander force, and the more likely the entity will change directions whilst moving.")]
    public float circleDistance;
    /// <summary>
    /// The greater the radius, the stronger the wander force, and the more likely the entity will change directions
    /// </summary>
    [Tooltip("Wander: The greater the radius, the stronger the wander force, and the more likely the entity will change directions")]
    public float circleRadius;
    /// <summary>
    /// The maximum angle in degrees that the wander force can change between two frames
    /// </summary>
    [Tooltip("Wander: The maximum angle in degrees that the wander force can change between two frames")]
    public float angleChange;

    /// .................................,
    ///   OBSTACLE AVOIDANCE PROPERTIES  . 
    /// .................................,

    /// <summary>
    /// The magnitude of the obstacle avoidance force. The higher this value, the faster it is to avoid obstacles and the less
    /// chance there is of hitting them.
    /// </summary>
    [Tooltip("Obstacle/Wall Avoidance: The higher this value, the faster it is to avoid obstacles and the less chance there is of hitting them.")]
    public float obstacleAvoidanceForce;

    /// <summary>
    /// The length (in meters) of the line of sight used for obstacle avoidance. If this value is large, obstacles from very
    /// far away from a steerable can be seen. For instance, if set to '2', obstacles as far as 2 meters from the steerable
    /// can be seen and avoided
    /// </summary>
    [Tooltip("Obstacle/Wall Avoidance: The max look-ahead (in meters) for an incorming obstacle")]
    public float maxObstacleViewDistance;

    /// <summary>
    /// Only colliders on this layer will be avoided if applying the 'Obstacle/WallAvoidance' steering behavior.
    /// </summary>
    [Tooltip("Obstacle/Wall Avoidance: Only colliders on this layer will be avoided")]
    public LayerMask obstacleLayer;
    
}