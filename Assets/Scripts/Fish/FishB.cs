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
        AddAction(new Wander(0, GetID()));
    }
    
    public override void ReactToPlayer(Transform player)
    {
        Debug.Log("Fish B React to Player()");
        Seek seek = new Seek(1, -1, player);
        AddAction(seek);
    }
    
    public override void ReactToNPC(Transform other)
    {
        // Check size, then decide whether or not to eat
        /*AbstractFish fish = other.gameObject.GetComponent<AbstractFish>();
        int id = fish.GetID();
        Seek seek = new Seek(1, other);
        actions.InsertAction(id, seek);*/
        
        // Flee flee = new Flee(2, other);
        // actions.InsertAction(id, seek);
    }
}