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
    private Vector3 lastPosition;
    private float threshold = 0.5f;

    void Update()
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
    
    // Transmits this value to all clients
    // Only sends commands if the player has moved at least the threshold value
    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer && Vector3.Distance(playerTransform.position, lastPosition) > threshold)
        {
            CmdProvidePositionToServer(playerTransform.position);
            lastPosition = playerTransform.position;
        }
    }
    
}
