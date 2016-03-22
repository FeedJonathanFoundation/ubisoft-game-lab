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
    
    [Tooltip("The speed at which the light's range changes when the light is turned on")]
    public float rangeChangeSpeed = 0.5f;
    [Tooltip("The speed at which the light's intensity changes when the light is turned on")]
    public float intensityChangeSpeed = 0.5f;
    private float percentRange;
    private float targetRange;
    private float targetIntensity;
    private bool lightJustEnabled;   // True if this light has been enabled this frame

    // Cached components
    private new Light light;
    
    void OnEnable()
    {
        lightJustEnabled = true;
    }
    
    void OnDisable()
    {
        Light.range = 0;
        Light.intensity = 0;
        
        StopAllCoroutines();
        
        //Debug.Log("LIGHT DISABLED");
    }
    
    void Update()
    {
        Light.range = Mathf.Lerp(Light.range, targetRange, rangeChangeSpeed * Time.deltaTime);
        Light.intensity = Mathf.Lerp(Light.intensity, targetIntensity, intensityChangeSpeed * Time.deltaTime);
    }

    public override void OnLightChanged(float currentLight)
    {
        // If true, the light was just enabled this frame
        bool lightJustEnabled = false;
        
        // Determine if the light was disabled last frame
        //if ((Time.time - timeLastActive) <= Time.deltaTime)
        //{
        //    lightJustEnabled = true;
        //}
        
        // If the light was just enabled
        //if (lightJustEnabled)
        //{
            // Stop all range/intensity lerping
            //StopAllCoroutines();
        //}
               
        // Modifies the range of the attached light component based on the current amount of light energy
        float newRange = currentLight * lightToRangeRatio * percentRange;
        targetRange = newRange;
        //StartCoroutine(SetRange(newRange,rangeChangeSpeed));

        float newIntensity = maxIntensity * lightToIntensityRatio * currentLight * percentRange;
        if (newIntensity <= maxIntensity)
        {
            targetIntensity = newIntensity;
            //if (lightJustEnabled)
            //{
               // StartCoroutine(SetIntensity(newIntensity,intensityChangeSpeed));
            //}
        }
        
        lightJustEnabled = false;
    }
    
    private IEnumerator SetRange(float range, float speed)
    {
        while (Mathf.Abs(Light.range - range) > 0.01f)
        {
            Light.range = Mathf.Lerp(Light.range, targetRange, speed * Time.deltaTime);
            yield return null;
        }
    }
    
    private IEnumerator SetIntensity(float intensity, float speed)
    {
        while (Mathf.Abs(Light.intensity - intensity) > 0.01f)
        {
            Light.intensity = Mathf.Lerp(Light.intensity, targetIntensity, speed * Time.deltaTime);
            yield return null;
        }
    }
    
    /// <summary>
    /// The percentage of range of the lights. 0 = zero range. 1 = normal range
    /// </summary>
    public float PercentRange
    {
        get { return percentRange; }
        set 
        { 
            percentRange = value; 
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
