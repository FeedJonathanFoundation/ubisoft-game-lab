using UnityEngine;
using System.Collections;

public class FlareSpawner : MonoBehaviour 
{

    [Tooltip("Refers to flare game object")]
    public GameObject flareObject;
    [Tooltip("Refers to the flare spwan zone")]
    public Transform flareSpawnObject;
    [Tooltip("Time between flare shoots")]
    public float cooldownTime;
    [Tooltip("Cost to use your flare")]
    public float flareEnergyCost;
    [Tooltip("Percentage of energy needed to use flare. 1 = 100%")]
    public float flareCostPercentage;
    [Tooltip("The amount of recoil applied on the player when shooting the flare")]
    public float recoilForce;

    private float timer;
    private LightSource lightSource;
    private new Rigidbody rigidbody;
	
    // Use this for initialization
	void Start () 
	{
        timer = cooldownTime;
        lightSource = GetComponent<LightSource>();
        rigidbody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () 
	{
        if((timer += Time.deltaTime) >= cooldownTime)
        {
            if (Input.GetButtonDown("UseFlare"))
            {
                if(lightSource.LightEnergy.CurrentEnergy > flareEnergyCost * flareCostPercentage)
                {
                    Instantiate(flareObject, flareSpawnObject.position, flareSpawnObject.rotation);
                    lightSource.LightEnergy.Deplete(flareEnergyCost);
                    // Apply recoil in the opposite direction the flare was shot
                    rigidbody.AddForce(-flareSpawnObject.right * recoilForce, ForceMode.Impulse);
                    timer = 0.0f;
                }
            }
        }
	}
}
