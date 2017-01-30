using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Direction
{
    down,
    up,
    left,
    right
}

/// <summary>
/// Current class acts as an invisible boundary
/// Be mindful of the direction the player will hit the boundary
/// This should be the side of the trigger box collider
///
/// @author - Stella L.
/// @author - Karl C.
/// @version - 1.0.0
///
/// </summary>
[RequireComponent(typeof(Collider))]
public class Current : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The strength of current force.")]
    private float strength;
    
    [SerializeField]
    [Tooltip("The direction of current force.")]
    private Direction currentDirection;
    
    [SerializeField]
    [Tooltip("Distance from current for the particle system to be destroyed.")]
    private float distanceFromPlayer;
    
    private Vector3 direction;
    private string particleDirection;
    private float distance;
    // Whether the current object is empty.
    private bool empty;
    // Holds all rigidbodies in the current.
    private List<Rigidbody> rigidbodies;
    private SmoothCamera smoothCamera;
    private bool playerInCurrent;
    private GameObject particles;
    [SerializeField]
    private GenericSoundManager soundManager;

    /// <summary>
    /// Initializes the current.
    /// </summary>
    protected void Start()
    {
        rigidbodies = new List<Rigidbody>();
        GameObject mainCamera = GameObject.FindWithTag("MainCamera");
        if (mainCamera != null)
        {
            this.smoothCamera = mainCamera.GetComponent<SmoothCamera>();
        }
        // By default, the current pushes downward.
        SetDirection();
        empty = true;
        playerInCurrent = false;
        distance = distanceFromPlayer;

        if (soundManager == null)
        {
            GameObject soundGameObject = GameObject.FindWithTag("SoundManager");
            if (soundGameObject !=null)
            {
                soundManager = soundGameObject.GetComponent<GenericSoundManager>();
            }
        }

    }

    /// <summary>
    /// If the current is not empty, add current force.
    /// Do not allow camera to go past current
    /// and only show particles if in view.
///// SHOULD PROBABLY CALL A FUNCTION IN SMOOTH CAMERA INSTEAD //////
    /// </summary>
    protected void Update()
    {
        if (!empty)
        {
            AddCurrentForce();
        }
        if (particles)
        {
            if (particleDirection == "downCurrent" || particleDirection == "upCurrent")
            {
                Vector3 position = particles.transform.position;
                position.x = smoothCamera.transform.position.x;
                particles.transform.position = position;
            }
            if (particleDirection == "leftCurrent" || particleDirection == "rightCurrent")
            {
                Vector3 position = particles.transform.position;
                position.y = smoothCamera.transform.position.y;
                particles.transform.position = position;
            }
            if (!playerInCurrent)
            {
                distance = Vector3.Distance(particles.transform.position, smoothCamera.transform.position);
                if (distance >= distanceFromPlayer)
                {
                    Destroy(particles);
                }
            }
        }
    }

    /// <summary>
    /// Instantiates the particle system.
    /// </summary>
    private void StartCurrentParticles()
    {
        if (!particles)
        {
            Transform child = smoothCamera.gameObject.transform.FindChild(particleDirection);
            particles = (GameObject)Instantiate(child.gameObject, this.transform.position, child.transform.rotation);
            particles.SetActive(true);
            particles.GetComponent<ParticleSystem>().Play();
        }
    }

    /// <summary>
    /// Sets the direction of the current force.
    /// </summary>
    private void SetDirection()
    {
        switch (currentDirection)
        {
            case Direction.down:
                direction = new Vector3(0f, -1f, 0f);
                particleDirection = "downCurrent";
                break;
            case Direction.up:
                direction = new Vector3(0f, 1f, 0f);
                particleDirection = "upCurrent";
                break;
            case Direction.left:
                direction = new Vector3(-1f, 0f, 0f);
                particleDirection = "leftCurrent";
                break;
            case Direction.right:
                direction = new Vector3(1f, 0f, 0f);
                particleDirection = "rightCurrent";
                break;
        }
    }
    /// <summary>
    /// Get rigidbody of the colliding game object and add to the list
    /// </summary>
    protected void OnTriggerEnter(Collider col)
    {
        Rigidbody rigidbody = col.gameObject.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbodies.Add(rigidbody);
            empty = false;
            if (col.CompareTag("Player"))
            {
                StartCurrentParticles();
                smoothCamera.SetCurrentState(true, particleDirection);
                playerInCurrent = true;
            }
        }
    }

    /// <summary>
    /// Remove rigidbody of game object that has exited the current
    /// </summary>
    protected void OnTriggerExit(Collider col)
    {
        Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
        if (rb != null && rigidbodies.Contains(rb))
        {
            rigidbodies.Remove(rb);
            if (rigidbodies.Count == 0)
            {
                empty = true;
            }
            if (col.CompareTag("Player"))
            {
                smoothCamera.SetCurrentState(false, "");
                playerInCurrent = false;
            }
        }
    }

    /// <summary>
    /// Applies the current force to the rigidbodies in the current
    /// </summary>
    private void AddCurrentForce()
    {
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            if (rigidbody != null)
            {
                // Plays the current sound
                if (soundManager)
                {
                    soundManager.CurrentSound(this.gameObject);
                }

                Vector3 initialVelocity = rigidbody.velocity;
                rigidbody.AddForce(-initialVelocity);
                rigidbody.AddForce(strength * direction);
            }
        }
    }
}
