using UnityEngine;

public class FlareSpawner : MonoBehaviour 
{
    [SerializeField]
    [Tooltip("Refers to flare game object")]
    private GameObject flareObject;
    
    [SerializeField]
    [Tooltip("Refers to the flare spwan zone")]
    private Transform flareSpawnObject;
    
    [SerializeField]
    [Tooltip("Time between flare shoots")]
    private float cooldownTime;
    
    [SerializeField]
    [Tooltip("Cost to use your flare")]
    private float flareEnergyCost;
    
    [SerializeField]
    [Tooltip("Percentage of energy needed to use flare. 1 = 100%")]
    private float flareCostPercentage;
    
    [SerializeField]
    [Tooltip("The amount of recoil applied on the player when shooting the flare")]
    private float recoilForce;
        
    private float timer;
    private LightSource lightSource;
    private new Rigidbody rigidbody;    
    private SmoothCamera smoothCamera;
	

    void Start()
    {
        this.timer = cooldownTime;
        this.lightSource = GetComponent<LightSource>();
        this.rigidbody = GetComponent<Rigidbody>();
        GameObject mainCamera = GameObject.Find("Main Camera");
        if (mainCamera != null)
        {
            this.smoothCamera = mainCamera.GetComponent<SmoothCamera>();
        }
    }

    void Update() 
    {
        if ((timer += Time.deltaTime) >= cooldownTime)
        {
            if (Input.GetButtonDown("UseFlare"))
            {
                float cost = flareEnergyCost * flareCostPercentage;

                if (lightSource.LightEnergy.CurrentEnergy > (flareEnergyCost + cost))
                {
                    Instantiate(flareObject, flareSpawnObject.position, flareSpawnObject.rotation);
                    lightSource.LightEnergy.Deplete(flareEnergyCost);
                    // Apply recoil in the opposite direction the flare was shot
                    rigidbody.AddForce(-flareSpawnObject.right * recoilForce, ForceMode.Impulse);
                    timer = 0.0f;
                    //reset all values for the zoom when ever a player fires a flare
                    if (smoothCamera != null)
                    {
                        smoothCamera.FlareShoot();
                        smoothCamera.ResetTimer();   
                    }                    
                }
            }
        }
    }
}