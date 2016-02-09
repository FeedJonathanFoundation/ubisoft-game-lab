using UnityEngine;
using System.Collections;

public class PlayerFlare : MonoBehaviour
{
    //variables not for user
    private new Rigidbody rigidbody;            //rigibody information, init in Start()
    private new Light light = new Light();      //light of the flare, init in Start()
    private float intensity;                    //intensity of light, used for decay of light

    //variables changable in the interface
    public float speed, timer, destroyTime;     //speed influence distance of flare, timer is for interval of decay of light, destroyTime is delay before destroying flare

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        light = gameObject.GetComponentInChildren<Light>();
        rigidbody.velocity = transform.right * speed;
    }
	
	// Update is called once per frame
	void Update()
    {
        //might need to put if rotating!
        //timers could help with frames
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            light.intensity = 5; 
        }
        Destroy(gameObject, destroyTime);
	}
}
