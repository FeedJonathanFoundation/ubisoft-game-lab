using UnityEngine;
using System.Collections;

public class PlayerSpin : MonoBehaviour
{
    /// <summary>
    /// The speed at which the player spins whilst idle (degrees/sec)
    /// </summary>
    [Tooltip("The speed at which the player spins whilst idle (degrees/sec)")]
    public float idleSpinSpeed = 50f;
    
    /** Caches the player's components */
    private new Transform transform;
    private new Rigidbody rigidbody;
    
    /// <summary>
    /// Determines the way in which the player is spinning
    /// </summary>
    private enum SpinState
    {
        Idle
    }
    
    void Start()
    {
        transform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        transform.RotateAround(transform.position, transform.up, idleSpinSpeed*Time.fixedDeltaTime);
    }
}