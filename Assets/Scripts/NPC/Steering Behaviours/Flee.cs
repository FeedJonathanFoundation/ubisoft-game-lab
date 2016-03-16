﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class Flee : NPCActionable
{   
    public bool useTimer;
    [Tooltip("The action stays active 'timer' seconds before being recycled")]
    public float timer;
    
    private Transform targetTransform;
    
    public Flee(int priority, string id, Transform transform) : base(priority, id)
    {
        targetTransform = transform;
    }
    
	public override void Execute(Steerable steerable) 
    {
        base.Execute(steerable);
        
        // If the action has elapsed its timer
        if (useTimer && timeActive > timer)
        {
            //Debug.Log("Flee timer elapsed.");
            
            // Inform subscribers that the action is completed. This stops the action's execution.
            ActionCompleted();
            
            // Reset the timer for the next time the action is performed.
            ResetTimer();
        }
        
        if (targetTransform)
        {            
            // If player's lights are on, seek player
            if (targetTransform.gameObject.CompareTag("Player")) 
            {
                Player player = targetTransform.gameObject.GetComponent<Player>();
                if (player.IsDetectable())
                {
                    // Override the steerable's min/max speed
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
                    
                    //Debug.Log("FLEE THE PLAYER [Flee.Execute()]");
                }
                else
                {
                    // The player is hidden. Thus, the fish should stop fleeing
                    //ActionCompleted();
                }
            }
            else
            {
                // Override the steerable's min/max speed
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
            }
            
            steerable.AddFleeForce(targetTransform.position, strengthMultiplier);
        }
    }
    
    public override bool CanBeCancelled()
    {
        // Return true only if the timer is elapsed or if a timer isn't used.
        return (useTimer && timeActive > timer) || !useTimer;
    }
    
    /// <summary>
    /// The transform to flee
    /// </summary>
    public Transform TargetTransform
    {
        get { return targetTransform; }
        set { targetTransform = value; }
    }
}
