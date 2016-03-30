using UnityEngine;

/// <summary>
/// Represents the amount of energy maintained by a LightSource
///
/// @author - Jonathan L.A
/// @author - Alex I.
/// @version - 1.0.0
///
/// </summary>
public class LightEnergy
{
    public delegate void LightChangedHandler(float currentLight);
    public delegate void LightDepletedHandler();
    
    // Called when the GameObject's amount of stored light changes
    public event LightChangedHandler LightChanged = delegate {};
        
    // Called when all light was depleted from the light source.
    public event LightDepletedHandler LightDepleted = delegate {};
    
    private float currentEnergy;
    private float defaultEnergy;
    private GameObject gameObject;    
    private bool hasInfiniteEnergy = false;

    /// <summary>
    /// Light energy constructor initializes
    /// current energy to default energy for the given game object
    /// </summary>
    public LightEnergy(float defaultEnergy, GameObject gameObject, bool debugInfiniteLight)
    {
        this.defaultEnergy = defaultEnergy;
        this.currentEnergy = defaultEnergy;
        this.gameObject = gameObject;
        this.hasInfiniteEnergy = debugInfiniteLight;
        LightChanged(this.currentEnergy);
    }

    /// <summary>
    /// Adds the specified amount of light energy and
    /// informs all interested subscribers that the amount of energy
    /// possessed by the game object has changed
    /// </summary>
    public void Add(float lightEnergy)
    {
        if (this.currentEnergy + lightEnergy > this.defaultEnergy)
        {
            this.currentEnergy = this.defaultEnergy;    
        }
        else 
        {
            this.currentEnergy += lightEnergy;    
        }        
        LightChanged(this.currentEnergy);
        //Debug.Log("LIGHT CHANGED on " + gameObject.name);
    }

    /// <summary>
    /// Depletes the desired amount of light energy from the component
    /// 
    /// Returns the amount of energy removed.
    /// </summary>
    /// <param name="energyToRemove">amount of energy to remove</param>
    /// <returns>float - amount of energy removed</returns>
    public float Deplete(float energyToRemove)
    {
        if (hasInfiniteEnergy) 
        { 
            LightChanged(this.currentEnergy);
            return energyToRemove; 
        }
        
        float energyRemoved = energyToRemove;
        
        if (energyToRemove > this.currentEnergy)
        {
            energyRemoved = this.currentEnergy;
        }

        // Remove the desired amount of light energy and clamp it to zero
        this.currentEnergy -= energyToRemove;
        this.currentEnergy = Mathf.Max(0, this.currentEnergy);

        // Notify subscribers that the amount of energy in this light has changed
        LightChanged(this.currentEnergy);
        //Debug.Log("LIGHT CHANGED on " + gameObject.name);
        
        // If all light was depleted from this light source
        if (this.currentEnergy <= 0)
        {
            // Notify subscribers that the amount of energy in this light is 0
            LightDepleted();
            if (this.gameObject.tag == "Player")
            {
                Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
                if (rigidbody) { rigidbody.drag = 10; }
            }
        }
        
        return energyRemoved;
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
