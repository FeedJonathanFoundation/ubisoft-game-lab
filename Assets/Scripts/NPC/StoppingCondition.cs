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