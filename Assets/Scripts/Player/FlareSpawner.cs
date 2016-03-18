using UnityEngine;
using System.Collections;

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
    
    [Tooltip("Reference to the main camera")]
    public GameObject mainCamera;
    
    [Tooltip("Z value for camera on flare launch")]
    public float zoomOutValue;
    
    [Tooltip("Amount of time before camera zooms back into the player")]
    public float timeBeforeZoomIn;
    
    [Tooltip("Camera zoom in and out speed")]
    public float zoomSpeed;
    
    [SerializeField]
    [Tooltip("The amount of recoil applied on the player when shooting the flare")]
    private float recoilForce;
    private float timer;
    private LightSource lightSource;
    private new Rigidbody rigidbody;
    private float originalCameraValue;
    private int isZoom;             //determines if we are zooming in or out
    private float zoomTimer;        //used for the delayed before zooming back in
	

    void Start()
    {
        timer = cooldownTime;
        lightSource = GetComponent<LightSource>();
        rigidbody = GetComponent<Rigidbody>();
        originalCameraValue = mainCamera.transform.position.z;
        isZoom = 2;
    }

    void Update() 
    {
        if((timer += Time.deltaTime) >= cooldownTime)
        {
            if (Input.GetButtonDown("UseFlare"))
            {
                float cost = flareEnergyCost * flareCostPercentage;

                if(lightSource.LightEnergy.CurrentEnergy > (flareEnergyCost + cost))
                {
                    Instantiate(flareObject, flareSpawnObject.position, flareSpawnObject.rotation);
                    lightSource.LightEnergy.Deplete(flareEnergyCost);
                    // Apply recoil in the opposite direction the flare was shot
                    rigidbody.AddForce(-flareSpawnObject.right * recoilForce, ForceMode.Impulse);
                    timer = 0.0f;
                    //reset all values for the zoom when ever a player fires a flare
                    zoomTimer = timeBeforeZoomIn;
                    zoomSpeed *= -1;
                    isZoom = 0;
                }
            }
        }
        
        if(isZoom != 2)
        {
            CameraZoom();
        }
    }
    
    void CameraZoom()
    {
        if((zoomTimer += Time.deltaTime) >= timeBeforeZoomIn)
        {
            //calculate new camera position
            float zValue = mainCamera.transform.position.z;
            zValue += (Time.deltaTime * zoomSpeed);
            Vector3 position = mainCamera.transform.position;
            //sets compared value depending on if we are zooming in or out
            float compareValue = (isZoom == 0? zoomOutValue : originalCameraValue);
            if((int)zValue != compareValue)
            {
                position.z = zValue;
            }
            else
            {
                position.z = compareValue;
                zoomTimer = 0.0f;
                isZoom++;
                //this to prevent a mistake when reseting when the user fires a new flare
                if(isZoom < 2)
                {
                    zoomSpeed *= -1;
                }
            }
            mainCamera.transform.position = position;
        }
    }
}