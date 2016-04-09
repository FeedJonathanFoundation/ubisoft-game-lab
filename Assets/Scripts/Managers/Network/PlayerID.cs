using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerID : NetworkBehaviour
{

    [SyncVar]
    private string playerUniqueIdentity;
    private NetworkInstanceId playerNetworkID;
    private Transform playerTransform;
    [Tooltip("What the game object is called when it is cloned.")]
    [SerializeField]
    private string playerPrefabCloneName = "Player(Clone)";

    public override void OnStartLocalPlayer()
    {
        GetNetIdentity();
        SetIdentity();
    }
    
    void Awake()
    {
        playerTransform = transform;
    }
    
    void Update()
    {
        if (playerTransform.name == "" || playerTransform.name == playerPrefabCloneName)
        {
            SetIdentity();
        }
    }
    
    [Client]
    void GetNetIdentity()
    {
        playerNetworkID = GetComponent<NetworkIdentity>().netId;
        CmdTellServerMyIdentity(MakeUniqueIdentity());
    }

    [Client]
    void SetIdentity()
    {
        if (!isLocalPlayer)
        {
            playerTransform.name = playerUniqueIdentity;
        }
        else
        {
            playerTransform.name = MakeUniqueIdentity();
        }
    }

    string MakeUniqueIdentity()
    {
        string uniqueIdentity = "Player" + playerNetworkID.ToString();
        return uniqueIdentity;
    }
    
    [Command]
    void CmdTellServerMyIdentity(string identity)
    {
        playerUniqueIdentity = identity;
    }

}
