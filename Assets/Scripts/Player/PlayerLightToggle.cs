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
        this.lightSource = lightSourceRef; //initiate lightSource
        this.minimalEnergyRestriction = minimalEnergy;
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

    public void LostOfLight(float lostOfLightTimeInterval, float energyCostLightToggle)
    {
        
        if (this.lightsEnabled)
        {
            this.timerLostOfLight += Time.deltaTime;
            if (this.timerLostOfLight > lostOfLightTimeInterval)
            {
                this.lightSource.LightEnergy.Deplete(energyCostLightToggle);
                this.timerLostOfLight = 0;

                if (this.minimalEnergyRestriction >= this.lightSource.LightEnergy.CurrentEnergy)
                {
                    this.ToggleLights();
                }
            }
        }
    }
    
    /// <summary>
    /// If true, the lights are enabled and the GameObject is visible.
    /// </summary>
    public bool GetLightsEnabled()
    {
        if (this.lightsEnabled)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
