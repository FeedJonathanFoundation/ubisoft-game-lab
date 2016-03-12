using UnityEngine;
using System.Collections;

public class Seaweed : MonoBehaviour
{
    [SerializeField]
    private float gravity = -9.8f;

    private Rigidbody rb;

    // Use this for initialization
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
        ReverseGravity();
    }
    void Update() 
    {
        rb.velocity += new Vector3(0, gravity * Time.deltaTime, 0);
    }
    public void ReverseGravity()
    {
        gravity = -gravity;
    }
	
}
