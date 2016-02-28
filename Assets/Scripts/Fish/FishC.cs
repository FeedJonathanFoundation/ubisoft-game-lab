using UnityEngine;
using System.Collections;

// Large, impossible to kill NPCs
// Seeks smaller fish by default
public class FishC : AbstractFish
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
        /*Seek seek = new Seek(1, player);
        actions.InsertAction(-1, seek);*/
    }
    
    public override void ReactToNPC(Transform other)
    {
        // implement passive eating of smaller NPCs??
        /*Seek seek = new Seek(1, other);
        actions.InsertAction(-1, seek);*/
    }
}