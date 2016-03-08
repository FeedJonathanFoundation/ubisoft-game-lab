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
    [Tooltip("The strength of current force.")]
    public float strength;
    // The direction of the current force.
    [Tooltip("The direction of current force.")]
    public Direction currentDirection;
    private Vector3 direction;
    

    // Whether the current object is empty.
    private bool empty;
    // Holds all rigidbodies in the current.
    private List<Rigidbody> rbs;
    
    void Start()
    {
        rbs = new List<Rigidbody>();
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
        Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
        if (rb != null) 
        {
            rbs.Add(rb);
            empty = false;
        } 
    }
    
    // Remove rigidbody of game object that has exited the current
    void OnTriggerExit(Collider col)
    {
        Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
        if (rb != null && rbs.Contains(rb)) 
        {
            rbs.Remove(rb);
            if (rbs.Count == 0)
            {
                empty = true;
            } 
        }
    }
    
    void AddCurrentForce()
    {
        foreach (Rigidbody rb in rbs)
        {
            if (rb != null) 
            {
                rb.AddForce(direction * strength);
            }
        }
    }
    
}
