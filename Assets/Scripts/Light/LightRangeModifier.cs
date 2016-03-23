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

    // Cached components
    private new Light light;
    
    public override void Start()
    {
        base.Start();
        
        TurnOffLightImmediate();
    }
    
    public virtual void OnEnable()
    {
        //base.OnEnable();
    }
    
    public virtual void OnDisable()
    {
        //base.OnDisable();
        
        TurnOffLightImmediate();
        
        StopAllCoroutines();
        
        //Debug.Log("LIGHT DISABLED");
    }
    
    void Update()
    {
        if (!ActiveLights) { TurnOffLight(); }

        Light.range = Mathf.Lerp(Light.range, targetRange, rangeChangeSpeed * Time.deltaTime);
        Light.intensity = Mathf.Lerp(Light.intensity, targetIntensity, intensityChangeSpeed * Time.deltaTime);
    }

    public override void OnLightChanged(float currentLight)
    {
        //ebug.Log("Current amount of light: " + currentLight);
               
        // Modifies the range of the attached light component based on the current amount of light energy
        float newRange = currentLight * lightToRangeRatio * percentRange;
        targetRange = newRange;
        //StartCoroutine(SetRange(newRange,rangeChangeSpeed));

        float newIntensity = maxIntensity * lightToIntensityRatio * currentLight * percentRange;
        // Cap the light's intensity
        if (newIntensity >= maxIntensity) { newIntensity = maxIntensity; }
        targetIntensity = newIntensity;
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
    
    public void TurnOffLight()
    {
        targetRange = 0;
        targetIntensity = 0;
    }
    
    private void TurnOffLightImmediate()
    {
        Light.range = 0;
        Light.intensity = 0;
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
    
    /// <summary>
    /// If false, the light's intensity is set to zero. Otherwise,
    /// the light behaves normally
    /// </summary>
    public bool ActiveLights
    {
        get; set;
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
