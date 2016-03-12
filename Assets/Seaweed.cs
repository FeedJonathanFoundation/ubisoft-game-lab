using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Seaweed : MonoBehaviour
{
    [SerializeField]
    private float gravity = -9.8f;

    private Rigidbody rigidBody;

    // Use this for initialization
    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
        ReverseGravity();
    }
    void Update() 
    {
        rigidBody.velocity += new Vector3(0, gravity * Time.deltaTime, 0);
    }
    public void ReverseGravity()
    {
        gravity = -gravity;
    }
	
}
