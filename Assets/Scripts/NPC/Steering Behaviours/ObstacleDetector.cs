using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Creates a box collider which detects obstacles in front of the GameObject. The collider acts like a line of sight. This
/// line of sight rotates to face the entity's direction of movement. 
/// </summary>
public class ObstacleDetector : MonoBehaviour 
{

	/** Stores all Transforms inside this trigger volume */
	private List<Transform> visibleObstacles = new List<Transform>();

	/** The maximum distance a character can see an obstacle. */
	public float viewDistance;

	/** The radius of the character. This is the breadth of the character's viewing box */
	public float bodyRadius;
	
	/** Colliders on this layer are considered obstacles. */
	public LayerMask obstacleLayer;

	/** The position of this GameObject the previous frame. Used to determine the entity's direction of movement. The detector
	 *  is then oriented to this direction. */
	private Vector2 previousPosition = Vector2.zero;
	/** The direction in which the GameObject is travelling. The detector points in this direction. */
	private Vector2 movementDirection = Vector2.zero;

	/** The trigger volume that detects if an obstacle is viewable by this GameObject or not. */
	private BoxCollider collider;
    /** The sphere at the tip of the line-of-sight. Allows the LOS to resemble more of a cone. */
    private SphereCollider tipCollider;
	/** Caches this obstacle detector's Transform */
	private new Transform transform;
	
	void Awake()
	{
		// Create colliders to detect which GameObjects enter this neighbourhood
		collider = gameObject.AddComponent<BoxCollider>();
        //tipCollider = gameObject.AddComponent<SphereCollider>();
		// Cache the entity's Transform
		transform = GetComponent<Transform>();

		// Update the dimensions of the collider used for obstacle detection according to the current 'bodyRadius' and 'viewDistance' fields
		UpdateColliderSize ();
		
		// Make the collider a trigger so that it doesn't collide with any GameObjects.
		collider.isTrigger = true;
        //tipCollider.isTrigger = true;
	}
	
	void OnTriggerEnter(Collider collider)
	{
		// If the collider which entered the trigger volume is on the correct layer
		if(((1 << collider.gameObject.layer) & obstacleLayer) == obstacleLayer)
		{
			// Cache the obstacle's Transform component
			Transform obstacle = collider.transform;
			//Debug.LogWarning("Collider entered in " + transform.parent.name + "'s neighbourhood: " + collider.transform.name);
			
			// Add the Transform to the list of visible obstacles, since it is inside this script's trigger collider 
			visibleObstacles.Add(obstacle); 
		}
	}
	
	void OnTriggerExit(Collider collider)
	{
		// If the collider which left the trigger volume belongs to the layer which is tracked by this neighbourhood
		if(((1 << collider.gameObject.layer) & obstacleLayer) == obstacleLayer)
		{
			// Cache the obstacle's Transform component
			Transform obstacle = collider.transform;

			// Remove the GameObject from the list of visible obstacles since it has just left the obstacle detector
			visibleObstacles.Remove(obstacle);
		} 
	}

	/// <summary>
	/// Returns the nearest obstacle in the object's line of sight.
	/// </summary>
	public Transform GetNearestObstacle()
	{
		// Holds the nearest obstacle to this obstacle detector
		Transform nearestObstacle = null;
		float nearestObstacleDistanceSqr = Mathf.Infinity;

		// Cycle through each visible neighbour
		for(int i = 0; i < visibleObstacles.Count; i++)
		{
			Transform obstacle = visibleObstacles[i];

			// Compute the the squared distance between t
			float distanceSqr = (transform.position - obstacle.position).sqrMagnitude;

			// If this obstacle is closer than the previous closest
			if(distanceSqr < nearestObstacleDistanceSqr)
			{
				// Update the nearest obstacle
				nearestObstacle = obstacle;
				nearestObstacleDistanceSqr = distanceSqr;
			}
		}

		return nearestObstacle;
	}
	
	/// <summary>
	/// Update the radius of the detector used to detect obtacles. This represents the width of the character's line of sight.
	/// </summary>
	public void SetBodyRadius(float radius)
	{
		this.bodyRadius = radius;

		// Update the size of the physical collider
		UpdateColliderSize();
	}

	/// <summary>
	/// Update the viewing distance of the detector used to detect obtacles. This represents the length of the character's line of sight.
	/// </summary>
	public void SetViewDistance(float viewDistance)
	{
		this.viewDistance = viewDistance;
		
		// Update the size of the physical collider
		UpdateColliderSize();
	}

	/// <summary>
	/// Updates the size and anchor of the collider used for obstacle detection
	/// </summary>
	private void UpdateColliderSize()
	{
		// Set the collider's width to the detector's viewing distance
		// Set the collider's height to the size of the character's body. This is the breadth of his line of sight
		collider.size = new Vector2(bodyRadius * 2, viewDistance);
		
		// Set the collider's anchor so that its left-center point is at the character's feet (0,0)
		collider.center = new Vector2(0, viewDistance*0.5f);
        
        // Place the tip SphereCollider at the end of the line-of-sight 
        //tipCollider.radius = bodyRadius*3;
        //tipCollider.center = new Vector2(0, viewDistance-bodyRadius*3);
	}

	/// <summary>
	/// The number of obstacles that are within sight of this detector
	/// </summary>
	public int ObstacleCount 
	{
		get { return visibleObstacles.Count; }
	}
	
	/// <summary>
	/// Only GameObjects on this layer are considered obstacles
	/// </summary>
	public LayerMask LayerToDetect
	{
		get { return obstacleLayer; }
		set { obstacleLayer = value; }
	}
}