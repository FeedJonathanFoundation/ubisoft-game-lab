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
    private BossSeekPlayer followPlayer;

    [Tooltip("The action performed when flare is within the fish's line of sight")]    
    [SerializeField]
    private SeekFlare flareBehaviour;
    
    [Tooltip("Then action performed when the player is in a safe zone")]
    [SerializeField]
    private MoveClosestWaypoint moveToWaypoint;
    public bool atSafeZone;
    
    public override void Awake()
    {
        // call parent LightSource Awake() first
        base.Awake();
        
        followPlayer.SetPriority(2);                    // High priority
        followPlayer.SetID(-1);
        followPlayer.Init();
        
        flareBehaviour.SetPriority(3);                  // Very high priority
        flareBehaviour.SetID(-2);
        flareBehaviour.Init();
        
        moveToWaypoint.SetBigFish(this.transform);      //lowest priority
        moveToWaypoint.SetPriority(0);
        moveToWaypoint.SetID(-3);
        
        atSafeZone = false;
    }
    
    public override void Update()
    {
        base.Update();
        BossReactToPlayer();
    }
    
    public void BossReactToPlayer()
    {
        //only for the boss ai, always seeks player if it isn't in a safe zone
        if (!player.GetComponent<Player>().isSafe)
        {
            followPlayer.TargetLightSource = player.GetComponent<LightSource>();
            AddAction(followPlayer);
        }
    }
    
    public override void Move() 
    {
        AddAction(moveToWaypoint);
    }
    
    public override void ReactToPlayer(Transform player)
    {
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