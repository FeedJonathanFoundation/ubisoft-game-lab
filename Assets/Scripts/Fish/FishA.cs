using UnityEngine;
using System.Collections;

// Small, easy to kill NPCs
// Run away upon close contact
public class FishA : AbstractFish
{
    public int movementSpeed;
    public int reactionSpeed;
    
    Action movement = new Action(new Wander());
    Action reaction = new Action(new Escape());

    public override void Move(Transform player) 
    {
        movement.think(movementSpeed);
    }
    
    public override void React(Transform player)
    {
        reaction.think(reactionSpeed);
    }
}
