using UnityEngine;
using System.Collections;

public class VignetteModifier : MonoBehaviour
{
    [Tooltip("The higher the value, the larger the intensity of the light illuminating the environment.")]
    public float maxIntensity;
    [Tooltip("Used to normalize the intensity.")]
    public float intensityRatio = 0.05f;

    public Transform target;

    // private Vignette vignette;

    void Awake()
    {
        // vignette = Camera.main.gameObject.GetComponent<Vignette>();
        DontDestroyOnLoad(this.gameObject); 
    }
    
    void Update()
    {
        LightEnergy lightEnergy = target.gameObject.GetComponent<LightEnergy>();
        if (lightEnergy != null)
        {
            float currentLight = lightEnergy.CurrentEnergy;

            // Modifies the intensity of the vignette on the camera based on the current amount of light energy
            
            // TO DO: opposition (make intensity go down as current Light goes up)
            float newIntensity = maxIntensity * currentLight * intensityRatio;
            if (newIntensity <= maxIntensity)
            {
                // Vignette.intensity = newIntensity;
            }
        }
        
    }
    
}
