using UnityEngine;
using System.Collections;

public class SmoothCamera : MonoBehaviour
{
    /// <summary>
    /// The higher the value, the slower the camera moves.
    /// </summary>
    public float dampTime = 0.15f;
    /// <summary>
    /// The camera will not follow the target if the target this close to the camera's center
    /// </summary>
    [Tooltip("The camera will not follow the target if the target this close to the camera's center")]
    public float deadzoneRadius;
    [Tooltip("The higher this value, the slower the camera follows the target in the deadzone")]
    public float deadzoneDampTime = 0.5f;
    /// <summary>
    /// The object that the camera follows.
    /// </summary>
    public Transform target;

    /// <summary>
    /// The camera's default z-value.
    /// </summary>
    public float zPosition;

    private float deadzoneRadiusSquared;
    private new Transform transform;
    
    private Vector2 velocity = Vector2.zero;

    void Start()
    {
        transform = GetComponent<Transform>();
        
        deadzoneRadiusSquared = deadzoneRadius * deadzoneRadius;
    }
    
    void FixedUpdate()
    {
        if (target)
        {
            float dampTime = this.dampTime;
            Vector3 targetPosition = Vector2.zero;
         
            float distanceFromTarget = ( (Vector2)(target.position - transform.position) ).sqrMagnitude;   
            // Choose a different damping time based on whether or not the target is in the deadzone
            if (distanceFromTarget <= deadzoneRadiusSquared)
            {
                dampTime = deadzoneDampTime;
                targetPosition = target.position;
            }
            // Else, if the target isn't in the deadzone
            else
            {
                // Compute the target position of the camera
                Vector3 distanceFromPlayer = target.position - transform.position;
                targetPosition = target.position - (Vector3)distanceFromPlayer.SetMagnitude(deadzoneRadius);
            }

            // Move the camera to its target smoothly.
            Vector3 newPosition = Vector2.SmoothDamp(transform.position, (Vector2)targetPosition, ref velocity, dampTime);
            // Lock the camera's depth
            newPosition.z = zPosition;
            
            //Debug.Log("Move camera to: " + (Vector2)targetPosition);

            transform.position = newPosition;
        }
    }
}