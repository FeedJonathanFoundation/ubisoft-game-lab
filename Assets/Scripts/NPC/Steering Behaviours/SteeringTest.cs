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
    [Tooltip("Obstacle Avoidance: The higher this value, the faster it is to avoid obstacles and the less chance there is of hitting them.")]
	public float obstacleAvoidanceForce;

	/// <summary>
	/// The length (in meters) of the line of sight used for obstacle avoidance. If this value is large, obstacles from very
	/// far away from a steerable can be seen. For instance, if set to '2', obstacles as far as 2 meters from the steerable
	/// can be seen and avoided
	/// </summary>
    [Tooltip("Obstacle Avoidance: The max look-ahead (in meters) for an incorming obstacle")]
	public float maxObstacleViewDistance;

	/// <summary>
	/// Only colliders on this layer will be avoided if applying the 'ObstacleAvoidance' steering behavior.
	/// </summary>
    [Tooltip("Obstacle Avoidance: Only colliders on this layer will be avoided")]
	public LayerMask obstacleLayer;
	
}

/// <summary>
/// The condition that must be met for a 'Steer' action to return 'Success'
/// </summary>
[System.Serializable]
public class StoppingCondition
{
	/** The margin for error when comparing floats */
	private const float epsilon = 0.2f;

	[Tooltip("The target that the steerable must reach for the 'Steer' action to return success.")]
	public Transform target;

	[Tooltip("The distance this steerable must be from his target for the 'Steer' action to return success.")]
	public float stoppingDistance;

	/** Caches the squared stopping distance for efficiency purposes. */
	private float stoppingDistanceSquared;

	[Tooltip("Once the 'Steer' action is active, 'waitTime' seconds will elapse before" +
		"the action returns 'Success'. If this is set to zero, the 'Steer' action will run until the target is reached.")]
	public float waitTime;

	/** The amount of time that has passed since the 'Steer' action started. */
	private float timeElapsed;

	/** The steerable's previous position. Used to determine if the steerable passed his stopping distance. */
	private Vector2 previousPosition;

	/** The Transform component that is performing the 'Steer' action this instance belongs to. The stopping condition
	 *  tested against this Transform's position. */
	private Transform transform;

	/// <summary>
	/// Call this every frame that the 'Steer.OnUpdate()' function is called. The stopping condition is updated
	/// so that the 'Steer' action can stop at the right time
	/// </summary>
	public void Update()
	{
		// Update the amount of time that the 'Steer' action was active for
		timeElapsed += Time.deltaTime;
	}

	/// <summary>
	/// Initializes the stopping condition whenever the 'Steer' action starts.
	/// </summary>
	public void Init()
	{
		// Caches the squared stopping distance for efficiency purposes
		stoppingDistanceSquared = stoppingDistance * stoppingDistance;

		// Update the steerable's previous position
		previousPosition = transform.position;

		// Reset the amount of time elapsed to zero.
		timeElapsed = 0;
	}

	/// <summary>
	/// Returns true if the stopping condition has been met and the 'Steer' action should return 'Success'
	/// </summary>
	public bool Complete()
	{
		// If the stopping condition is met when the GameObject reaches his target
		if(target != null)
		{
			// Compute the squared distance between this GameObject and his target.
			float distanceToTargetSqr = (transform.position - target.position).sqrMagnitude;
			float previousDistanceToTargetSqr = (previousPosition - (Vector2)target.position).sqrMagnitude;

			// If the steerable has reached his stopping distance
			if((distanceToTargetSqr >= stoppingDistanceSquared && previousDistanceToTargetSqr <= stoppingDistanceSquared)
			   || (distanceToTargetSqr <= stoppingDistanceSquared && previousDistanceToTargetSqr >= stoppingDistanceSquared))
			{
				// Return true, since the stopping condition has been met
				return true;
			}

			if(distanceToTargetSqr <= stoppingDistanceSquared)
				return true;	

			// FAILSAFE: If this GameObject is 'stoppingDistance' units away from his target
			if(Mathf.Abs (distanceToTargetSqr - stoppingDistanceSquared) <= epsilon)
			{
				// Return true, since the stopping condition has been met
				return true;
			}
		}

		// If the wait time is zero, the 'Steer' action can run for an infinite amount of time. If it is nonzero, the 
		// 'Steer' action can be performed for at most 'waitTime' seconds.
		if(waitTime != 0)
		{	
			// If at least 'waitTime' seconds have elapsed since the 'Steer' action started
			if(timeElapsed >= waitTime)
			{
				// Return true, since the stopping condition was met, and the 'Steer' action this stopping condition controls should stop
				return true;
			}
		}

		// Update the steerable's previous position for the next frame.
		previousPosition = transform.position;

		// If this statement is reached, the stopping condition has not yet been met. Thus, return 'false'
		return false;
	}

	/// <summary>
	/// Sets the transform whose position is tested against the 'target' Transform to test if the stopping condition is met.
	/// Set this to the same Transform that is performing the 'Seek' action.
	/// </summary>
	public void SetTransform(Transform transform)
	{
		this.transform = transform;
	}
}