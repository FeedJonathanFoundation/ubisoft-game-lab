using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A Neighbourhood is a circle trigger collider which holds an array containing all GameObjects presently
/// inside the trigger volume. Allows entities to keep track of the  
/// </summary>
public class Neighbourhood : MonoBehaviour
{
    /** Stores all GameObjects inside this trigger collider */
    private List<GameObject> neighbours = new List<GameObject>();

    /** The center position of the SphereCollider used to detect neighbours. */ 
    public Vector3 centerOffset;

	/** The radius of the neighbourhood. Only collider's within 'radius' meters of the Neighbourhood are added to the list of neighbours */
	public float radius;

    /** Colliders on this layer will be added to the neighbourhood. */
    public LayerMask layerToDetect;

	/** The trigger volume that detects if a GameObject is inside this neighbourhood or not. */
	private SphereCollider circleCollider;
    
    /** Raised when neighbours enter or exit the neighbourhood. */
    public delegate void NeighbourEnterHandler(GameObject neighbour);
    public delegate void NeighbourExitHandler(GameObject neighbour);
    public delegate void NeighbourStayHandler(GameObject neighbour);
    
    public event NeighbourEnterHandler NeighbourEnter = delegate {};
    public event NeighbourExitHandler NeighbourExit = delegate {};
    public event NeighbourStayHandler NeighbourStay = delegate {};

	void Awake()
	{
		// Create a circle collider to detect which colliders enter this neighbourhood
		circleCollider = gameObject.AddComponent<SphereCollider>();
		// Set the collider's radius to the neighbourhood's radius
		circleCollider.radius = radius;
        circleCollider.center = centerOffset;

		// Make the collider a trigger so that it doesn't collide with any GameObjects.
		circleCollider.isTrigger = true;
	}

	void Update()
	{
		for (int i = 0; i < neighbours.Count; i++)
        {
            // If the neighbour was destroyed, remove it from the list of neighbours
            if (neighbours[i] == null)
            {
                neighbours.RemoveAt(i);
                continue;
            }
                
            // Notify each subscriber that this neighbour is still in the GameObject's line-of-sight
            NeighbourStay(neighbours[i]);
        }
	}

    void OnTriggerEnter(Collider collider)
    {
        // If the collider which entered the trigger volume is on the correct layer
        if(((1 << collider.gameObject.layer) & layerToDetect) == layerToDetect 
            /*&& collider.transform.parent != transform.parent*/)
        {
			//Debug.LogWarning("Collider entered in " + transform.parent.name + "'s neighbourhood: " + collider.transform.name);
            
            GameObject neighbour = collider.gameObject;

            // Add the GameObject to the neighbourhood, since it is inside this script's trigger collider 
            neighbours.Add(neighbour);   
            
            // Inform subscribers that a new neighbour is in sight
            NeighbourEnter(neighbour);
             
        }
    }

    void OnTriggerExit(Collider collider)
    {
        // If the collider which left the trigger volume belongs to the layer which is tracked by this neighbourhood
		if(((1 << collider.gameObject.layer) & layerToDetect) == layerToDetect)
        {
            GameObject neighbour = collider.gameObject;
            
            // Remove the GameObject from this neighbourhood since it has just left the neighbourhood's collider
            neighbours.Remove(neighbour);
            
            // Inform subscribers that a neighbour has left the line-of-sight
            NeighbourExit(neighbour);
        } 
    }

	/// <summary>
	/// Returns the neighbour at the given index of the neighbourhood's internal array of neighbours.
	/// </summary>
	public GameObject GetNeighbour(int index)
	{
		return neighbours[index];
	}

	/// <summary>
	/// Update the neighbourhood's radius. Only colliders within 'radius' meters from the Neighbourhood's 
	/// center is elligible to be added to the neighbourhood's list of neighbours.
	/// </summary>
	public void SetRadius(float radius)
	{
		this.radius = radius;

		// Update the trigger circle's radius to the given value. This changes the neighbourhood's bounding volume in the physics engine.
		circleCollider.radius = radius;
	}

    /// <summary>
    /// The number of GameObjects which are inside this Neighbourhood's trigger collider
    /// </summary>
    public int NeighbourCount 
    {
        get { return neighbours.Count; }
    }
    
    /// <summary>
    /// Only GameObjects on this layer will be added and kept track of in this neighbourhood
    /// </summary>
    public LayerMask LayerToDetect
    {
        get { return layerToDetect; }
        set { layerToDetect = value; }
    }
    
    public string ToString()
    {
        string log = "Neighbours: ";

		for(int i = 0; i < neighbours.Count; i++)
			log += neighbours[i].name + ", ";
            
        return log;
    }
}