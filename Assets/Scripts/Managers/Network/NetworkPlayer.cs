using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkPlayer : NetworkBehaviour
{

    public bool IsLocalPlayer
    {
        get { return isLocalPlayer; }
    }

    [Command]
    void CmdShootFlare()
    {
        // add flare to registered spawnable prefabs
        // do as normal
        // NetworkServer.Spawn(spawnedObject);
    }
    
    // [Command]
    // void CmdLightToggle()
    // {
        
    // }

}
