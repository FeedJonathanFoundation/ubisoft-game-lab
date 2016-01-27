using UnityEngine;
using System.Collections;

// Small, easy to kill NPCs
// Run away upon close contact
public class FishA : AbstractFish
{

    public override void Move(Transform player) 
    {
        // swim
    }
    
    public override void React(Transform player)
    {
        // escape
    }
}
