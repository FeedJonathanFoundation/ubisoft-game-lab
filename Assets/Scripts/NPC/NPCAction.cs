using UnityEngine;
using System.Collections;

/// <summary>
/// The types of steering behaviors a Steerable instance can perform
/// </summary>
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

[RequireComponent(typeof(Steerable))]
public class NPCAction : MonoBehaviour 
{
    private NPCActionable executableStrategy;
    private NPCActionType actionType;
    
    // Assigns strategy to interface
    public NPCAction(NPCActionable strategy)
    {
        this.executableStrategy = strategy;
        
    }

    // Executes the strategy
    public void Execute(Steerable steerable)
    {
        executableStrategy.Execute(steerable);
    }
    
    // stopping condition / check if valid
    
}