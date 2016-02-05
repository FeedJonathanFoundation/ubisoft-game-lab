using UnityEngine;
using System.Collections;

/// <summary>
/// Scales the GameObject based on its current amount of light energy
/// </summary>
public class LightScaleModifier : LightAttribute
{
    /// <summary>
    /// Determines the amount of energy points required to have a 1.0 scale.
    /// Transform.scale = currentLight * lightEnergyToScale;
    /// <summary>
    [Tooltip("The larger the value, the bigger the scale of the GameObject per " +
             "light unit")]
    public float lightToScaleRatio = 0.1f;
    
    /// <summary>
    /// Called by LightEnergy.cs when the amount of light energy owned by the 
    /// GameObject changes.
    /// </summary>
    public override void OnLightChanged(float currentLight)
    {
        //Debug.Log("Amount of light changed to: " + currentLight);   
        
        // Update the GameObject's scale based on its current amount of energy
        float newScale = currentLight * lightToScaleRatio;
        transform.localScale = new Vector3(newScale,newScale,newScale);
        
    }
    
}