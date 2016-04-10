using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerNetworkSetup : NetworkBehaviour
{
    [SerializeField]
    private GameObject playerCamera;
    
    void Start()
    {
	    if (isLocalPlayer)
        {
            GameObject sceneCamera = GameObject.Find("Scene Camera");
            if (sceneCamera != null)
            {
                sceneCamera.SetActive(false);
            }
            // GetComponent<>().enabled = true;
            GetComponent<Player>().enabled = true;
            //playerCamera.enabled = true;
            
            SmoothCamera camera = ((GameObject)GameObject.Instantiate(playerCamera, transform.position, Quaternion.Euler(0,0,0))).GetComponent<SmoothCamera>();
            camera.Target = transform;
            camera.Init();
        }
	}
	

}
