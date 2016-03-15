using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// AbstractFish class is responsible for behaviour related to fish objects
///
/// @author - Stella L.
/// @author - Jonathan L.A
/// @version - 1.0.0
///
/// </summary>
[RequireComponent(typeof(Steerable))]
public abstract class AbstractFish : LightSource
{
    [Tooltip("Amount of light absorbed from the player upon collision")]
    public float damageInflicted = 4;
    
    [Tooltip("Detects lights within the fish's line of sight")]
    [SerializeField]
    private Neighbourhood lightDetector;

    [Tooltip("If true, the fish will not react to fish of the same type, except in the default flocking behaviour")]
    [SerializeField]
    private bool ignoreFishOfSameType = false;
        
    // Holds the steering behaviors of this fish
    private PriorityDictionary actions;
    protected Steerable steerable;
    
    /// <summary>
    /// Initializes the fish object
    /// </summary>
    protected override void Awake()
    {
        // Call parent LightSource Awake() first
        base.Awake();

        // Initialize action priority dictionary
        this.actions = new PriorityDictionary();
        this.Move();

        // Cache the 'Steerable' component attached to the GameObject performing this action
        this.steerable = transform.GetComponent<Steerable>();
    }

    /// <summary>
    /// Retrieves and executes active actions every frame
    /// </summary>
    protected override void Update()
    {
        base.Update();
        List<NPCActionable> activeActions = actions.GetActiveActions();

        foreach (NPCActionable action in activeActions)
        {
            action.Execute(steerable);
        }
    }

    /// <summary>
    /// Destroys the fish when it loses all its light
    /// </summary>
    protected override void OnLightDepleted()
    {
        base.OnLightDepleted();
        GameObject.Destroy(this.gameObject);
    }

    /// <summary>
    /// Subscribe to the events dictating when lights enter
    /// or exit the fish's line-of-sight.
    /// </summary>
    public override void OnEnable()
    {
        base.OnEnable();
        lightDetector.NeighbourEnter += OnLightEnter;
        lightDetector.NeighbourExit += OnLightExit;
        lightDetector.NeighbourStay += OnLightStay;
    }

    /// <summary>
    /// Unsubscribe to the events dictating when lights enter
    /// or exit the fish's line-of-sight.
    /// </summary>
    public override void OnDisable()
    {
        base.OnDisable();
        lightDetector.NeighbourEnter -= OnLightEnter;
        lightDetector.NeighbourExit -= OnLightExit;
        lightDetector.NeighbourStay -= OnLightStay;
    }

    /// <summary>
    /// How the fish moves when it is not proximate to the player
    /// </summary>
    public abstract void Move();

    /// <summary>
    /// How the fish moves when it is proximate to the player
    /// (called every frame when the player is in sight).
    /// </summary>
    public abstract void ReactToPlayer(Transform player);

    /// <summary>
    /// How the fish moves when it is proximate to the player
    /// (called every frame when a fish is in sight).
    /// </summary>
    public abstract void ReactToNPC(Transform other);

    /// <summary>
    /// Called the instant an NPC becomes out of sight
    /// </summary>
    public abstract void NPCOutOfSight(Transform other);

    /// <summary>
    /// How the fish reacts when a flare is within line of sight
    /// </summary>
    public abstract void ReactToFlare(Transform other);

    /// <summary>
    /// Returns the height of a fish
    /// </summary>
    public virtual float GetHeight()
    {
        return transform.lossyScale.y;
    }

    /// <summary>
    /// Returns the width of a fish
    /// </summary>
    public virtual float GetWidth()
    {
        return transform.lossyScale.x;
    }

    /// <summary>
    /// Returns the fish object's unique ID
    /// </summary>
    public string GetID()
    {
        return this.LightSourceID;
    }

    /// <summary>
    /// Calculates the radius of a sphere around the fish
    /// </summary>
    public float CalculateRadius()
    {
        float height = GetHeight();
        float width = GetWidth();

        if (height > width)
        {
            return height / 2;
        }
        else
        {
            return width / 2;
        }
    }

    /// <summary>
    /// Applies the steerable forces to the object every physics step
    /// </summary>
    protected void FixedUpdate()
    {
        steerable.ApplyForces(Time.fixedDeltaTime);
    }

    /// <summary>
    /// Adds an action to the priority dictionary
    /// </summary>
    protected void AddAction(NPCActionable action)
    {
        action.ActionComplete += OnActionComplete;
        actions.InsertAction(action);
    }

    /// <summary>
    /// Removes an action from the list of actions to perform
    /// using an ID
    /// </summary>
    protected void RemoveAction(string id)
    {
        RemoveAction(actions.GetAction(id));
    }

    /// <summary>
    /// Removes an action from the list of actions to perform
    /// using an action type
    /// </summary>
    protected void RemoveAction(NPCActionable action)
    {
        if (action == null || !action.CanBeCancelled())
        {
            return;
        }
        // Unsubscribe from events before removing the action
        action.ActionComplete -= OnActionComplete;
        actions.RemoveAction(action.id);
    }

    /// <summary>
    /// Removes the completed action from the list of actions to perform.
    /// </summary>
    protected void OnActionComplete(NPCActionable completedAction)
    {
        actions.RemoveAction(completedAction.id);
    }

    /// <summary>
    /// Detects if fish is close to another light
    /// </summary>
    private void OnLightEnter(GameObject lightObject)
    {
        LightSource lightSource = lightObject.GetComponentInParent<LightSource>();
        
        if (lightSource && lightSource.tag.Equals("Fish"))
        {
            if (!ignoreFishOfSameType || lightSource.gameObject.layer != gameObject.layer)
            {
                ReactToNPC(lightSource.transform);
            }
        }
        
        if (lightObject.tag.Equals("Flare"))
        {
            ReactToFlare(lightObject.transform);
        }
    }

    /// <summary>
    /// While another light is in the viewable region of the fish
    /// </summary>
    private void OnLightStay(GameObject lightObject)
    {
        LightSource lightSource = lightObject.GetComponentInParent<LightSource>();
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

    /// <summary>
    /// Detects if fish is no longer close to another light
    /// </summary>
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
                    string otherID = lightSource.GetComponent<AbstractFish>().GetID();
                    RemoveAction(otherID);
                }
            }
            else if (lightSource.CompareTag("Player"))
            {
                // Player id = -1
                RemoveAction("-1");
            }
        }
        if (lightObject.CompareTag("Flare"))
        {
            // Stop reacting to the flare 
            RemoveAction("-2");
        }
    }

}
