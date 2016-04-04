using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerSyncRotation : NetworkBehaviour
{

    [SyncVar]
    private Quaternion syncPlayerRotation;
    [SyncVar]
    private Quaternion syncCameraRotation;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private float lerpRate = 15;
    private Quaternion lastPlayerRotation;
    private Quaternion lastCameraRotation;
    [SerializeField]
    private float threshold = 5;

    void Update()
    {
        TransmitRotations();
        LerpRotations();
    }
    
    void LerpRotations()
    {
        if (isLocalPlayer)
        {
            playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, syncPlayerRotation, Time.deltaTime * lerpRate);
            cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, syncCameraRotation, Time.deltaTime * lerpRate);
        }
    }
    
    [Command]
    void CmdProvideRotationsToServer(Quaternion playerRotation, Quaternion cameraRotation)
    {
        syncPlayerRotation = playerRotation;
        syncCameraRotation = cameraRotation;
    }
    
    [Client]
    void TransmitRotations()
    {
        if (isLocalPlayer)
        {
            if (Quaternion.Angle(playerTransform.rotation, lastPlayerRotation) > threshold || Quaternion.Angle(cameraTransform.rotation, lastCameraRotation) > threshold)
            {
                CmdProvideRotationsToServer(playerTransform.rotation, cameraTransform.rotation);
                lastPlayerRotation = playerTransform.rotation;
                lastCameraRotation = cameraTransform.rotation;
            }
        }
    }
}
