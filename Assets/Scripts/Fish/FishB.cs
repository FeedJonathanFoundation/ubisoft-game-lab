using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Medium, hard to kill NPCs
// Seeks smaller fish by default
// Run away from large fish upon close contact
public class FishB : AbstractFish
{
    [SerializeField]
    private Flocking flockingBehaviour;
    
    [Tooltip("When another fish is in sight, this fish will either seek or flee it")]
    [SerializeField]
    private SeekOrFleeLight otherFishBehaviour;
    
    [Tooltip("Then action performed when the fish detects the player")]
    [SerializeField]
    private SeekOrFleeLight playerBehaviour;

    [Tooltip("The action performed when flare is within the fish's line of sight")]    
    [SerializeField]
    private SeekFlare flareBehaviour;
    
    public override void Awake()
    {
        // call parent LightSource Awake() first
        base.Awake(); 
        
        flockingBehaviour.SetPriority(0);   // Lowest priority
        flockingBehaviour.SetID(GetID());
        
        otherFishBehaviour.SetPriority(1);  // Medium priority
        otherFishBehaviour.Init();
        
        playerBehaviour.SetPriority(2);     // High priority
        playerBehaviour.SetID(-1);
        playerBehaviour.Init();
        
        flareBehaviour.SetPriority(3);      // Very high priority
        flareBehaviour.SetID(-2);
        flareBehaviour.Init();
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
        LightSource currentFishTarget = otherFishBehaviour.TargetLightSource;
        
        if (currentFishTarget == null)
        {
            // Debug.Log("React to new fish: " + other.name);
            AbstractFish fish = other.gameObject.GetComponent<AbstractFish>();
            int id = fish.GetID();
            
            otherFishBehaviour.TargetLightSource = fish;
            otherFishBehaviour.SetID(id);
            AddAction(otherFishBehaviour);
        }
    }
    
    public override void NPCOutOfSight(Transform other)
    {
        if (otherFishBehaviour.TargetLightSource != null 
            && otherFishBehaviour.TargetLightSource.transform == other)
        {
            otherFishBehaviour.TargetLightSource = null;
        }
    }
    
    public override void ReactToFlare(Transform flare)
    {
        // Seek the flare
        flareBehaviour.TargetFlare = flare;
        AddAction(flareBehaviour);
    }
}