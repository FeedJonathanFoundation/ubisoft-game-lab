using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour
{

    /// <summary>
    /// The position at which the character ejects mass.
    /// </summary>
    public Transform massEjectionTransform;

    /// <summary>
    /// The light ball ejected by the player when thrusting
    /// </summary>
    public GameObject lightBallPrefab;

    /// <summary>
    /// The amount of force applied on the player when ejecting one piece of mass.
    /// </summary>
    public float thrustForce;

    // Private vars
    private PlayerMovement movement;
    private Transform transform;
    private Rigidbody rigidbody;

    // Use this for initialization
    void Awake()
    {
       transform = GetComponent<Transform>();
       rigidbody = GetComponent<Rigidbody>();
       movement = new PlayerMovement(massEjectionTransform, lightBallPrefab, thrustForce, transform, rigidbody);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    /// <summary>
    /// Player movements responding to user input
    /// </summary>
    private void Move()
    {
        if (Input.GetButtonDown("Thrust"))
        {
            // Eject mass in the direction of the left stick
            movement.EjectMass(massEjectionTransform.up);
        }

        // Makes the character follow the left stick's rotation
        movement.FollowLeftStickRotation();
    }

}