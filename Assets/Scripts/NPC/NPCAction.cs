using UnityEngine;
using System.Collections;

public class NPCAction : MonoBehaviour 
{
    private NPCActionable executableStrategy;
    
    // Assigns strategy to interface
    public NPCAction(NPCActionable strategy)
    {
        this.executableStrategy = strategy;
    }

    // Executes the strategy
    public void Execute(int speed)
    {
        executableStrategy.Execute(speed);
    }
}

