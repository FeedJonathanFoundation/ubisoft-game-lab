using UnityEngine;
using System.Collections;

/// <summary>
/// Modifies the GameObject based on its current amount of light energy
/// </summary>
public abstract class LightAttribute : MonoBehaviour
{    
    void OnEnable()
    {
        LightEnergy lightEnergy = GetComponent<LightEnergy>();
        
        if(lightEnergy)
        {
            // Call OnLightChanged() whenever the GameObject's amount of light energy changes.  
            lightEnergy.LightChanged += OnLightChanged;
        }
    }
    
    void OnDisable()
    {
        LightEnergy lightEnergy = GetComponent<LightEnergy>();
        
        if(lightEnergy)
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