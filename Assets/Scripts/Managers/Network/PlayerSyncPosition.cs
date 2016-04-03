using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerSyncPosition : NetworkBehaviour
{
    [SyncVar]
    private Vector3 syncPosition;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private float lerpRate = 15f;
    
    void FixedUpdate()
    {
        TransmitPosition();
        LerpPosition();
    }
    
    // Used to provide player's position
    // Only used for other clients to smooth out movement
    void LerpPosition()
    {
        if (isLocalPlayer)
        {
            playerTransform.position = Vector3.Lerp(playerTransform.position, syncPosition, Time.deltaTime * lerpRate);
        }
    }
    
    // Server receives this value
    [Command]
    void CmdProvidePositionToServer(Vector3 position)
    {
        syncPosition = position;
    }
    
    // transmits this value to all clients
    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer)
        {
            CmdProvidePositionToServer(playerTransform.position);
        }
    }
    
}
