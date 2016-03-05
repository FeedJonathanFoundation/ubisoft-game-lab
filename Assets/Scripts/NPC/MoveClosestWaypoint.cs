using UnityEngine;
using System.Collections;

[System.Serializable]
public class MoveClosestWaypoint : NPCActionable
{
    [Tooltip("The list of waypoints that the big fish can go when the player is in a safe zone")]
    public GameObject waypointList;
    [Tooltip("Distance that big fish slows down before getting to waypoint")]
    public float slowingRadius;
    private Transform bigFish;

    [SerializeField]
    private WallAvoidance wallAvoidance;
    
    public MoveClosestWaypoint(int priority, int id, Transform boss) : base(priority, id)
    {
        SetPriority(priority);
        SetID(id);
        SetBigFish(boss);
    }
    
    public void SetPriority(int priority)
    {
        this.priority = priority;
        wallAvoidance.priority = priority;
    }
    
    public void SetID(int id)
    {
        this.id = id;
    }
    
    public void SetBigFish(Transform boss)
    {
        this.bigFish = boss;
    }
    
    public void SetWallAvoidanceProperties(float strengthMultiplier, float avoidanceForce, float maxViewDistance, LayerMask obstacleLayer)
    {
        wallAvoidance.strengthMultiplier = strengthMultiplier;
        wallAvoidance.avoidanceForce = avoidanceForce;
        wallAvoidance.maxViewDistance = maxViewDistance;
        wallAvoidance.obstacleLayer = obstacleLayer;
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
        
        if((Vector2)bigFish.GetComponent<Rigidbody>().velocity == Vector2.zero)
        {
            ActionCompleted();
        }
        else
        {
            steerable.AddMoveWaypointForce(waypointList, bigFish, slowingRadius, strengthMultiplier);
        }
        
        wallAvoidance.Execute(steerable);
    }
} 