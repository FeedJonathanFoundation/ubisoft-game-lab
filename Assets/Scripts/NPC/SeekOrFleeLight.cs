using UnityEngine;
using System.Collections;

[System.Serializable]
public class SeekOrFleeLight : NPCActionable
{
    [Tooltip("If true, this NPC will always flee the light source.")]   
    public bool alwaysFlee = false;
    [Tooltip("If true, this NPC will always seek the light source.")]   
    public bool alwaysSeek = false;
    
    /// <summary>
	/// The light that this NPC can see
	/// </summary>
    private LightSource targetLightSource;

    /// <summary>
    /// The seek action performed when the NPC has more light than the target light source.
    /// </summary>
    [SerializeField]
    private Seek seekWhenStronger;
    /// <summary>
    /// The flee action performed when the NPC has less light than the target light source.
    /// </summary>
    [SerializeField]
    private Flee fleeWhenWeaker;
    
    [SerializeField]
    private WallAvoidance wallAvoidance;
    
    public SeekOrFleeLight(int priority, int id, LightSource targetLightSource) : base(priority, id)
    {
        this.targetLightSource = targetLightSource;
        
        SetPriority(priority);
        SetID(id);
    }
    
    /** Call this method in the Start() function of the fish performing this action. */
    public void Init()
    {
        // Call ChildActionComplete() when either the seek or flee actions are completed.
        seekWhenStronger.ActionComplete += ChildActionComplete;
        fleeWhenWeaker.ActionComplete += ChildActionComplete;
        wallAvoidance.ActionComplete += ChildActionComplete;
    }
    
    public void SetPriority(int priority)
    {
        this.priority = priority;
        
        seekWhenStronger.priority = priority;
        fleeWhenWeaker.priority = priority;
        wallAvoidance.priority = priority;
    }
    
    public void SetID(int id)
    {
        this.id = id;
        
        seekWhenStronger.id = id;
        fleeWhenWeaker.id = id;
        wallAvoidance.id = id;
    }
    
	public override void Execute(Steerable steerable) 
    {
        base.Execute(steerable);
        
        // Retrieve the amount of light energy possessed by the NPC performing this action
        float myLightEnergy = steerable.GetComponent<LightSource>().LightEnergy.CurrentEnergy;
        
        if (targetLightSource)
        {
            // If this fish has less light than its target
            if(!alwaysSeek && (myLightEnergy < targetLightSource.LightEnergy.CurrentEnergy || alwaysFlee))
            {
                // Flee the light source since it is stronger than this fish
                fleeWhenWeaker.Execute(steerable);
                
                //Debug.Log("FLEE THE FISH: " + targetLightSource.name);
            }
            // Else, if this fish has more light than its target
            else
            {
                // Seek the light source
                seekWhenStronger.Execute(steerable);
            }
        }
        else 
        {
            // If the light source has been destroyed, stop performing this action.
            ActionCompleted();
        }
        
        wallAvoidance.Execute(steerable);
    }
    
    public override bool CanBeCancelled()
    {
        return seekWhenStronger.CanBeCancelled() && fleeWhenWeaker.CanBeCancelled();
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
                seekWhenStronger.TargetTransform = value.transform;
                fleeWhenWeaker.TargetTransform = value.transform;
            }
            else
            {
                seekWhenStronger.TargetTransform = null;
                fleeWhenWeaker.TargetTransform = null;
            }
        }
    }
    
}
