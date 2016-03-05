using UnityEngine;
using System.Collections;

// Large, impossible to kill NPCs
// Seeks smaller fish by default
public class FishBoss : AbstractFish
{   
    [Tooltip("Reference to the player")]
    public GameObject player;
    [Tooltip("Then action performed when the fish detects the player")]
    [SerializeField]
    private SeekOrFleeLight playerBehaviour;

    [Tooltip("The action performed when flare is within the fish's line of sight")]    
    [SerializeField]
    private SeekFlare flareBehaviour;
    
    [Tooltip("Then action performed when the player is in a safe zone")]
    [SerializeField]
    private MoveClosestWaypoint moveToWaypoint;
    
    public override void Awake()
    {
        // call parent LightSource Awake() first
        base.Awake();
        
        playerBehaviour.SetPriority(2);     // High priority
        playerBehaviour.SetID(-1);
        playerBehaviour.Init();
        
        flareBehaviour.SetPriority(3);      // Very high priority
        flareBehaviour.SetID(-2);
        flareBehaviour.Init();
        
        moveToWaypoint.SetBigFish(this.transform);
        moveToWaypoint.SetPriority(0);
        moveToWaypoint.SetID(-3);
    }
    
    public override void Move() 
    {
        AddAction(moveToWaypoint);
        //use waypoints??? wtf new npc actionnable subclass look at flocking thingy
    }
    
    // Called every frame when the fish can see the player
    public override void ReactToPlayer(Transform player)
    {        
        playerBehaviour.TargetLightSource = player.GetComponent<LightSource>();
        AddAction(playerBehaviour);
    }
    
    public override void ReactToNPC(Transform other)
    {                
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