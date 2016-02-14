﻿using UnityEngine;
using System.Collections;

// Small, easy to kill NPCs
// Run away upon close contact
public class FishA : AbstractFish
{

    public override void Move() 
    {
        PushAction(GetID(), new Wander(0));
    }
    
    public override void ReactToPlayer(Transform player)
    {
        Seek seek = new Seek(1, player);
        PushAction(-1, seek);
    }
    
    public override void ReactToNPC(Transform other)
    {
        AbstractFish fish = other.gameObject.GetComponent<AbstractFish>();
        int id = fish.GetID();
        Seek seek = new Seek(1, other);
        PushAction(id, seek);
    }
}
