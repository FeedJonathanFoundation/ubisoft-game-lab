using UnityEngine;
using System.Collections;

public enum NPCActionPriority
{
    Low = 0,
    Medium = 1,
    High = 2
}

public enum NPCActionType
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

[System.Serializable]
public abstract class NPCActionable
{
    [System.NonSerialized]
    public int priority;
    [System.NonSerialized]
    public int id;
    public float strengthMultiplier;
    
    public bool overrideSteerableSpeed = false; // If true, the Steerable performing this action is given a new min/maxSpeed 
    public float minSpeed;
    public float maxSpeed;
    
    public bool overrideMaxForce = false;
    public float maxForce;
    
    protected float timeActive;
    
    /** Called when the action is done being performed. The AbstractFish class then knows to stop performing the action. */
    public delegate void ActionCompleteHandler(NPCActionable completedAction);
    public event ActionCompleteHandler ActionComplete = delegate {};
    
    public NPCActionable(int priority, int id)
    {
        this.priority = priority;
        this.id = id;
    }
    
    // Inform the AbstractFish performing this action that the action is complete 
    protected virtual void ActionCompleted()
    {
        //if (ActionComplete != null)
            // Notify subscribers that the action is complete
            ActionComplete(this);
    }
    
    protected void ResetTimer()
    {
         // Reset the timer for the next time this action is performed.
        timeActive = 0;
    }
    
    // Returns true if the action can be stopped. Should return "false" if the action's timer is ongoing.
    public virtual bool CanBeCancelled()
    {
        return true;
    }
    
	// void Execute(Steerable steerable, SteeringBehavior steeringBehavior);
    public virtual void Execute(Steerable steerable)
    {
        timeActive += Time.deltaTime;
    }
    
    public new string ToString()
    {
        return base.ToString() + ", Priority = " + priority + ", ID = " + id;
    }
}