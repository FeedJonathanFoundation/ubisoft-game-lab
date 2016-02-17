using UnityEngine;
using System.Collections;

public class LightEnergy
{
    /// <summary>
    /// The current amount of energy held by the GameObject
    /// </summary>
    private float currentEnergy;

    private GameObject gameObject;
    
    private bool debugInfiniteLight = false;

    public delegate void LightChangedHandler(float currentLight);
    /** Called when the GameObject's amount of stored light changes */
    public event LightChangedHandler LightChanged = delegate {};

    public delegate void LightDepletedHandler();
    /** Called when all light was depleted from the light source. */
    public event LightDepletedHandler LightDepleted = delegate {};

    public LightEnergy(float defaultEnergy, GameObject gameObject, bool debugInfiniteLight)
    {
        this.currentEnergy = defaultEnergy;
        this.gameObject = gameObject;
        this.debugInfiniteLight = debugInfiniteLight;
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
        if (debugInfiniteLight)
            return lightToRemove;
        
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
            // Destroy LightSource object and its children, except if its Player.
            //TODO: Pooling
            if (this.gameObject.name != "Player")
            {
                this.gameObject.GetComponent<Rigidbody>().drag = 20;                
                //UnityEngine.Object.Destroy(this.gameObject);
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