using UnityEngine;
using System.Collections;

[System.Serializable]
public class BossSeekPlayer : NPCActionable
{   
    /// <summary>
	/// The light that this NPC can see
	/// </summary>
    private LightSource targetLightSource;

    /// <summary>
    /// The seek action performed when the NPC has more light than the target light source.
    /// </summary>
    [SerializeField]
    private Seek seekPlayer;
    
    [SerializeField]
    private WallAvoidance wallAvoidance;
    
    public BossSeekPlayer(int priority, int id, LightSource targetLightSource) : base(priority, id)
    {
        this.targetLightSource = targetLightSource;
        
        SetPriority(priority);
        SetID(id);
    }
    
    /** Call this method in the Start() function of the fish performing this action. */
    public void Init()
    {
        // Call ChildActionComplete() when either the seek or flee actions are completed.
        seekPlayer.ActionComplete += ChildActionComplete;
        wallAvoidance.ActionComplete += ChildActionComplete;
    }
    
    public void SetPriority(int priority)
    {
        this.priority = priority;
        
        seekPlayer.priority = priority;
        wallAvoidance.priority = priority;
    }
    
    public void SetID(int id)
    {
        this.id = id;
        
        seekPlayer.id = id;
        wallAvoidance.id = id;
    }
    
	public override void Execute(Steerable steerable) 
    {
        base.Execute(steerable);
        
        if (overrideSteerableSpeed)
        {
            steerable.MinSpeed = minSpeed;
            steerable.MaxSpeed = maxSpeed;
        }
        // Override the steerable's max force
        if (overrideMaxForce)
        {
            steerable.MaxForce = maxForce;
        }
        
        Player player = targetLightSource.gameObject.GetComponent<Player>();
        
        if (!player.isSafe)
        {
            steerable.AddSeekForce(targetLightSource.transform.position, strengthMultiplier);
        }
        else
        {
            ActionCompleted();
        }
        
        wallAvoidance.Execute(steerable);
    }
    
    private void ChildActionComplete(NPCActionable childAction)
    {
        // Call ActionCompleted() to notify subscribers that the parent action is complete
        ActionCompleted();
    }
    
    /// <summary>
	/// The light that this NPC will seek or flee
	/// </summary>
    public LightSource TargetLightSource
    {
        get { return targetLightSource; }
        set 
        { 
            targetLightSource = value; 
            
            if (value)
            {
                seekPlayer.TargetTransform = value.transform;
            }
            else
            {
                seekPlayer.TargetTransform = null;
            }
        }
    }
    
}
