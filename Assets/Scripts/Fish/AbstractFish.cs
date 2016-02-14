﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Steerable))]
public abstract class AbstractFish : MonoBehaviour 
{
    
    /** The steerable component attached to the GameObject performing this action. */
	protected Steerable steerable;

	// The steering behaviors to apply every frame
    public Dictionary<int, NPCActionable> actionDirectory;

	// The condwition that must be met for this action to return 'Success'
	public StoppingCondition stoppingCondition;
    
    // public int movementSpeed;
    // public int reactionSpeed;
    
    private bool isProximateToPlayer;
    private bool isProximateToNPC;
    Transform player;                           // Reference to player's position
    Transform other;
    Transform target;
    private int activePriority = 0;
    // public AbstractFish() { }
    // public AbstractFish(NPCAction reactionToPlayer) { }

    static int globalId = 0;
    private int myId;
    public int GetID() { return myId; }

    void Awake() 
    {
        myId = globalId++;
        
        isProximateToPlayer = false;
        isProximateToNPC = false;
        
        actionDirectory = new Dictionary<int, NPCActionable>();

        // Cache the 'Steerable' component attached to the GameObject performing this action
		steerable = transform.GetComponent<Steerable>();

		// Set the stopping condition's transform to the same Transform that is performing this 'Steer' action.
		// This way, the stopping condition will be tested using this GameObject's position
		stoppingCondition.SetTransform(transform); 
        
        // Reset the stopping condition. The stopping condition now knows that the 'Steer' action just started.
		stoppingCondition.Init();
    }
    
    public void PushAction(int id, NPCActionable action)
    {
        if (action.priority > activePriority)
        {
            activePriority = action.priority;
        }
        actionDirectory.Add(id, action);
    }
    
    public void RemoveAction(int id)
    {
        
        //todo go to a sensible activePriority.
    }

    void Update()
    {        
        if(stoppingCondition.Complete())
		{
            // if player health <=0
            // if own health <=0
            if (IsDead()) { this.gameObject.SetActive(false); }
			return;
		}
        
        if (isProximateToPlayer) { ReactToPlayer(player); }
        else if (isProximateToNPC) { ReactToNPC(other); }
        else { Move(); }
        
        foreach(KeyValuePair<int, NPCActionable> entry in actionDirectory)
        {
            NPCActionable actionable = entry.Value;
            if(actionable.priority == activePriority)
            {
                actionable.Execute(steerable);
            }    
        }
        actionDirectory.Clear();
    }

    void FixedUpdate() 
    {
        
        
        steerable.ApplyForces (Time.fixedDeltaTime); 
        // if (player light == on) { Approach(player); }
    }
    
    // Detects if fish is close to the player
    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            isProximateToPlayer = true;
            player = other.gameObject.transform;
        }
        else if (other.gameObject.CompareTag("Fish")) 
        {
            isProximateToNPC = true;
            this.other = other.gameObject.transform;
        }
    }
    
    // Detects if fish is no longer close to the player
    void OnTriggerExit(Collider other) 
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            isProximateToPlayer = false;
        }
        else if (other.gameObject.CompareTag("Fish")) 
        {
            isProximateToNPC = false;
        }
    }
    
    public bool IsDead()
    {
        // if (enemyHealth >= 0) { return true; }
        // else { return false; }
        return false;
    }

    // How the fish moves when it is not proximate to the player
    public abstract void Move();
    
    // How the fish moves when it is proximate to the player
    public abstract void ReactToPlayer(Transform player);
    
    // How the fish moves when it is proximate to the player
    public abstract void ReactToNPC(Transform other);
    
    // Returns the height of a fish
    public virtual float GetHeight()
    {
        return transform.lossyScale.y;
    }
    
    // Returns the width of a fish
    public virtual float GetWidth()
    {
        return transform.lossyScale.x;
    }
    
    // Calculates the radius of a sphere around the fish
    public float CalculateRadius()
    {
        float height = GetHeight();
        float width = GetWidth();
        
        if (height > width) { return height / 2; }
        else { return width / 2; }
    }

}
