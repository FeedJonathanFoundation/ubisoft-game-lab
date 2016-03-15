using UnityEngine;

/// <summary>
/// PlayerLightToggle class is responsible for behaviour related to player lights.  
///
/// @author - Simon T.
/// @author - Alex I.
/// @version - 1.0.0
///
/// </summary>
public class PlayerLightToggle
{
    // If true, the lights that are children of this object are enabled.
	private bool lightsEnabled;
    private GameObject lightsToToggle;
    private float timerLostOfLight; 
    private LightSource lightSource;
    private float minimalEnergyRestriction;

    public PlayerLightToggle(GameObject lightsToToggle, bool defaultLightStatus, LightSource lightSource, float minimalEnergy)
    {        
        this.lightsToToggle = lightsToToggle;
        this.lightsEnabled = defaultLightStatus;
        this.lightSource = lightSource;                
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
        
        Debug.Log("Toggle lights");
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
    /// If true, the lights are enabled and the GameObject is visible
    /// </summary>
    public bool LightsEnabled
    {
        get { return this.lightsEnabled; }
    }
}
