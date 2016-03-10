using UnityEngine;
using System.Collections;

/// <summary>
/// ???????
///
/// @author - Jonathan L.A
/// @author - Alex I.
/// @version - 1.0.0
///
/// </summary>
public class LightEnergy
{
    public delegate void LightChangedHandler(float currentLight);
    
    // Called when the GameObject's amount of stored light changes
    public event LightChangedHandler LightChanged = delegate {};

    public delegate void LightDepletedHandler();
    
    // Called when all light was depleted from the light source.
    public event LightDepletedHandler LightDepleted = delegate {};
    
    private float currentEnergy;

    private GameObject gameObject;
    
    private bool debugInfiniteLight = false;

    /// <summary>
    /// Light energy constructor initializes
    /// current energy to default energy for the given game object
    /// </summary>
    public LightEnergy(float defaultEnergy, GameObject gameObject, bool debugInfiniteLight)
    {
        this.currentEnergy = defaultEnergy;
        this.gameObject = gameObject;
        this.debugInfiniteLight = debugInfiniteLight;
        LightChanged(this.currentEnergy);
    }

    /// <summary>
    /// Adds the specified amount of light energy and
    /// informs all all interested subscribers that the amount of energy
    /// possessed by the game object has changed
    /// </summary>
    public void Add(float lightEnergy)
    {
        this.currentEnergy += lightEnergy;
        LightChanged(this.currentEnergy);
    }

    /// <summary>
    /// Depletes the desired amount of light energy from this component
    /// if there is enough existing energy to remove
    /// Returns the actual amount of energy removed.
    /// </summary>
    public float Deplete(float lightToRemove)
    {
        if (debugInfiniteLight)
        {
            return lightToRemove;
        }

        float actualLightRemoved = lightToRemove;
        if (lightToRemove > this.currentEnergy)
        {
            actualLightRemoved = this.currentEnergy;
        }

        // Remove the desired amount of light energy and clamp it to zero
        this.currentEnergy -= lightToRemove;
        this.currentEnergy = Mathf.Max(0, this.currentEnergy);

        // Notify subscribers that the amount of energy in this light has changed
        LightChanged(this.currentEnergy);
        
        // If all light was depleted from this light source
        if (this.currentEnergy <= 0)
        {
            LightDepleted();
            if (this.gameObject.tag == "Player")
            {
                Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
                if (rigidbody)
                {
                    rigidbody.drag = 10;                
                }
            }
        }
        return actualLightRemoved;
    }

    /// <summary>
    /// The current amount of energy held by the GameObject
    /// </summary>
    public float CurrentEnergy
    {
        get { return currentEnergy; }
        set { currentEnergy = value; }
    }
    
}