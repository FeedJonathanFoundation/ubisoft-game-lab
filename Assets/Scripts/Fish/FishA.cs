using UnityEngine;
using System.Collections;

// Small, easy to kill NPCs
// Run away upon close contact
public class FishA : AbstractFish
{
    [SerializeField]
    private FishAFlocking flockingBehaviour;
    
    [Tooltip("Then action performed when the fish detects the player")]
    [SerializeField]
    private SeekOrFleeLight playerBehaviour;
    
    public override void Awake()
    {
        // call parent LightSource Awake() first
        base.Awake(); 
        
        flockingBehaviour.SetPriority(0);   // Lowest priority
        flockingBehaviour.SetID(GetID());
        
        playerBehaviour.SetPriority(2);     // Highest priority
        playerBehaviour.SetID(-1);
        playerBehaviour.Init();
    }
    
    public override void Move() 
    {
        AddAction(flockingBehaviour);
    }
    
    // Called every frame when the fish can see the player
    public override void ReactToPlayer(Transform player)
    {        
        // Flee flee = new Flee(1, -1, player);
        // flee.strengthMultiplier = 50f;
        // flee.overrideSteerableSpeed = true;
        // flee.minSpeed = 6f;
        // flee.maxSpeed = 6f;
        
        playerBehaviour.TargetLightSource = player.GetComponent<LightSource>();
        AddAction(playerBehaviour);
    }
    
    public override void ReactToNPC(Transform other)
    {
        /*AbstractFish fish = other.gameObject.GetComponent<AbstractFish>();
        int id = fish.GetID();
        Flee flee = new Flee(1, other);
        actions.InsertAction(id, flee);*/
    }
}