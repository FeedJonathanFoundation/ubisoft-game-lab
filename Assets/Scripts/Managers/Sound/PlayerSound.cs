using UnityEngine;
using System.Collections;

public class PlayerSound : MonoBehaviour {

    public LayerMask obstacleLayer;

    private float crashVelocity;

    void Start()
    {
        //AkSoundEngine.PostEvent("Default", this.gameObject); 
    }
    void OnCollisionEnter(Collision collision)
    {
        //AkSoundEngine.PostEvent("WallCrash", this.gameObject);
    }
}
