using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;


// If syncing too slowly, increase lerp rate
public class PlayerSyncRotation : NetworkBehaviour
{

    [SyncVar (hook = "OnPlayerRotationSynced")]
    private float syncPlayerRotation;
    [SyncVar (hook = "OnCameraRotationSynced")]
    private float syncCameraRotation;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private float lerpRate = 20;
    private float lastPlayerRotation;
    private float lastCameraRotation;
    [Tooltip("Number of degrees that must change before sending an update to the server.")]
    [SerializeField]
    private float threshold = 1f;

    private List<float> syncPlayerRotationList = new List<float>();
    private List<float> syncCameraRotationList = new List<float>();
    [SerializeField]
    private float buffer = 0.3f;
    [SerializeField]
    private bool useSavedLerping;

    void Update()
    {
        LerpRotations();
    }
    
    void FixedUpdate()
    {
        TransmitRotations();
    }
    
    void LerpRotations()
    {
        if (!isLocalPlayer)
        {
            // playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, syncPlayerRotation, Time.deltaTime * lerpRate);
            // cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, syncCameraRotation, Time.deltaTime * lerpRate);
            if (useSavedLerping)
            {
                SavedLerping();
            }
            else
            {
                OrdinaryLerping();
            }
        }
    }
    
    void OrdinaryLerping()
    {
        LerpPlayerRotation(syncPlayerRotation);
        LerpCameraRotation(syncCameraRotation);
    }
    
    void LerpPlayerRotation(float rotationAngle)
    {
        Debug.Log("If angle axis is changed, need to change new rotation vector 3");
        Vector3 newRotation = new Vector3(0, rotationAngle, 0);
        playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, Quaternion.Euler(newRotation), lerpRate * Time.deltaTime);
    }
    
    void LerpCameraRotation(float rotationAngle)
    {
        Debug.Log("If angle axis is changed, need to change new rotation vector 3");
        Vector3 newRotation = new Vector3(rotationAngle, 0, 0);
        cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, Quaternion.Euler(newRotation), lerpRate * Time.deltaTime);
    }
    
    void SavedLerping()
    {
        if (syncPlayerRotationList.Count > 0)
        {
            LerpPlayerRotation(syncPlayerRotationList[0]);
            
            float difference = Mathf.Abs(playerTransform.localEulerAngles.y - syncPlayerRotationList[0]);
            if (difference < buffer)
            {
                syncPlayerRotationList.RemoveAt(0);
            }
        }
        if (syncCameraRotationList.Count > 0)
        {
            LerpCameraRotation(syncCameraRotationList[0]);
            
            float difference = Mathf.Abs(cameraTransform.localEulerAngles.x - syncCameraRotationList[0]);
            if (difference < buffer)
            {
                syncCameraRotationList.RemoveAt(0);
            }
        }
    }
    
    [Command]
    void CmdProvideRotationsToServer(float playerRotation, float cameraRotation)
    {
        syncPlayerRotation = playerRotation;
        syncCameraRotation = cameraRotation;
    }
    
    [Client]
    void TransmitRotations()
    {
        if (isLocalPlayer)
        {
            Debug.Log("CHANGE AXIS THAT THE VALUES ROTATE AROUND!!!!!!!!!!!");
            if (ExceedsThreshold(playerTransform.localEulerAngles.y, lastPlayerRotation) || ExceedsThreshold(cameraTransform.localEulerAngles.x, lastCameraRotation))
            // if (Quaternion.Angle(playerTransform.rotation, lastPlayerRotation) > threshold || Quaternion.Angle(cameraTransform.rotation, lastCameraRotation) > threshold)
            {
                lastPlayerRotation = playerTransform.localEulerAngles.y;
                lastCameraRotation = cameraTransform.localEulerAngles.x;
                CmdProvideRotationsToServer(lastPlayerRotation, lastCameraRotation);
            }
        }
    }
    
    private bool ExceedsThreshold(float rotation1, float rotation2)
    {
        float difference = Mathf.Abs(rotation1 - rotation2);
        if (difference > threshold) { return true; }
        return false;
    }
    
    [Client]
    void OnPlayerRotationSynced(float latestPlayerRotation)
    {
        syncPlayerRotation = latestPlayerRotation;
        syncPlayerRotationList.Add(syncPlayerRotation);
    }
    
    [Client]
    void OnCameraRotationSynced(float latestCameraRotation)
    {
        syncCameraRotation = latestCameraRotation;
        syncCameraRotationList.Add(syncCameraRotation);
    }
}
