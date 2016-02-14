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
        PushAction(GetID(), new Wander());
    }
    
    public override void ReactToPlayer(Transform player)
    {
        // reactionToPlayer.Execute(steerable);
        Seek seek = new Seek(player);
        PushAction(-1, seek);
    }
    
    public override void ReactToNPC(Transform other)
    {
        // reactionToPlayer.Execute(reactionSpeed);
        Seek seek = new Seek(other);
        PushAction(-2, seek);
    }
}
