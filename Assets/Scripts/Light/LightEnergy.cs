using UnityEngine;
using System.Collections;

public class LightEnergy
{
    /// <summary>
    /// The current amount of energy held by the GameObject
    /// </summary>
    private float currentEnergy;

    private GameObject gameObject;
    // [Tooltip("The default amount of energy this light source holds")]
    // public float defaultEnergy;

    public delegate void LightChangedHandler(float currentLight);
    /** Called when the GameObject's amount of stored light changes */
    public event LightChangedHandler LightChanged = delegate {};

    public delegate void LightDepletedHandler();
    /** Called when all light was depleted from the light source. */
    public event LightDepletedHandler LightDepleted = delegate {};

    public LightEnergy(float defaultEnergy, GameObject gameObject)
    {
        this.currentEnergy = defaultEnergy;
        this.gameObject = gameObject;
        LightChanged(this.currentEnergy);
    }
    
    // void Start()
    // {
    //     // Set the current amount of energy to default
    //     currentEnergy = defaultEnergy;
    //     LightChanged(currentEnergy);
    // }

    /// <summary>
    /// Adds the given amount of light energy
    /// </summary>
    public void Add(float lightEnergy)
    {
        this.currentEnergy += lightEnergy;

        // Inform all interested subscribers that the amount of energy possessed by
        // GameObject has changed
        LightChanged(this.currentEnergy);
    }

    /// <summary>
    /// Depletes the desired amount of light energy from this component.
    /// Returns the actual amount of energy removed.
    /// </summary>
    public float Deplete(float lightToRemove)
    {
        // Stores the actual amount of light depleted from this energy source
        float actualLightRemoved = lightToRemove;

        // If there is more light to remove than there is actual energy
        if (lightToRemove > this.currentEnergy)
        {
            // Remove 'currentEnergy' amount of light from this component.
            actualLightRemoved = this.currentEnergy;
        }

        // Remove the desired amount of light energy and clamp it to zero
        this.currentEnergy -= lightToRemove;
        this.currentEnergy = Mathf.Max(0, this.currentEnergy);
        //Debug.Log("My name is : " + this.gameObject.name);
        //Debug.Log("Current energy: " + this.currentEnergy);
        // Notify subscribers that the amount of energy in this light has changed
        LightChanged(this.currentEnergy);

        // If all light was depleted from this light source
        if (this.currentEnergy <= 0)
        {
            LightDepleted();
            //Debug.Log("Light source depleted");
            // Destroy LightSource object and its children. TODO: Pooling
            UnityEngine.Object.Destroy(this.gameObject);
        }

        return actualLightRemoved;
    }

    /// <summary>
    /// The current amount of energy held by the GameObject
    /// </summary>
    public float CurrentEnergy
    {
        get { return currentEnergy; }
    }
}