using UnityEngine;
using System.Collections;

// Large, impossible to kill NPCs
// Seeks smaller fish by default
public class FishBoss : AbstractFish
{
    private GameObject player;
    [Tooltip("Then action performed when the fish detects the player")]
    [SerializeField]
    private BossSeekPlayer followPlayer;

    [Tooltip("The action performed when flare is within the fish's line of sight")]    
    [SerializeField]
    private SeekFlare flareBehaviour;
    
    [Tooltip("Then action performed when the player is in a safe zone")]
    [SerializeField]
    private MoveClosestWaypoint moveToWaypoint;

    
    private float animationSpeed = 1f;
    private Animator animator;
    private float speedFactor = 5f;

    protected override void Awake()
    {
        // call parent LightSource Awake() first
        base.Awake();
        
        player = GameObject.Find("Player");
        
        followPlayer.SetPriority(2);                    // High priority
        followPlayer.SetID("-1");
        followPlayer.Init();
        
        flareBehaviour.SetPriority(3);                  // Very high priority
        flareBehaviour.SetID("-2");
        flareBehaviour.Init();
        
        moveToWaypoint.SetBigFish(this.transform);      //lowest priority
        moveToWaypoint.SetPriority(0);
        moveToWaypoint.SetID("-3");
        
        animator = GetComponentInChildren<Animator>();
        
        SetAnimationSpeed();
        Animate(false, true);
    }
    
    protected override void Update()
    {
        base.Update();
        BossReactToPlayer();
    }
    
    public void BossReactToPlayer()
    {
        // only for the boss ai, always seeks player if it isn't in a safe zone
        if(!player)
        {
            player = GameObject.Find("Player");
        }
        
        if (!player.GetComponent<Player>().isSafe)
        {
            Animate(true, true);
            followPlayer.TargetLightSource = player.GetComponent<LightSource>();
            AddAction(followPlayer);
            SetAnimationSpeed();
        }
        
    }
    
    public override void Move() 
    {
        Animate(false, true);
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
        Animate(true, true);
        // Seek the flare
        flareBehaviour.TargetFlare = flare;
        AddAction(flareBehaviour);
        SetAnimationSpeed();
    }
    
    private void Animate(bool bite, bool swim)
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
            float currentSpeed = GetComponent<Rigidbody>().velocity.magnitude;
            animationSpeed = currentSpeed / speedFactor;
            if (animationSpeed > 0)
            {
                animator.SetFloat("Speed", animationSpeed);
            }
            else
            {
                animator.SetFloat("Speed", 0.2f);
            }
        }
    }
}