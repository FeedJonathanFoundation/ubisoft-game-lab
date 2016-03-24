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

    private bool bite;
    private bool swim;
    private float animationSpeed = 1f;
    private Animator animator;

    protected override void Awake()
    {
        // call parent LightSource Awake() first
        base.Awake();
        
        followPlayer.SetPriority(2);                    // High priority
        followPlayer.SetID("-1");
        followPlayer.Init();
        
        flareBehaviour.SetPriority(3);                  // Very high priority
        flareBehaviour.SetID("-2");
        flareBehaviour.Init();
        
        moveToWaypoint.SetBigFish(this.transform);      //lowest priority
        moveToWaypoint.SetPriority(0);
        moveToWaypoint.SetID("-3");
        
        //animator = GetComponentInParent<Animator>();
        bite = false;
        swim = false;
        Animate();
    }
    
    protected override void Update()
    {
        base.Update();
        BossReactToPlayer();
    }
    
    public void BossReactToPlayer()
    {
        // only for the boss ai, always seeks player if it isn't in a safe zone
        if (!player.GetComponent<Player>().isSafe)
        {
            bite = true;
            swim = true;
            followPlayer.TargetLightSource = player.GetComponent<LightSource>();
            AddAction(followPlayer);
            SetAnimationSpeed();
        }
        
    }
    
    public override void Move() 
    {
        bite = false;
        swim = true;
        Animate();
        moveToWaypoint.SetPriority(0);   // Lowest priority
        moveToWaypoint.SetID(GetID());
        Debug.Log(moveToWaypoint.ToString());
        AddAction(moveToWaypoint);
        SetAnimationSpeed();
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
        bite = true;
        swim = true;
        Animate();
        // Seek the flare
        flareBehaviour.TargetFlare = flare;
        AddAction(flareBehaviour);
        SetAnimationSpeed();
    }
    
    private void Animate()
    {
        if(animator)
        {
            animator.SetBool("Bite", bite);
            animator.SetBool("Swim", swim);
        }
    }
    
    private void SetAnimationSpeed()
    {
        if(animator)
        {
            animationSpeed = GetComponent<Rigidbody>().velocity.magnitude;
            animator.SetFloat("Speed", animationSpeed);
        }
    }
}