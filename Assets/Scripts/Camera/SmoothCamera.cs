using UnityEngine;
using System.Collections;

public class SmoothCamera : MonoBehaviour
{
    /// <summary>
    /// The higher the value, the slower the camera moves.
    /// </summary>
    public float dampTime = 0.15f;
    /// <summary>
    /// The object that the camera follows.
    /// </summary>
    public Transform target;
    
    /// <summary>
    /// The camera's default z-value.
    /// </summary>
    public float zPosition;
     
    private Vector2 velocity = Vector2.zero;
    
    void FixedUpdate()
    {
        if (target)
        {
            // Move the camera to its target smoothly.
            Vector3 newPosition = Vector2.SmoothDamp(transform.position, target.position, ref velocity, dampTime);
            // Lock the camera's depth
            newPosition.z = zPosition;
            
            transform.position = newPosition;
        }
    }
}