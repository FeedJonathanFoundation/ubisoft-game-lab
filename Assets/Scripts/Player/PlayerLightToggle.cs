using UnityEngine;

/// <summary>
/// PlayerLightToggle class is responsible for behaviour related to player lights.
/// It toggles the lights on or off and depletes the energy from the Player
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
    private float timeToDeplete;
    private float minimalEnergyRestriction;
    private GameObject lightsToToggle;
    private LightSource lightSource;
    private Light closeLight;
   
    public PlayerLightToggle(GameObject lightsToToggle, bool defaultLightStatus, LightSource lightSource, float minimalEnergy)
    {
        this.lightsToToggle = lightsToToggle;
        this.lightsEnabled = defaultLightStatus;
        this.lightSource = lightSource;
        this.minimalEnergyRestriction = minimalEnergy;
        this.ToggleLights();
        
        // Cache very close light - needs to be refactored
        Transform lightTransform = lightsToToggle.transform.Find("VeryClose Light");
        if (lightTransform != null)
        {
            this.closeLight = lightTransform.GetComponent<Light>();
            this.closeLight.intensity = 0;    
        }                
    }

    /// <summary>
    /// Changes the status of Light component attached to Player
    /// </summary>
    public void ToggleLights()
    {
        foreach (Behaviour childComponent in this.lightsToToggle.GetComponentsInChildren<Light>())
        {
            // Skip VeryClose Light                                     
            Light light = childComponent.GetComponent<Light>();
            if (light != null && light.name != "VeryClose Light")
            {
                childComponent.enabled = !this.lightsEnabled;
            }
        }
        // Update the status of the lights
        this.lightsEnabled = !this.lightsEnabled;
    }

    /// <summary>
    /// Gradually depletes Player's energy when lights are turned on
    /// </summary>
    /// <param name="timeToDeplete">Time it takes to deplete a unit of energy when light toggle is on</param>
    /// <param name="energyCost">Energy cost of having Player lights turned on</param>
    public void DepleteLight(float timeToDeplete, float energyCost)
    {
        if (this.lightsEnabled)
        {
            this.timeToDeplete += Time.deltaTime;
            if (this.timeToDeplete > timeToDeplete)
            {
                this.lightSource.LightEnergy.Deplete(energyCost);
                this.timeToDeplete = 0;

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

    public Light ProbeLight
    {
        get { return this.closeLight; }
        set { this.closeLight = value; }
    }
}
