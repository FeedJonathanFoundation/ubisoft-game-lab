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

// Acts as an invisible boundary
// Be mindful of the direction the player will hit the boundary
// This should be the side of the trigger box collider
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


    void Start()
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
    }

    void Update()
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
                //Debug.Log("distance: " + distance);
                if (distance >= distanceFromPlayer)
                {
                    Destroy(particles);
                }
            }
        }
    }

    void StartCurrentParticles()
    {
        if (!particles)
        {
            Transform child = smoothCamera.gameObject.transform.FindChild(particleDirection);
            particles = (GameObject)Instantiate(child.gameObject, this.transform.position, child.transform.rotation);
            particles.SetActive(true);
            particles.GetComponent<ParticleSystem>().Play();
        }
    }

    void SetDirection()
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

    // Get rigidbody of the colliding game object and add to the list
    void OnTriggerEnter(Collider col)
    {
        Rigidbody rigidbody = col.gameObject.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbodies.Add(rigidbody);
            empty = false;

            if (col.CompareTag("Player"))
            {
                Debug.Log(particleDirection);
                StartCurrentParticles();
                smoothCamera.SetCurrentState(true, particleDirection);
                playerInCurrent = true;
            }
        }
    }

    // Remove rigidbody of game object that has exited the current
    void OnTriggerExit(Collider col)
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
                //smoothCamera.StopCurrentParticles(particleDirection);
            }
        }
    }

    void AddCurrentForce()
    {
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            if (rigidbody != null)
            {
                AkSoundEngine.PostEvent("Current", this.gameObject);
                Vector3 initialVelocity = rigidbody.velocity;
                rigidbody.AddForce(-initialVelocity);
                rigidbody.AddForce(strength * direction);
            }
        }
    }
}
