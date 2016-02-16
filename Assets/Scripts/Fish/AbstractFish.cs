using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Steerable))]
public abstract class AbstractFish : LightSource
{

    /** The steerable component attached to the GameObject performing this action. */
    protected Steerable steerable;

    // The steering behaviors to apply every frame
    protected PriorityDictionary actions;

    // The condition that must be met for this action to return 'Success'
    protected StoppingCondition stoppingCondition;
    
    static int globalId = 0;
    private int myId;

    public override void Awake()
    {
        base.Awake(); // call parent LightSource Awake() first
        
        // Initialize action priority dictionary
        actions = new PriorityDictionary();
        Move();

        // Cache the 'Steerable' component attached to the GameObject performing this action
		steerable = transform.GetComponent<Steerable>();

		// Set the stopping condition's transform to the same Transform that is performing this 'Steer' action.
		// This way, the stopping condition will be tested using this GameObject's position
		stoppingCondition.SetTransform(transform);

        // Reset the stopping condition. The stopping condition now knows that the 'Steer' action just started.
		stoppingCondition.Init();
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
        
        Dictionary<int, NPCActionable> activeActions = actions.GetActiveDictionary(); 
        foreach(KeyValuePair<int, NPCActionable> entry in activeActions)
        {
            entry.Value.Execute(steerable);
        }
    }

    void FixedUpdate()
    {
        steerable.ApplyForces (Time.fixedDeltaTime); 
    }
    
    // Returns the fish's unique ID
    public int GetID()
    {
        return myId;
    }

    // Detects if fish is close to another character
    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            ReactToPlayer(other.gameObject.transform);
        }
        else if (other.gameObject.CompareTag("Fish")) 
        {
            ReactToNPC(other.gameObject.transform);
        }
    }
    
    // Detects if fish is no longer close to another character
    void OnTriggerExit(Collider other) 
    {
        if (other.gameObject.CompareTag("Fish")) 
        {
            int otherID = other.GetComponent<AbstractFish>().GetID();
            actions.RemoveAction(otherID);
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
