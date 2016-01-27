using UnityEngine;
using System.Collections;

class Action : MonoBehaviour 
{
    private NPCActionable thinkStrategy;
    
    // Assigns strategy to interface
    public Action(NPCActionable strategy)
    {
        this.thinkStrategy = strategy;
    }

    // Executes the strategy
    public void think(int speed)
    {
        thinkStrategy.think(speed);
    }

}
