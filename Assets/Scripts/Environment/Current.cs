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
    private GameObject particleSystem;


    void Start()
    {
        rigidbodies = new List<Rigidbody>();
        GameObject mainCamera = GameObject.Find("Main Camera");
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

        if (particleSystem)
        {
            if (particleDirection == "downCurrent" || particleDirection == "upCurrent")
            {
                Vector3 position = particleSystem.transform.position;
                position.x = smoothCamera.transform.position.x;
                particleSystem.transform.position = position;
            }

            if (particleDirection == "leftCurrent" || particleDirection == "rightCurrent")
            {
                Vector3 position = particleSystem.transform.position;
                position.y = smoothCamera.transform.position.y;
                particleSystem.transform.position = position;
            }

            if (!playerInCurrent)
            {
                distance = Vector3.Distance(particleSystem.transform.position, smoothCamera.transform.position);
                //Debug.Log("distance: " + distance);
                if (distance >= distanceFromPlayer)
                {
                    Destroy(particleSystem);
                    particleSystem = null;
                }
            }
        }
    }

    void StartCurrentParticles()
    {
        if (!particleSystem)
        {
            foreach (Transform child in smoothCamera.GetComponentInChildren<Transform>())
            {
                //Debug.Log(child.name +  "==" + particleDirection);
                if (child.name == particleDirection)
                {
                    Vector3 position = this.transform.position;

                    if (particleDirection == "downCurrent")
                    {
                        position.y *= 1.05f;
                    }

                    if (particleDirection == "upCurrent")
                    {
                        position.y *= -1.05f;
                    }

                    if (particleDirection == "leftCurrent")
                    {
                        position.x *= 1.05f;
                    }

                    if (particleDirection == "rightCurrent")
                    {
                        position.x *= 1.6f;
                    }

                    particleSystem = (GameObject)Instantiate(child.gameObject, position, child.transform.rotation);
                    particleSystem.SetActive(true);
                    particleSystem.GetComponent<ParticleSystem>().Play();
                }
            }
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
                //smoothCamera.StartCurrentParticles(particleDirection);
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
