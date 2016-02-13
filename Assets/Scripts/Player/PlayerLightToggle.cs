using UnityEngine;
using System.Collections;

public class PlayerLightToggle
{
    /// <summary>
    /// If true, the lights that are children of this object are enabled.
    /// </summary>
	private bool lightsEnabled;
    private GameObject lightsToToggle;
    private float timerLostOfLight; //timer for lost of light
    private LightSource lightSource;
    private float minimalEnergyRestriction;

    public PlayerLightToggle(GameObject lightsToToggle, bool defaultLightStatus,LightSource lightSourceRef, float minimalEnergy)
    {
        this.lightsEnabled = defaultLightStatus;
        this.lightsToToggle = lightsToToggle;
        lightSource = lightSourceRef; //initiate lightSource
        minimalEnergyRestriction = minimalEnergy;
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

    public void lostOfLight(float lostOfLightTimeInterval, float energyCostLightToggle)
    {
        
        if (this.lightsEnabled)
        {
            timerLostOfLight += Time.deltaTime;
            if (timerLostOfLight > lostOfLightTimeInterval)
            {
                lightSource.LightEnergy.Deplete(energyCostLightToggle);
                timerLostOfLight = 0;

                if (minimalEnergyRestriction >= lightSource.LightEnergy.CurrentEnergy)
                {
                    Debug.Log("turning off...");
                    this.ToggleLights();
                }
            }
        }
    }
}
