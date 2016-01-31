using UnityEngine;
using System.Collections;

public class LightToggle : MonoBehaviour 
{
    [Tooltip("If true, the lights are enabled on scene start.")]
    public bool beginEnabled = true;
    
    /// <summary>
    /// If true, the lights that are children of this object are enabled.
    /// </summary>
	private bool lightsEnabled = true;
    
    void Start()
    {
        ToggleLights(beginEnabled);
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (Input.GetButtonDown("LightToggle")) 
        {
		  ToggleLights(!lightsEnabled);
		}
	}
    
    public void ToggleLights(bool toggle)
    {
        foreach (Behaviour childComponent in GetComponentsInChildren<Light>()) 
        {
            childComponent.enabled = toggle;
        }
        
        // Update the status of the lights
        lightsEnabled = toggle;
    }
}
