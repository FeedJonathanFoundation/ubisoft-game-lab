using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Steerable))]
public abstract class AbstractFish : LightSource
{
    /** The steerable component attached to the GameObject performing this action. */
    protected Steerable steerable;
    
    [Tooltip("Detects lights within the fish's line of sight")]
    [SerializeField]
    private Neighbourhood lightDetector;

    // The steering behaviors to apply every frame
    private PriorityDictionary actions;

    // The condition that must be met for this action to return 'Success'
    protected StoppingCondition stoppingCondition = new StoppingCondition();
    
    [Tooltip("If true, the fish will not react to fish of the same type, except in the default flocking behaviour")]
    public bool ignoreFishOfSameType = false;
    
    [Tooltip("If true, the fish prints every action it performs every frame in the console")]
    public bool debugLogActions = false;
    
    static int globalId = 1;
    private int myId;

    public override void Awake()
    {
        base.Awake(); // call parent LightSource Awake() first
        
        // Initialize action priority dictionary
        actions = new PriorityDictionary();
        Move();
        
        // Assign the next available ID
        myId = globalId++;

        // Cache the 'Steerable' component attached to the GameObject performing this action
		steerable = transform.GetComponent<Steerable>();

		// Set the stopping condition's transform to the same Transform that is performing this 'Steer' action.
		// This way, the stopping condition will be tested using this GameObject's position
		stoppingCondition.SetTransform(transform);

        // Reset the stopping condition. The stopping condition now knows that the 'Steer' action just started.
		stoppingCondition.Init();
    }
    
    public override void OnEnable()
    {
        base.OnEnable();
        
        // Subscribe to the events dictating when lights enter/exit the fish's line-of-sight 
        lightDetector.NeighbourEnter += OnLightEnter;
        lightDetector.NeighbourExit += OnLightExit;
        lightDetector.NeighbourStay += OnLightStay;
    }
    
    public override void OnDisable()
    {
        base.OnDisable();
        
        // Unsubscribe to the events dictating when lights enter/exit the fish's line-of-sight 
        lightDetector.NeighbourEnter -= OnLightEnter;
        lightDetector.NeighbourExit -= OnLightExit;
        lightDetector.NeighbourStay -= OnLightStay; 
    }

    public override void Update()
    {     
        base.Update();
           
        if(stoppingCondition.Complete())
		{
			//return;
		}
        
        List<NPCActionable> activeActions = actions.GetActiveActions(); 
        foreach(NPCActionable action in activeActions)
        {
            //Debug.Log("Before performing action : \n" + actions.ToString());
            
            if (debugLogActions) { Debug.Log("Execute action : " + action.ToString()); }
            
            action.Execute(steerable);
            //Debug.Log("After performing action : \n" + actions.ToString());
        }
        
        if (debugLogActions) { Debug.Log(actions.ToString()); }
        
        // NPCActionable playerSeek = actions.GetAction(-1);
        // if(playerSeek != null)
        //     Debug.Log("Flee player: " + gameObject.name);
            
        //Debug.Log("After foreach loop : \n" + actions.ToString());
            
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

    // Detects if fish is close to another light
    private void OnLightEnter(GameObject lightObject) 
    {
        LightSource lightSource = lightObject.GetComponentInParent<LightSource>();
        
        // if (lightSource.name == "Fish B") 
        // {
        //     Debug.Log("Collided with : " + lightSource.tag);
        // }
        
        if (lightSource)
        {
            if (lightSource.tag.Equals("Player")) 
            {
                // ReactToPlayer(lightSource.transform);
                
                //Debug.Log(actions.ToString());
            }
            else if (lightSource.tag.Equals("Fish")) 
            {
                if (!ignoreFishOfSameType || lightSource.gameObject.layer != gameObject.layer) 
                { 
                    ReactToNPC(lightSource.transform); 
                }
            }
        }
        
        if (lightObject.tag.Equals("Flare"))
        {
            ReactToFlare(lightObject.transform);
        }
    }
    
    private void OnLightStay(GameObject lightObject)
    {
        LightSource lightSource = lightObject.GetComponentInParent<LightSource>();
        
        //Debug.Log(lightObject.name + " is visible by " + gameObject.name);

        if (lightSource)
        {
            if (lightSource.tag.Equals("Player")) 
            {
                ReactToPlayer(lightSource.transform);
            }
            else if (lightSource.tag.Equals("Fish"))
            {
                if (!ignoreFishOfSameType || lightSource.gameObject.layer != gameObject.layer) 
                { 
                    ReactToNPC(lightSource.transform); 
                }
            }
        }
    }
    
    // Detects if fish is no longer close to another light
    private void OnLightExit(GameObject lightObject) 
    {
        LightSource lightSource = lightObject.GetComponentInParent<LightSource>();
        
        if (lightSource)
        {
            if (lightSource.CompareTag("Fish")) 
            {
                if (!ignoreFishOfSameType || lightSource.gameObject.layer != gameObject.layer)
                {
                    // Inform subclasses that the NPC went out of sight
                    NPCOutOfSight(lightSource.transform);
                    
                    int otherID = lightSource.GetComponent<AbstractFish>().GetID();
                    RemoveAction(otherID);
                }
            }
            else if (lightSource.CompareTag("Player"))
            {
                //Debug.Log("Before RemoveAction()\n" + actions.ToString());
                //Debug.Log("Player out of sight of fish : " + lightSource.name);
                // Player id = -1
                RemoveAction(-1);
                
                //Debug.Log("After RemoveAction()\n" + actions.ToString());
            }
        }
        
        if (lightObject.CompareTag("Flare"))
        {
            // Stop reacting to the flare 
            RemoveAction(-2);
        }
    }
    
    // Called the instant the fish loses all its light.
    protected override void OnLightDepleted()
    {
        base.OnLightDepleted();
        
        GameObject.Destroy(this.gameObject);
    }
    
    protected void AddAction(NPCActionable action)
    {
        action.ActionComplete += OnActionComplete;
        actions.InsertAction(action);
    }
    
    protected void RemoveAction(int id)
    {
        //Debug.Log("Remove the action with ID : " + id);
        RemoveAction(actions.GetAction(id));
    }
    
    protected void RemoveAction(NPCActionable action)
    {
        if (action == null || !action.CanBeCancelled())
            return;        
        
        Debug.Log("Remove the action : " + action.ToString());
        
        // Unsubscribe from events before removing the action
        action.ActionComplete -= OnActionComplete;
        actions.RemoveAction(action.id);
        
        Debug.Log("Priority dictionary after removing action:\n" + actions.ToString());
    }

    // Removes the completed action from the list of actions to perform.
    protected void OnActionComplete(NPCActionable completedAction)
    {
        actions.RemoveAction(completedAction.id);
    }

    // How the fish moves when it is not proximate to the player
    public abstract void Move();

    // How the fish moves when it is proximate to the player (called every frame when the player is in sight)
    public abstract void ReactToPlayer(Transform player);

    // How the fish moves when it is proximate to the player (called every frame when a fish is in sight)
    public abstract void ReactToNPC(Transform other);
    // Called the instant an NPC becomes out of sight
    public abstract void NPCOutOfSight(Transform other);
    
    // How the fish reacts when a flare is within line of sight
    public abstract void ReactToFlare(Transform other);

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
