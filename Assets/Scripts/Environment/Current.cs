using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Direction
{
    down,
    up,
    left,
    right
}

// Acts as an invisible boundary
// Be mindful of the direction the player will hit the boundary
// This should be the side of the trigger box collider
public class Current : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The strength of current force.")]
    private float strength;
    [SerializeField]
    [Tooltip("The direction of current force.")]
    private Direction currentDirection;
    private Vector3 direction;
    

    // Whether the current object is empty.
    private bool empty;
    // Holds all rigidbodies in the current.
    private List<Rigidbody> rigidbodies;
    
    void Start()
    {
        rigidbodies = new List<Rigidbody>();
        // By default, the current pushes downward.
        SetDirection();
        empty = true;
    }
    
    void Update()
    {
        if (!empty)
        {
            AddCurrentForce();
        }
    }
    
    void SetDirection()
    {
        switch(currentDirection)
        {
            case Direction.down:
                direction = new Vector3(0f, -1f, 0f);
                break;
            case Direction.up:
                direction = new Vector3(0f, 1f, 0f);
                break;
            case Direction.left:
                direction = new Vector3(-1f, 0f, 0f);
                break;
            case Direction.right:
                direction = new Vector3(1f, 0f, 0f);
                break;    
        }
    }
    
    // Get rigidbody of the colliding game object and add to the list
    void OnTriggerEnter(Collider col) 
    {
        Rigidbody rigidbody = col.gameObject.GetComponent<Rigidbody>();
        if (rigidbody != null) 
        {
            rigidbodies.Add(rigidbody);
            empty = false;
        }
    }
    
    // Remove rigidbody of game object that has exited the current
    void OnTriggerExit(Collider col)
    {
        Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
        if (rb != null && rigidbodies.Contains(rb)) 
        {
            rigidbodies.Remove(rb);
            if (rigidbodies.Count == 0)
            {
                empty = true;
            } 
        }
    }
    
    void AddCurrentForce()
    {
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            if (rigidbody != null)
            {
                Vector3 initialVelocity = rigidbody.velocity;
                rigidbody.AddForce(-initialVelocity);
                rigidbody.AddForce(strength * direction);
            }
        }
    }
}
