using UnityEngine;
using System.Collections;

// Medium, hard to kill NPCs
// Seeks smaller fish by default
// Run away from large fish upon close contact
public class FishB : AbstractFish
{
    public override void Awake()
    {
        // call parent LightSource Awake() first
        base.Awake();
    }

    public override void Move() 
    {
        actions.InsertAction(GetID(), new Wander(0));
    }
    
    public override void ReactToPlayer(Transform player)
    {
        Seek seek = new Seek(1, player);
        actions.InsertAction(-1, seek);
    }
    
    public override void ReactToNPC(Transform other)
    {
        // Check size, then decide whether or not to eat
        AbstractFish fish = other.gameObject.GetComponent<AbstractFish>();
        if (fish != null)
        {
            int id = fish.GetID();
            Seek seek = new Seek(1, other);
            actions.InsertAction(id, seek);
        }
        
        // Flee flee = new Flee(2, other);
        // actions.InsertAction(id, seek);
    }
}