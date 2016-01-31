using UnityEngine;
using System.Collections;

/// <summary>
/// Modifies the GameObject based on its current amount of light energy
/// </summary>
public abstract class LightAttribute : MonoBehaviour
{   
    [Tooltip("The LightEnergy component which modifies the desired attribute. If none " +
     "specified, the LightEnergy attached to this GameObject is used.")] 
    public LightEnergy lightEnergyOverride;
    
    void OnEnable()
    {
        LightEnergy lightEnergy;
        
        // Choose either the override (if assigned in the Inspector) or the component
        // attached to this GameObject.
        if (lightEnergyOverride) { lightEnergy = lightEnergyOverride; }
        else { lightEnergy = GetComponent<LightEnergy>(); }
        
        if (lightEnergy)
        {
            // Call OnLightChanged() whenever the GameObject's amount of light energy changes.  
            lightEnergy.LightChanged += OnLightChanged;
        }
    }
    
    void OnDisable()
    {
        LightEnergy lightEnergy;
        
        // Choose the LightEnergy instance that affects the desired attribute
        if (lightEnergyOverride) { lightEnergy = lightEnergyOverride; }
        else { lightEnergy = GetComponent<LightEnergy>(); }
        
        if (lightEnergy)
        {
            // Unsubscribe from events to avoid errors
            lightEnergy.LightChanged -= OnLightChanged;
        }
    }
    
    /// <summary>
    /// Called by LightEnergy.cs when the amount of light energy owned by the 
    /// GameObject changes.
    /// </summary>
    public abstract void OnLightChanged(float currentLight);
    
}