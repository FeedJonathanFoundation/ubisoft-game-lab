﻿public enum NPCActionPriority
{
    Low = 0,
    Medium,
    High
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
	// void Execute(Steerable steerable, SteeringBehavior steeringBehavior);
    public abstract void Execute(Steerable steerable);
}