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
    private bool isDead; // determines is current player is dead
    private new Transform transform;
    private new Rigidbody rigidbody;

    // Use this for initialization
    public override void Awake()
    {
       base.Awake(); // call parent LightSource Awake() first
       transform = GetComponent<Transform>();
       rigidbody = GetComponent<Rigidbody>();
       movement = new PlayerMovement(massEjectionTransform, lightBallPrefab, thrustForce, thrustEnergyCost, transform, rigidbody, this.LightEnergy);
       this.LightEnergy.LightDepleted += OnLightDepleted;
       this.isDead = false;
    }

    void Start()
    {
        Debug.Log(transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            Restart();
        }
        else
        {
            Move();
        }
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

    private void Restart()
    {
        if (Input.GetButtonDown("Restart"))
        {
            Debug.Log("Game Restarted");
            this.LightEnergy.Add(this.defaultEnergy);
            this.isDead = false;
            //Checkpoint.Load();
        }
    }

    /// <summary>
    /// Listens for LightDepleted event from LightEnergy
    /// Sets player to dead when it occurs
    /// </summary>
    public void OnLightDepleted()
    {
        isDead = true;
        Debug.Log("Game OVER! Press 'R' to restart!");
    }

}