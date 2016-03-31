﻿using UnityEngine;
using System.Collections;

public class JellyfishBounce : MonoBehaviour 
{
    //NOTE IMPORTANT
    //in order to make this script work and to make the jellyfish not get affected by the collision
    //make sure is kenematic is checked in rigidbody
    [SerializeField]
    [Tooltip("Intensity that the player bounces on the jellyfish.")]
    private float bounceIntensity;
    
    void OnCollisionEnter(Collision col)
    {
        //Check if you have to be specific to an object to bounce
        //this could not exist and the player would still bounce off the body as long as istrigger is not checked
        if(col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<Rigidbody>().velocity = Vector3.Reflect(col.relativeVelocity*-bounceIntensity, col.contacts[0].normal );
        }
    }
}
