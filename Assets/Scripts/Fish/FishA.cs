using UnityEngine;
using System.Collections;

// Small, easy to kill NPCs
// Run away upon close contact
public class FishA : AbstractFish
{
    
    // SteeringBehavior steeringBehavior;
    NPCAction steeringBehavior;
    
    NPCAction reactionToPlayer;
    
    // public FishA() { }
    
    // public FishA(NPCAction reactionToPlayer) : base(reactionToPlayer) 
    // {
    //     this.reactionToPlayer = reactionToPlayer;
    // }
    
    public override void Move() 
    {
        // movement.Execute(movementSpeed);
        // if (actionQueue.Count > 0) {
        //     NPCAction current = actionQueue.Dequeue();
        //     current.Execute(steerable);
        // }
        // else {
        // actionQueue.Enqueue(new NPCAction(new Wander()));
        // }
        
        NPCAction wander = new NPCAction(new Wander());
        wander.Execute(steerable);
        // Apply the forces on the steerable that is performing this action
		steerable.ApplyForces (Time.fixedDeltaTime);
    }
    
    public override void ReactToPlayer(Transform player)
    {
        // reactionToPlayer.Execute(steerable);
        NPCAction current = new NPCAction(new Seek());
        current.Execute(steerable);
        
        // Apply the forces on the steerable that is performing this action
		steerable.ApplyForces (Time.fixedDeltaTime);
    }
    
    public override void ReactToNPC(Transform other)
    {
        // reactionToPlayer.Execute(reactionSpeed);
        NPCAction current = new NPCAction(new Seek());
        current.Execute(steerable);
        
        // Apply the forces on the steerable that is performing this action
		steerable.ApplyForces (Time.fixedDeltaTime);
    }
}
