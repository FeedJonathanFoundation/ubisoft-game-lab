using UnityEngine;

/// <summary>
/// Responsible for enabling and disabling the lights attached to the player
///
/// @author - Simon A Thompson
/// @author - Jonathan L.A
/// @inspiration - Roxanne Sirois / Tristan Mirza
/// @version - 1.0.0
///
/// </summary>
public class PlayerLightToggle
{
	private bool lightsEnabled;
    private GameObject lightsToToggle;
    private float timerLossOfLight;
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

    /// <summary>
    /// Toggles the lights on the player.
    /// </summary>
    public void ToggleLights()
    {
        foreach (Behaviour childComponent in this.lightsToToggle.GetComponentsInChildren<Light>())
        {
            childComponent.enabled = !this.lightsEnabled;
        }
        // Update the status of the lights
        this.lightsEnabled = !this.lightsEnabled;
    }

    /// <summary>
    /// Depletes the player's light source when the light is toggled on
    /// </summary>
    public void UpdateLossOfLight(float lossOfLightInterval, float energyCostLightToggle)
    {
        if (this.lightsEnabled)
        {
            this.timerLossOfLight += Time.deltaTime;
            if (this.timerLossOfLight > lossOfLightInterval)
            {
                this.lightSource.LightEnergy.Deplete(energyCostLightToggle);
                this.timerLossOfLight = 0;

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
    public bool LightsEnabled
    {
        get
        { 
            return this.lightsEnabled;
        }
    }
}
