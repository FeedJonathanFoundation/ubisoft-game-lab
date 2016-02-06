using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Player : LightSource
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
    [Tooltip("The amount of force applied on the player when ejecting one piece of mass.")]
    public float thrustForce;

    /// <summary>
    /// The amount of light energy spent when ejecting one piece of mass.
    /// </summary>
    [Tooltip("The amount of light energy spent when ejecting one piece of mass.")]
    public float thrustEnergyCost = 1;

    /** Caches the player's components */
    private PlayerMovement movement;
    private new Transform transform;
    private new Rigidbody rigidbody;

    // Use this for initialization
    public override void Awake()
    {
       base.Awake(); // call parent LightSource Awake() first
       transform = GetComponent<Transform>();
       rigidbody = GetComponent<Rigidbody>();
       movement = new PlayerMovement(massEjectionTransform, lightBallPrefab, thrustForce, thrustEnergyCost, transform, rigidbody, this.LightEnergy);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void SelfDestroy()
    {
        GameObject.Destroy(gameObject);
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

        // Ensure that the rigidbody never spins
        rigidbody.angularVelocity = Vector3.zero;
    }

}