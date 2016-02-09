using UnityEngine;
using System.Collections;

// Small, easy to kill NPCs
// Run away upon close contact
public class FishA : AbstractFish
{
    
    NPCAction reactionToPlayer;
    
    public FishA() { }
    
    public FishA(NPCAction reactionToPlayer) : base(reactionToPlayer) 
    {
        this.reactionToPlayer = reactionToPlayer;
    }
    
    public override void Move() 
    {
        // movement.Execute(movementSpeed);
    }
    
    public override void ReactToPlayer(Transform player)
    {
        reactionToPlayer.Execute(reactionSpeed);
    }
    
    public override void ReactToNPC(Transform other)
    {
        reactionToPlayer.Execute(reactionSpeed);
    }
}
