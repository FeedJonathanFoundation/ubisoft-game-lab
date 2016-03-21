using UnityEngine;
using System.Collections;

public class PlayerSound : MonoBehaviour {

    public LayerMask obstacleLayer;

    private float crashVelocity;

    void Start()
    {
        AkSoundEngine.PostEvent("Default", this.gameObject);
        
    }

    // Update is called once per frame
    void Update()
    {
        // if (true)// crashes into obstacle layer)
        // {
        //     AkSoundEngine.SetState("MainStateGroup", "WallCrash");
        //     // velocity
        // }
        // else
        // {
        //     AkSoundEngine.SetState("MainStateGroup", "Idle");
        // }
	}
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == obstacleLayer)
        {
            AkSoundEngine.SetState("MainStateGroup", "WallCrash");
        }
    }
}
