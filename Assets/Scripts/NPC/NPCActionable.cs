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
    
    /** Called when the action is done being performed. The AbstractFish class then knows to stop performing the action. */
    public delegate void ActionCompleteHandler(NPCActionable completedAction);
    public event ActionCompleteHandler ActionComplete = delegate {};
    
    public NPCActionable(int priority, int id)
    {
        this.priority = priority;
        this.id = id;
    }
    
    // Inform the AbstractFish performing this action that the action is complete 
    protected void ActionCompleted()
    {
        ActionComplete(this);
    }
    
	// void Execute(Steerable steerable, SteeringBehavior steeringBehavior);
    public abstract void Execute(Steerable steerable);
    
    public string ToString()
    {
        return base.ToString() + ", Priority = " + priority + ", ID = " + id;
    }
}