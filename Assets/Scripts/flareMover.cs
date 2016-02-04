using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {
    private Rigidbody rigidBody;
    public float speed,timer, destroyTime;
    private Light light;
    private float intensity;
	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();
        light = gameObject.GetComponentInChildren<Light>();
        rigidBody.velocity = transform.right * speed;
    }
	
	// Update is called once per frame
	void Update () {
        //might need to put if rotating!
        //timers could help with frames
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            light.intensity = 5; 
        }
        Destroy(gameObject, destroyTime);
	}
}
