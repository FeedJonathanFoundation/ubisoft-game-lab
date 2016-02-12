using UnityEngine;
using System.Collections;

public class PlayerLightToggle
{
    /// <summary>
    /// If true, the lights that are children of this object are enabled.
    /// </summary>
	private bool lightsEnabled;
    private GameObject lightsToToggle;

    public PlayerLightToggle(GameObject lightsToToggle, bool defaultLightStatus)
    {
        this.lightsEnabled = defaultLightStatus;
        this.lightsToToggle = lightsToToggle;
        ToggleLights();
    }

    public void ToggleLights()
    {
        foreach (Behaviour childComponent in this.lightsToToggle.GetComponentsInChildren<Light>())
        {
            childComponent.enabled = !this.lightsEnabled;
        }
        // Update the status of the lights
        this.lightsEnabled = !this.lightsEnabled;
    }
}
