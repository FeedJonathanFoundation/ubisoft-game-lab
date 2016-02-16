using UnityEngine;
using System.Collections;

// Large, impossible to kill NPCs
public class FishC : AbstractFish
{
    
    public override void Awake()
    {
        base.Awake(); // call parent LightSource Awake() first
    }

    public override void Move() 
    {
        actions.InsertAction(GetID(), new Wander(0));
        // add obstacle avoidance
    }
    
    public override void ReactToPlayer(Transform player)
    {
        Seek seek = new Seek(1, player);
        actions.InsertAction(-1, seek);
    }
    
    public override void ReactToNPC(Transform other)
    {
        AbstractFish fish = other.gameObject.GetComponent<AbstractFish>();
        int id = fish.GetID();
        Seek seek = new Seek(1, other);
        actions.InsertAction(id, seek);
        
        // add algorithm to decide which fish to ignore
    }
}