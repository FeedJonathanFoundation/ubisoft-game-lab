using UnityEngine;
using System.Collections;

public class WhaleMovement : MonoBehaviour
{

    [Tooltip("The strength of movement force.")]
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float frequency;
    [SerializeField]
    private float magnitude;
    
    [SerializeField]
    private float animationSpeed;
    
    [Tooltip("What the game object moves toward.")]
    [SerializeField]
    private Transform target;

    private Vector3 axis;
    private Vector3 position;

    void Start()
    {
        position = transform.position;
        axis = transform.up;
        Animator animator = GetComponentInParent<Animator>();
        animator.SetFloat("Speed", animationSpeed);
    }

    void Update()
    {
        Move();
    }
    
    void Move()
    {
        position = Vector3.MoveTowards(transform.position, target.position, movementSpeed * Time.deltaTime);
        transform.position = position + axis * Mathf.Sin(Time.time * frequency) * magnitude;
    }
    
    // If in contact with a disable collider, disables the game object
    void OnTriggerEnter(Collider col)
    {
        gameObject.SetActive(false);
    }
}
