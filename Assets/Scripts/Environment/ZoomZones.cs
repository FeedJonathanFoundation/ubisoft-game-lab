using UnityEngine;
using System.Collections;

public class ZoomZones : MonoBehaviour 
{
    private SmoothCamera smoothCamera;

    // Use this for initialization
    void Start () 
    {
        GameObject mainCamera = GameObject.Find("Main Camera");
        if (mainCamera != null)
        {
            this.smoothCamera = mainCamera.GetComponent<SmoothCamera>();
        }
    }
    
    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            this.smoothCamera.SetZoomInZone(true);
        }
    }

    void OnTriggerExit(Collider col) 
    {
        if(col.CompareTag("Player"))
        {
            this.smoothCamera.SetZoomInZone(false);
        }
    }
}
