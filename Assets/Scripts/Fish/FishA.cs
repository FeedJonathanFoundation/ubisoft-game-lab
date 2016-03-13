using UnityEngine;
using System.Collections;

// Small, easy to kill NPCs
// Run away upon close contact
public class FishA : AbstractFish
{
    [SerializeField]
    private Flocking flockingBehaviour;
    
    [Tooltip("Then action performed when the fish detects the player")]
    [SerializeField]
    private SeekOrFleeLight playerBehaviour;

    [Tooltip("The action performed when flare is within the fish's line of sight")]    
    [SerializeField]
    private SeekFlare flareBehaviour;
    
    protected override void Awake()
    {        
        base.Awake(); 
                                      
        playerBehaviour.SetPriority(1);     // Medium priority
        playerBehaviour.SetID("-1");
        playerBehaviour.Init();
        
        flareBehaviour.SetPriority(2);      // Highest priority
        flareBehaviour.SetID("-2");
        flareBehaviour.Init();
    }
    
    public override void Move() 
    {
        flockingBehaviour.SetPriority(0);   // Lowest priority
        flockingBehaviour.SetID(this.LightSourceID);
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
    
    public override void NPCOutOfSight(Transform other)
    {
    }
    
    public override void ReactToFlare(Transform flare)
    {
        // Seek the flare
        flareBehaviour.TargetFlare = flare;
        AddAction(flareBehaviour);
    }
}