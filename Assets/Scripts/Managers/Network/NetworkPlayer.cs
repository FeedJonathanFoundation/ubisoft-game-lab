using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkPlayer : NetworkBehaviour
{

    public bool IsLocalPlayer
    {
        get { return isLocalPlayer; }
    }
}
