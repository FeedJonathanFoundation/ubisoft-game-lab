using UnityEngine;
using System.Collections;

/// <summary>
/// WhaleMovement class is responsible for controlling whale behaviour.
///
/// @author - Stella L.
/// @version - 1.0.0
///
/// </summary>
public class WhaleMovement : MonoBehaviour
{

    [Tooltip("The strength of movement force.")]
    [SerializeField]
    private float movementSpeed;
    [Tooltip("The frequency of the whale's sine movement.")]
    [SerializeField]
    private float frequency;
    [Tooltip("The amplitude of the whale's sine movement.")]
    [SerializeField]
    private float magnitude;
    [Tooltip("The speed of the whale animation.")]
    [SerializeField]
    private float animationSpeed;
    [Tooltip("What the game object moves toward.")]
    [SerializeField]
    private Transform target;
    // Used to calculate sine movement
    private Vector3 axis;
    // Used to set the whale's next position
    private Vector3 position;

    /// <summary>
    /// Initializes the whale movement.
    /// </summary>
    protected void Start()
    {
        position = transform.position;
        axis = transform.up;
        Animator animator = GetComponentInParent<Animator>();
        animator.SetFloat("Speed", animationSpeed);
    }
    
    /// <summary>
    /// Applies movement changes every frame.
    /// </summary>
    protected void Update()
    {
        Move();
    }
    
    /// <summary>
    /// Calculates the next position of the whale
    /// based on the target and sine function
    /// </summary>
    private void Move()
    {
        if (target == null)
        { 
            Debug.Log("Please set a target for the whale movement.");
            return;
        }
        position = Vector3.MoveTowards(transform.position, target.position, movementSpeed * Time.deltaTime);
        transform.position = position + axis * Mathf.Sin(Time.time * frequency) * magnitude;
    }
    
    /// <summary>
    /// If in contact with a disable collider, disables the game object.
    /// </summary>
    protected void OnTriggerEnter(Collider col)
    {
        gameObject.SetActive(false);
    }
}
