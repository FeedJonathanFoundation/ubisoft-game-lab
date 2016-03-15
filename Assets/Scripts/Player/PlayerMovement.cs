using UnityEngine;
using System.Collections;

public class PlayerMovement
{
    /// <summary>
    /// The position at which the character ejects mass.
    /// </summary>
    private Transform massEjectionTransform;

    /// <summary>
    /// The light ball ejected by the player when thrusting
    /// </summary>
    private GameObject lightBallPrefab;

    /// <summary>
    /// The amount of force applied on the player when ejecting one piece of mass.
    /// </summary>
    private float thrustForce;

    /// <summary>
    /// The higher the value, the faster the propulsion when changing directions
    /// </summary>
    private float changeDirectionBoost;
    
    /// <summary>
    /// The amount of light energy spent when ejecting one piece of mass.
    /// </summary>
    private float thrustEnergyCost = 1;
    
    /// <summary>
    /// The damping to apply when the brakes are on at full strength
    /// </summary>
    private float brakeDrag = 1;
    
    /// <summary>
    /// The propulsion effect activated when the player is propulsing
    /// </summary>
    private GameObject jetFuelEffect;
    
    private float defaultDrag;

    /** Caches the player's components */
    private Transform transform;
    private Rigidbody rigidbody;
    private LightEnergy lightEnergy;

    /// <summary>
    /// Public constructor
    /// </summary>
    public PlayerMovement(Transform massEjectionTransform, GameObject lightBallPrefab, float thrustForce, float changeDirectionBoost, float thrustEnergyCost, float brakeDrag, Transform transform, Rigidbody rigidbody, LightEnergy lightEnergy, GameObject jetFuelEffect)
    {
        this.massEjectionTransform = massEjectionTransform;
        this.lightBallPrefab = lightBallPrefab;
        this.thrustForce = thrustForce;
        this.changeDirectionBoost = changeDirectionBoost;
        this.thrustEnergyCost = thrustEnergyCost;
        this.brakeDrag = brakeDrag;
        this.jetFuelEffect = jetFuelEffect;
        this.defaultDrag = rigidbody.drag;

        this.transform = transform;
        this.rigidbody = rigidbody;
        this.lightEnergy = lightEnergy;
        
        OnPropulsionEnd();
    }

    /// <summary>
    /// Makes the character follow the left stick's rotation.
    /// </summary>
    public void FollowLeftStickRotation()
    {
        // Get the direction the left stick is pointing to
        Vector2 leftStickDirection = InputManager.GetLeftStick();

        if(leftStickDirection.sqrMagnitude > 0.01f)
        {
            //Debug.Log("Move the player " + Time.time);

            // Get the angle the left stick is pointing in
            float leftStickAngle = 0.0f;
            if (leftStickDirection.x != 0 || leftStickDirection.y != 0)
            {
                // 90-degree offset to ensure angle is relative to +y-axis
                leftStickAngle = Mathf.Atan2(leftStickDirection.y,leftStickDirection.x) * Mathf.Rad2Deg - 90;
            }

            // Make the character face in the direction of the left stick
            transform.localEulerAngles = new Vector3(0,0,leftStickAngle);
        }
    }
    
    /// <summary>
    /// Call this the instant the player starts propulsing
    /// </summary>
    public void OnPropulsionStart()
    {
        foreach (ParticleSystem particles in jetFuelEffect.GetComponentsInChildren<ParticleSystem>())
        {
            particles.Play();
            var em = particles.emission;
            em.enabled = true;
        }
        //jetFuelEffect.enableEmission = true;
    }
    
    public void Propulse(Vector2 direction)
    {
        Propulse(direction,1);
    }

    /// <summary>
    /// Propulse in the given direction, pushing the gameObject in the given direction
    /// <param name="strength">Value between 0 and 1. Determines the speed of the propulsion.</param>
    /// </summary>
    public void Propulse(Vector2 direction, float strength)
    {
        // Compute how much the gameObject has to turn opposite to its velocity vector
        float angleChange = Vector2.Angle((Vector2)rigidbody.velocity, direction);
        // Debug.Log("Change in angle (PlayerMovement.Propulse()): " + angleChange);

        // Augment the thrusting power depending on how much the player has to turn
        float thrustBoost = 1 + (angleChange/180) * changeDirectionBoost;

        // Calculate the final propulsion force
        Vector3 thrustVector = thrustForce * direction * thrustBoost * strength;
        // Push the character in the given direction
        rigidbody.AddForce(thrustVector, ForceMode.Force);
        // Deplete energy from the player for each ejection
        lightEnergy.Deplete(thrustEnergyCost * strength);
    }
    
    /// <summary>
    /// Call this the frame the player releases the propulsion button
    /// </summary>
    public void OnPropulsionEnd()
    {
        foreach (ParticleSystem particles in jetFuelEffect.GetComponentsInChildren<ParticleSystem>())
        {
            var em = particles.emission;
            em.enabled = false;
        }
    }
    
    /// <summary>
    /// Add linear damping on the player's rigidbody 
    /// </summary>
    public void Brake(float strength)
    {
        rigidbody.drag = Mathf.Lerp(defaultDrag, brakeDrag, strength);
    }
}
