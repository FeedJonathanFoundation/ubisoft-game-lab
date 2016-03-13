using System;
using UnityEngine;

/// <summary>
/// If attached to a GameObject, this GameObject can absorb light
/// from other GameObjects with a LightEnergy component 
///
/// @author - Jonathan L.A
/// @author - Alex I.
/// @version - 1.0.0
///
/// </summary>
public class LightSource : MonoBehaviour
{
    [Header("Light Source")]
    [SerializeField]
    [Tooltip("If true, this GameObject can absorb other GameObjects with a LightSource component")]
    private bool canAbsorb = false;
    
    [SerializeField]
    [Tooltip("If true, the player can always absorb this GameObject, even if it has higher light.")]
    private bool playerWillAlwaysAbsorb = false;

    [SerializeField]
    [Tooltip("The higher the value, the faster light is absorbed from other light sources")]
    private float absorptionRate = 15;

    [SerializeField]
    [Tooltip("The default amount of energy this light source holds")]
    private float defaultEnergy = 10;

    [SerializeField]
    [Tooltip("If true, the light source has infinite energy")]
    private bool infiniteEnergy = false;

    [SerializeField]
    [Tooltip("Detects absorbable lights that are in contact with this light source" )]
    private Neighbourhood absorbableLightDetector;
    
    private LightEnergy lightEnergy;
    private string lightSourceId;

    protected virtual void Awake() 
    {
        // Generates a unique id prefixed by object name
        this.lightSourceId = GenerateID(this.name);
    }
        
    protected virtual void Update()
    {
        // Cycle through each absorbable light source being touched by this GameObject
        for (int i = 0; i < absorbableLightDetector.NeighbourCount; i++)
        {
            GameObject absorbableLight = absorbableLightDetector.GetNeighbour(i);            
            if (absorbableLight == null) { continue; }
            
            LightSource otherLightSource = absorbableLight.GetComponentInParent<LightSource>();
            if (otherLightSource == null) { continue; }
            
            // If this GameObject can absorb the touched light source, 
            // Transfer light energy from the other light source to this one
            if (CanAbsorb(otherLightSource))
            {
                LightEnergy lightEnergyToAbsorb = otherLightSource.LightEnergy;
                float lightToAbsorb = absorptionRate * Time.deltaTime; 
                float lightAbsorbed = lightEnergyToAbsorb.Deplete(lightToAbsorb);
                lightEnergy.Add(lightAbsorbed);
            }
        }
    }
    
    /// <summary>
    /// Called the instant the light depletes to zero. 
    /// Called from the LightEnergy.LightDepleted event.
    /// </summary>
    protected virtual void OnLightDepleted() {}
   
    public virtual void OnEnable()
    {
        this.LightEnergy.LightDepleted += OnLightDepleted;
    }
    
    public virtual void OnDisable()
    {
        this.LightEnergy.LightDepleted -= OnLightDepleted;
    }
        
    /// <summary>
    /// Returns true if this LightSource can absorb the given light source.
    /// Calculated based on which LightSource has more energy
    /// </summary>
    private bool CanAbsorb(LightSource otherLightSource)
    {
        if (canAbsorb && LightEnergy.CurrentEnergy > otherLightSource.LightEnergy.CurrentEnergy)
        {
            return true;
        }        
        else if (canAbsorb && !otherLightSource.canAbsorb)
        {
            return true;
        }        
        else if (this is Player && otherLightSource.playerWillAlwaysAbsorb)
        {
            // The player can always absorb a light source with LightSource.playerWillAlwaysAbsorb set to true
            return true;
        }
        else
        {
            return false;
        }
    }
    
    private string GenerateID(string objectName)
    {
        return Guid.NewGuid().ToString();
        // if (objectName != null)
        // {
        //     return string.Format("{0}_{1:N}", objectName, Guid.NewGuid());    
        // }
        // else 
        // {
        //     return Guid.NewGuid().ToString();
        // }        
    }
     
     
    public LightEnergy LightEnergy
    {
        get 
        { 
            if (lightEnergy == null)
            {
                lightEnergy = new LightEnergy(defaultEnergy, gameObject, infiniteEnergy);    
            }                        
            return lightEnergy; 
        }
        set { this.lightEnergy = value; }
    }
    
    public float DefaultEnergy
    {
        get { return this.defaultEnergy; }
        set { this.defaultEnergy = value; }
    }
    
    public string LightSourceID
    {
        get { return this.lightSourceId; }        
    }
    
    public bool InfiniteEnergy
    {
        get { return this.infiniteEnergy; }
        set { this.infiniteEnergy = value; }
    }
}