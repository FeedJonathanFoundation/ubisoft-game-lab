using UnityEngine;
using System.Collections;

[System.Serializable]
public class SeekFlare : NPCActionable
{    
    /// <summary>
	/// The light that this NPC will seek
	/// </summary>
    private Transform targetFlare;

    /// <summary>
    /// The arrival (seek) action performed when the NPC is seeking the light.
    /// </summary>
    [SerializeField]
    private Arrival arrivalForce;
    
    [SerializeField]
    private WallAvoidance wallAvoidance;
    
    public SeekFlare(int priority, string id, Transform targetFlare) : base(priority, id)
    {
        this.targetFlare = targetFlare;
        
        SetPriority(priority);
        SetID(id);
    }
    
    /** Call this method in the Start() function of the fish performing this action. */
    public void Init()
    {
        // Call ChildActionComplete() when either the seek or flee actions are completed.
        arrivalForce.ActionComplete += ChildActionComplete;
        wallAvoidance.ActionComplete += ChildActionComplete;
    }
    
    public void SetPriority(int priority)
    {
        this.priority = priority;
        
        arrivalForce.priority = priority;
        wallAvoidance.priority = priority;
    }
    
    public void SetID(string id)
    {
        this.id = id;
        
        arrivalForce.id = id;
        wallAvoidance.id = id;
    }
    
	public override void Execute(Steerable steerable) 
    {
        base.Execute(steerable);
        
        if (targetFlare)
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
            
            // Seek the light source
            arrivalForce.Execute(steerable);
        }
        else
        {
            // If the flare has been destroyed, stop seeking it
            ActionCompleted();
        }
        
        wallAvoidance.Execute(steerable);
    }
    
    public override bool CanBeCancelled()
    {
        return arrivalForce.CanBeCancelled() && wallAvoidance.CanBeCancelled();
    }
    
    private void ChildActionComplete(NPCActionable childAction)
    {
        // Call ActionCompleted() to notify subscribers that the parent action is complete
        ActionCompleted();
    }
    
    /// <summary>
	/// The light that this NPC will seek or flee
	/// </summary>
    public Transform TargetFlare
    {
        get { return targetFlare; }
        set 
        { 
            targetFlare = value; 
            
            arrivalForce.TargetTransform = value.transform;
        }
    }
    
}
