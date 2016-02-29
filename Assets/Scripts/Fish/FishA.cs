using UnityEngine;
using System.Collections;

// Small, easy to kill NPCs
// Run away upon close contact
public class FishA : AbstractFish
{
    public override void Awake()
    {
        // call parent LightSource Awake() first
        base.Awake(); 
    }
    

    public override void Move() 
    {
        //actions.InsertAction(GetID(), new Wander(0));
    }
    
    public override void ReactToPlayer(Transform player)
    {
        Flee flee = new Flee(1, -1, player);
        flee.strengthMultiplier = 20f;
        AddAction(flee);
    }
    
    public override void ReactToNPC(Transform other)
    {
        /*AbstractFish fish = other.gameObject.GetComponent<AbstractFish>();
        int id = fish.GetID();
        Flee flee = new Flee(1, other);
        actions.InsertAction(id, flee);*/
    }
}