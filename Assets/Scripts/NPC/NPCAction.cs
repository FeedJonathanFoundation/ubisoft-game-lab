using UnityEngine;
using System.Collections;

/// <summary>
/// The types of steering behaviors a Steerable instance can perform
/// </summary>


[RequireComponent(typeof(Steerable))]
public class NPCAction 
{
    private NPCActionable executableStrategy;
    
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