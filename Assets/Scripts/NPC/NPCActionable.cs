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

public abstract class NPCActionable
{
    public int priority;
    public float strengthMultiplier;
    
    public NPCActionable(int priority)
    {
        this.priority = priority;
    }
    
	// void Execute(Steerable steerable, SteeringBehavior steeringBehavior);
    public abstract void Execute(Steerable steerable);
}