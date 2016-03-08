using UnityEngine;
using System.Collections;

/// <summary>
/// Modifies the range of a Light component based on the amount of light energy
/// stored in a LightEnergy component.
/// </summary>
public class LightRangeModifier : LightEnergyListener
{
    /// <summary>
    /// The amount of light energy required for to make the light have a range of 1.0
    /// </summary>
    [Tooltip("The higher the value, the larger the range of light per unit of light energy")]
    public float lightToRangeRatio;

    public float maxIntensity;
    public float lightToIntensityRatio = 0.05f;

    // Cached components
    private new Light light;

    public override void OnLightChanged(float currentLight)
    {
        // Modifies the range of the attached light component based on the current amount of light energy
        float newRange = currentLight * lightToRangeRatio;
        Light.range = newRange;

        float newIntensity = maxIntensity * lightToIntensityRatio * currentLight;
        if (newIntensity <= maxIntensity)
        {
            Light.intensity = newIntensity;
        }
    }

    /** Cached light component */
    private Light Light
    {
        get
        {
            if (light == null)
                light = GetComponent<Light>();

            return light;
        }
    }
}
