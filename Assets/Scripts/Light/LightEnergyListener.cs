using UnityEngine;
using System.Collections;

/// <summary>
/// Modifies the GameObject based on its current amount of light energy
/// </summary>
public abstract class LightEnergyListener : MonoBehaviour
{
    [Tooltip("The LightEnergy component which modifies the desired attribute. If none " +
     "specified, the LightEnergy attached to this GameObject is used.")]
    public LightEnergy lightEnergyOverride;

    protected LightSource lightSource;  // The LightSource being listened to
    private LightEnergy lightEnergy;

    public virtual void Start()
    {
        lightSource = GetComponentInParent<LightSource>();
        
        // Choose either the override (if assigned in the Inspector) or the component
        // attached to this GameObject.
        if (lightEnergyOverride != null)
        {
            this.lightEnergy = lightEnergyOverride;
        }
        else if (lightSource)
        {
            this.lightEnergy = lightSource.LightEnergy;
        }
        else
        {
            this.lightEnergy = null;
        }

        Subscribe();

        // Initialize the attribute to the light's initial energy
        OnLightChanged(lightEnergy.CurrentEnergy);
    }

    public void Subscribe()
    {
        if (this.lightEnergy != null)
        {
            // Call OnLightChanged() whenever the GameObject's amount of light energy changes.
            this.lightEnergy.LightChanged += OnLightChanged;
            //this.lightEnergy.LightDepleted += OnLightDepleted;
        }
    }

    public void Unsubscribe()
    {
        if (this.lightEnergy != null)
        {
            // Unsubscribe from events to avoid errors
            this.lightEnergy.LightChanged -= OnLightChanged;
            //this.lightEnergy.LightDepleted -= OnLightDepleted;
        }
    }

    public void OnLightDepleted()
    {
        Unsubscribe();
    }

    /// <summary>
    /// Called by LightEnergy.cs when the amount of light energy owned by the
    /// GameObject changes.
    /// </summary>
    public abstract void OnLightChanged(float currentLight);

}