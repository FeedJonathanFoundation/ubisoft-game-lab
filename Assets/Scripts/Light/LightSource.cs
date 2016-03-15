using System;
using UnityEngine;

/// <summary>
/// Base class for all LigthSource objects
///
/// Provides GameObject with ability to hold LightEnergy 
/// and absorb LightEnery from other LightSources when two GameObjects collide.
///
/// All classes that wish to have properties of a LightSource need to extend this class.
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
    [Tooltip("If true, this GameObject will absorb other GameObjects with a LightSource component")]
    private bool willAbsorb = false;
    
    [SerializeField]
    [Tooltip("If true, the player will always absorb this GameObject, even if it has higher light")]
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

    /// <summary>
    /// Awake is called when the script instance is being loaded
    /// <see cref="Unity Documentation">
    /// </summary>
    protected virtual void Awake() 
    {
        // Generates a unique id prefixed by object name
        this.lightSourceId = GenerateID(this.name);
    }
        
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled
    /// <see cref="Unity Documentation">
    /// </summary>
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
            // transfer light energy from the other light source to this one
            if (WillAbsorb(otherLightSource))
            {
                LightEnergy lightEnergyToAbsorb = otherLightSource.LightEnergy;
                float lightToAbsorb = absorptionRate * Time.deltaTime; 
                float lightAbsorbed = lightEnergyToAbsorb.Deplete(lightToAbsorb);
                lightEnergy.Add(lightAbsorbed);
            }
        }
    }
    
    /// <summary>
    /// Called the instant the light depletes to zero 
    /// from the LightEnergy.LightDepleted event
    /// 
    /// Implemented in child classes
    /// </summary>
    protected virtual void OnLightDepleted() {}
   
    /// <summary>
    /// Subscribe to OnLightDepleted event
    /// </summary>
    public virtual void OnEnable()
    {
        this.LightEnergy.LightDepleted += OnLightDepleted;
    }
    
    /// <summary>
    /// Unsubscribe from OnLightDepleted event
    /// </summary>
    public virtual void OnDisable()
    {
        this.LightEnergy.LightDepleted -= OnLightDepleted;
    }
        
    /// <summary>
    /// Returns true if this LightSource can absorb the given LightSource
    /// Calculated by comparing the amount of energy in LightEnergy property of LightSources 
    /// </summary>
    private bool WillAbsorb(LightSource otherLightSource)
    {         
        // If current light source has more energy, it will absorb other object 
        if (this.LightEnergy.CurrentEnergy > otherLightSource.LightEnergy.CurrentEnergy)
        {
            return true;
        }        
        
        // LightSource with willAbsorb true will absorb all object with willAbsorb set to false
        if (willAbsorb && !otherLightSource.willAbsorb)
        {
            return true;
        }        
        
        // Player can always absorb a light source with LightSource.playerWillAlwaysAbsorb set to true
        if (this is Player && otherLightSource.playerWillAlwaysAbsorb)
        {            
            return true;
        }
       
       return false;       
    }
    
    /// <summary>
    /// Generates an unique ID for each LightSource
    /// </summary>
    /// <param name="objectName">name of the object used to prefix generated ID</param>
    /// <returns></returns>
    private string GenerateID(string objectName)
    {        
        if (objectName != null)
        {
            return string.Format("{0}_{1:N}", objectName, Guid.NewGuid());    
        }
        else 
        {
            return Guid.NewGuid().ToString();
        }        
    }
    
    
    /// PROPERTIES
      
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