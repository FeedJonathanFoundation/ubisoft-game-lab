using UnityEngine;
using System.Collections;

public class WhaleMovement : MonoBehaviour
{

    [Tooltip("The strength of movement force.")]
    public float speed;
    public Transform target;

	
	// Update is called once per frame
	void Update()
    {
        Move();
    }
    
    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }
    
    // If in contact with a disable collider, disables the game object
    void OnTriggerEnter(Collider col)
    {
        gameObject.SetActive(false);
    }
}
