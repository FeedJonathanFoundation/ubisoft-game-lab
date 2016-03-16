using UnityEngine;
using System.Collections;

public class BossEatFlare : MonoBehaviour 
{
    [Tooltip("Size of the mouth of the boss fish")]
    public float mouthSize;

    // Use this for initialization
    void Start () 
    {
        //set the radius
        this.GetComponent<SphereCollider>().radius = mouthSize;
    }
    
    void OnTriggerEnter(Collider col) 
    {
        if(col.tag == "Flare") // have to put this because LightAbsorber has a player tag
        {
            Destroy(col.transform.parent.gameObject);
        }
    }
}
