using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    /// <summary>
    /// The higher the value, the slower the camera moves.
    /// </summary>
    public float dampTime = 0.15f;
    /// <summary>
    /// The camera will not follow the target if the target this close to the camera's center
    /// </summary>
    [Tooltip("The camera will not follow the target if the target this close to the camera's center")]
    public float deadzoneRadius;
    [Tooltip("The higher this value, the slower the camera follows the target in the deadzone")]
    public float deadzoneDampTime = 0.5f;
    
    [SerializeField]
    [Tooltip("The small speed value")]
    private float speedZommSmall;
    
    [SerializeField]
    [Tooltip("The medium speed value")]
    private float speedZoomMedium;
    
    [SerializeField]
    [Tooltip("Z value for camera when player has small speed")]
    private float smallZoomValue;
    
    [SerializeField]
    [Tooltip("Z value for camera when player has medium speed")]
    private float mediumZoomValue;
    
    [SerializeField]
    [Tooltip("Z value for camera on flare launch")]
    private float maxZoomValue;
    
    [SerializeField]
    [Tooltip("Z value for camera in zoom zones")]
    private float zoomZonesValue;
    
    [SerializeField]
    [Tooltip("Amount of time before camera zooms back into the player")]
    private float timeBeforeZoomIn;
    
    [SerializeField]
    [Tooltip("Camera zoom in speed")]
    private float zoomInSpeed;
    
    [SerializeField]
    [Tooltip("Camera zoom out speed")]
    private float zoomOutSpeed;
    
    /// <summary>
    /// The object that the camera follows.
    /// </summary>
    public Transform target;

    /// <summary>
    /// The camera's default z-value.
    /// </summary>
    public float zPosition;

    private float deadzoneRadiusSquared;
    private new Transform transform;
    
    private Vector2 velocity = Vector2.zero;
    private Rigidbody playerRigidbody;
    private bool acquiredZoom;
    private float zoomTimer;
    private bool shootFlare;
    private bool zoomInZone;
    private bool inCurrents;

    void Awake()
    {
        playerRigidbody =  target.GetComponent<Rigidbody>();
        this.shootFlare = false;
        this.zoomInZone = false;
        this.inCurrents = false;
        transform = GetComponent<Transform>();
        Vector3 position = transform.position;
        position.z = zPosition;
        transform.position = position;
        deadzoneRadiusSquared = deadzoneRadius * deadzoneRadius;
        this.zoomTimer = timeBeforeZoomIn;
        DontDestroyOnLoad(this.gameObject); 
    }
    
    void FixedUpdate()
    {
        if (target)
        {
            Vector3 newPosition = Vector3.zero;
            
            if(!inCurrents)
            {
                float dampTime = this.dampTime;
                Vector3 targetPosition = Vector2.zero;
            
                float distanceFromTarget = ( (Vector2)(target.position - transform.position) ).sqrMagnitude;   
                // Choose a different damping time based on whether or not the target is in the deadzone
                if (distanceFromTarget <= deadzoneRadiusSquared)
                {
                    dampTime = deadzoneDampTime;
                    targetPosition = target.position;
                }
                // Else, if the target isn't in the deadzone
                else
                {
                    // Compute the target position of the camera
                    Vector3 distanceFromPlayer = target.position - transform.position;
                    targetPosition = target.position - distanceFromPlayer.SetMagnitude(deadzoneRadius);
                }

                // Move the camera to its target smoothly.
                newPosition = Vector2.SmoothDamp(transform.position, (Vector2)targetPosition, ref velocity, dampTime);
                // Lock the camera's depth
                newPosition.z = transform.position.z;
            }
            else
            {
                newPosition = this.transform.position;
                newPosition.x = target.transform.position.x;
            }
            
            
            // camera zoom settings
            acquiredZoom = false;
            float playerVelocity = playerRigidbody.velocity.sqrMagnitude;
            float mediumSpeed = speedZoomMedium * speedZoomMedium;
            float smallSpeed = speedZommSmall * speedZommSmall;
            
            if(zoomInZone && !inCurrents)
            {
                if(zoomZonesValue != newPosition.z)
                {
                    //Debug.Log("Zoom zone");
                    newPosition.z = CameraZoom((newPosition.z > zoomZonesValue? zoomOutSpeed : zoomInSpeed), zoomZonesValue);
                }
                acquiredZoom = true;
            }
            
            if(shootFlare && !acquiredZoom)
            {
                if(maxZoomValue != newPosition.z)
                {
                    //Debug.Log("flare shot");
                    newPosition.z = CameraZoom(zoomOutSpeed, maxZoomValue);
                    acquiredZoom = true;
                }
                else
                {
                    //Debug.Log("flare zoom out done");
                    shootFlare = false;
                }
                
            }
            
            if(zoomTimer < timeBeforeZoomIn && !shootFlare)
            {
                //Debug.Log("wait before zoom in");
                zoomTimer += Time.deltaTime;
                acquiredZoom = true;
            }
            
            if(playerVelocity < smallSpeed && !acquiredZoom && zPosition != newPosition.z)
            {
                //Debug.Log("normal");
                newPosition.z = CameraZoom(zoomInSpeed, zPosition);
                acquiredZoom = true;
            }
            
            if(playerVelocity > smallSpeed && playerVelocity < mediumSpeed && !acquiredZoom && smallZoomValue != newPosition.z && !inCurrents)
            {
                //Debug.Log("small");
                newPosition.z = CameraZoom((newPosition.z > smallZoomValue? zoomOutSpeed : zoomInSpeed), smallZoomValue);
                acquiredZoom = true;
            }
            
            if(playerVelocity > mediumSpeed && !acquiredZoom && mediumZoomValue != newPosition.z && !inCurrents)
            {
                //Debug.Log("medium");
                newPosition.z = CameraZoom((newPosition.z > mediumZoomValue? zoomOutSpeed : zoomInSpeed), mediumZoomValue);
                acquiredZoom = true;
            }
            
            //Debug.Log("Move camera to: " + (Vector2)targetPosition);
            transform.position = newPosition;
        }
    }
    
    //this is used for other objects that need the camera to zoom in and out
    
    public float CameraZoom(float zoomSpeed, float zoomToValue)
    {
        //calculate new camera position
        float zValue = Mathf.Lerp(this.transform.position.z, zoomToValue, Time.deltaTime * zoomSpeed);
        //round up the value to 2 digits after point in orther to check when the value is at the desired zoomToValue
        float roundedValue = Mathf.Round(zValue * 100f) / 100f;
        //Debug.Log(roundedValue + " |------| " + (Mathf.Round(zoomToValue * 100f) / 100f));
        
        if(roundedValue == (Mathf.Round(zoomToValue * 100f) / 100f))
        {
            return zoomToValue;
        }
        else
        {
            return zValue;
        }
        
    }
    
    public void FlareShoot()
    {
        //need to check that we aren't in a zoomInZone
        if(!zoomInZone && !inCurrents)
        {
            this.shootFlare = true;
        }
    }
    
    public void ResetTimer()
    {
        this.zoomTimer = 0.0f;
    }
    
    public void SetZoomInZone(bool isZoom)
    {
        this.zoomInZone = isZoom;
        //need to reset if flare was shoot before entering zoomInZone
        this.shootFlare = false;
        this.zoomTimer = timeBeforeZoomIn;
    }
    
    public void SetCurrentState(bool isCurrent)
    {
        this.inCurrents = isCurrent;
        //need to reset if flare was shoot before entering zoomInZone
        this.shootFlare = false;
        this.zoomTimer = timeBeforeZoomIn;
    }
    
    public void StartCurrentParticles(string location)
    {
        foreach(Transform child in this.GetComponentInChildren<Transform>())
        {
            //Debug.Log(child.name +  "==" + location);
            if(child.name == location)
            {
                child.gameObject.SetActive(true);
                child.GetComponent<ParticleSystem>().Play();
            }
        }
    }
    
    public void StopCurrentParticles(string location)
    {
        foreach(Transform child in this.GetComponentInChildren<Transform>())
        {
            //Debug.Log(child.name +  "=!!=" + location);
            if(child.name == location)
            {
                Debug.Log(location);
                child.gameObject.SetActive(false);
                child.GetComponent<ParticleSystem>().Stop();
            }
        }
    }
}