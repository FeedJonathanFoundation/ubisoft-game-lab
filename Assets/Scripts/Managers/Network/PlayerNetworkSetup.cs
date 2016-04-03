using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerNetworkSetup : NetworkBehaviour
{
    [SerializeField]
    private Camera playerCamera;
    
    void Start()
    {
	    if (isLocalPlayer)
        {
            GameObject.Find("Scene Camera").SetActive(false);
            // GetComponent<>().enabled = true;
            GetComponent<Player>().enabled = true;
            playerCamera.enabled = true;
        }
	}
	

}
