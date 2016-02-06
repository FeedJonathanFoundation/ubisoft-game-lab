using UnityEngine;
using System.Collections;

public enum RotationAxis
{
    X,Y,Z
}

/// <summary>
/// Spins a GameObject around a given axis
/// </summary>
public class Spin : MonoBehaviour
{
    /// <summary>
    /// The speed at which the player spins whilst idle (degrees/sec)
    /// </summary>
    [Tooltip("The speed at which the object spins (degrees/sec)")]
    public float spinSpeed = 50f;
    
    [Tooltip("The axis around which the object spins")]
    public RotationAxis rotationAxis;
    
    /** Caches the GameObject's components */
    private new Transform transform;
    
    void Start()
    {
        transform = GetComponent<Transform>();
    }
    
    void FixedUpdate()
    {
        Vector3 rotationDirection = Vector3.zero;
        
        // Choose the direction around which the object will spin
        switch (rotationAxis)
        {
            case RotationAxis.X:
                rotationDirection = transform.right;
                break;
            case RotationAxis.Y:
                rotationDirection = transform.up;
                break;
            case RotationAxis.Z:
                rotationDirection = transform.forward;
                break;
            default:
                Debug.LogError("Rotation axis not specified (Spin.cs)");
                break;
        }
        
        // Perform the rotation
        transform.RotateAround(transform.position, rotationDirection, spinSpeed*Time.fixedDeltaTime);
    }
}