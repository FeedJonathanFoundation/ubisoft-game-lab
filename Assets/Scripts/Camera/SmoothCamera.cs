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
    [Tooltip("Amount of time before camera zooms back into the player")]
    private float timeBeforeZoomIn;
    
    [SerializeField]
    [Tooltip("Camera zoom in and out speed")]
    private float zoomSpeed;
    
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
    public bool shootFlare;

    void Awake()
    {
        playerRigidbody =  target.GetComponent<Rigidbody>();
        shootFlare = false;
        transform = GetComponent<Transform>();
        Vector3 position = transform.position;
        position.z = zPosition;
        transform.position = position;
        deadzoneRadiusSquared = deadzoneRadius * deadzoneRadius;
        zoomTimer = timeBeforeZoomIn;
        DontDestroyOnLoad(this.gameObject); 
    }
    
    void FixedUpdate()
    {
        if (target)
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
            Vector3 newPosition = Vector2.SmoothDamp(transform.position, (Vector2)targetPosition, ref velocity, dampTime);
            // Lock the camera's depth
            newPosition.z = transform.position.z;
            
            
            // camera zoom settings
            acquiredZoom = false;
            float playerVelocity = playerRigidbody.velocity.sqrMagnitude;
            float mediumSpeed = speedZoomMedium * speedZoomMedium;
            float smallSpeed = speedZommSmall * speedZommSmall;
            
            if(shootFlare)
            {
                if(maxZoomValue != newPosition.z)
                {
                    //Debug.Log("flare shot");
                    newPosition.z = CameraZoom(-zoomSpeed, maxZoomValue);
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
                //Debug.Log("NORMAL");
                newPosition.z = CameraZoom(zoomSpeed, zPosition);
                acquiredZoom = true;
            }
            
            if(playerVelocity > smallSpeed && playerVelocity < mediumSpeed && !acquiredZoom && smallZoomValue != newPosition.z)
            {
                //Debug.Log("small");
                newPosition.z = CameraZoom((newPosition.z > smallZoomValue? -zoomSpeed : zoomSpeed), smallZoomValue);
                acquiredZoom = true;
            }
            
            if(playerVelocity > mediumSpeed && !acquiredZoom && mediumZoomValue != newPosition.z)
            {
                //Debug.Log("medium");
                newPosition.z = CameraZoom((newPosition.z > mediumZoomValue? -zoomSpeed : zoomSpeed), mediumZoomValue);
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
        float zValue = this.transform.position.z;
        zValue += (Time.deltaTime * zoomSpeed);
        
        if((int)zValue != zoomToValue)
        {
            return zValue;
        }
        else
        {
            return zoomToValue;
        }
    }
    
    public void FlareShoot()
    {
        this.shootFlare = true;
    }
    
    public void ResetTimer()
    {
        this.zoomTimer = 0.0f;
    }
}