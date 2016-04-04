using UnityEngine;

public class FlareSpawner : MonoBehaviour 
{
    [SerializeField]
    [Tooltip("Refers to flare game object")]
    private GameObject flareObject;
    
    [SerializeField]
    [Tooltip("Refers to the flare spawn zone")]
    private Transform flareSpawnObject;
    
    [SerializeField]
    [Tooltip("Time between flare shoots")]
    private float cooldownTime;
    
    [SerializeField]
    [Tooltip("Cost to use your flare")]
    private float flareEnergyCost;
    
    [SerializeField]
    [Range(0,1)]
    [Tooltip("Percentage of energy needed to use flare. 1 = 100%")]
    private float flareCostPercentage;
    
    [SerializeField]
    [Tooltip("The amount of recoil applied on the player when shooting the flare")]
    private float recoilForce;
        
    private float timer;
    private LightSource lightSource;
    // Caches the component that rumbles the controller
    private ControllerRumble controllerRumble;
    private new Rigidbody rigidbody;    
    private SmoothCamera smoothCamera;
    private float flareDistance = 0f;
    private Transform player;
    private GameObject flare;
    private FlareSound flareSound;
    private PlayerSound playerSound;

    void Start()
    {
        this.timer = cooldownTime;
        this.lightSource = GetComponent<LightSource>();
        this.controllerRumble = GetComponent<ControllerRumble>();
        this.rigidbody = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player").transform;
        playerSound = player.GetComponent<PlayerSound>();
        GameObject mainCamera = GameObject.Find("Main Camera");
        if (mainCamera != null)
        {
            this.smoothCamera = mainCamera.GetComponent<SmoothCamera>();
        }
    }

    void Update() 
    {
        bool ready = false;
        if (timer < cooldownTime)
        {
            timer += Time.deltaTime;
        }
        else
        {
            ready = true;
        }
        if (Input.GetButtonDown("UseFlare"))
        {
            if (ready)
            {
                float cost = flareEnergyCost * flareCostPercentage;
                if ((lightSource.LightEnergy.CurrentEnergy > (flareEnergyCost + cost)))
                {
                    flare = (GameObject)Instantiate(flareObject, flareSpawnObject.position, flareSpawnObject.rotation);
                    lightSource.LightEnergy.Deplete(flareEnergyCost);
                    // Apply recoil in the opposite direction the flare was shot
                    rigidbody.AddForce(-flareSpawnObject.right * recoilForce, ForceMode.Impulse);
                    controllerRumble.ShotFlare();   // Rumble the controller
                    timer = 0.0f;
                    flareSound = new FlareSound(flare);
                    flareSound.ShootFlareSound();

                    //reset all values for the zoom whenever player fires a flare
                    if (smoothCamera != null)
                    {
                        smoothCamera.FlareShoot();
                        smoothCamera.ResetTimer();
                    }
                }
            }
            else
            {
                if (playerSound == null)
                {
                    playerSound = player.GetComponent<PlayerSound>();
                }
                if (playerSound != null)
                {
                    playerSound.InsufficientEnergySound();
                }
            }
        }
        if (flare != null)
        {
            flareDistance = Vector3.Distance(flare.transform.position, player.position);
            flareSound.SetFlareDistance(flareDistance);
        }
    }
    
    public void EatFlare()
    {
        flareSound.EatFlareSound();
    }
}