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
    private bool canAbsorb = false;
    
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
    public Neighbourhood absorbableLightDetector;

    private new Transform transform;
    private new Rigidbody rigidbody;
    // DO NOT ACCESS DIRECTLY. Use LightEnergy property instead.
    private LightEnergy lightEnergy;
    private string lightSourceId;
    
    /** Raised when the light source consumes some light */
    public delegate void ConsumedLightSourceHandler(LightSource consumedLightSource);
    public event ConsumedLightSourceHandler ConsumedLightSource = delegate {};


    /// <summary>
    /// Awake is called when the script instance is being loaded
    /// <see cref="Unity Documentation">
    /// </summary>
    protected virtual void Awake() 
    {
        // Generates a unique id prefixed by object name
        this.lightSourceId = GenerateID(this.name);
        this.rigidbody = GetComponent<Rigidbody>();        
    }
    
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
            // Transfer light energy from the other light source to this one
            if (CanAbsorb(otherLightSource))
            {
                
                if (this is Player)
                {                    
                    if (otherLightSource.CompareTag("Pickup"))
                    {
                        AkSoundEngine.PostEvent("Light_Orb_Pickup", this.gameObject);
                    }
                    else if (otherLightSource.CompareTag("Checkpoint"))
                    {
                        AkSoundEngine.PostEvent("Checkpoint", this.gameObject);
                    }
                    {                                                                             
                        AkSoundEngine.PostEvent("Eat", this.gameObject);
                    }                    
                }
                LightEnergy lightEnergyToAbsorb = otherLightSource.LightEnergy;
                                        
                // Calculate the amount of light to absorb from the other light source
                float lightToAbsorb = absorptionRate * Time.deltaTime;
                
                // If the player was hit
                if (otherLightSource is Player)
                {
                    if (this is AbstractFish)
                    {
                        // Absorb a certain amount of light from the player to the fish
                        AbstractFish fish = (AbstractFish)this;
                        lightToAbsorb = fish.damageInflicted;
                    }
                    
                    Debug.Log("Absorb " + lightToAbsorb + " from player");
                     
                    // Knockback the player away from the enemy fish
                    otherLightSource.Knockback(this);
                }
                
                // Transfer light energy from the other light source to this one
                float lightAbsorbed = lightEnergyToAbsorb.Deplete(lightToAbsorb);
                lightEnergy.Add(lightAbsorbed);
                
                // Inform subscribers that this light source consumed another light source.
                ConsumedLightSource(otherLightSource);
            }
        }
    }
     
    protected virtual void ChangeColor(Color color, bool isSmooth, float seconds)
    {
        // Implemented in children        
    } 
     
     
    /// <summary>
    /// Returns true if this LightSource can absorb the given LightSource
    /// Calculated by comparing the amount of energy in LightEnergy property of LightSources 
    /// </summary>
    private bool CanAbsorb(LightSource otherLightSource)
    {
        if (!otherLightSource.CanBeAbsorbed()) { return false; }
        
        // If this light source has more energy than the other one, return true. This light source can absorb the given argument.
        if (canAbsorb && LightEnergy.CurrentEnergy > otherLightSource.LightEnergy.CurrentEnergy) { return true; }
                       
        if (canAbsorb && !otherLightSource.canAbsorb) { return true; }        
        
        // The player can always absorb a light source with LightSource.playerWillAlwaysAbsorb set to true
        if (this is Player && otherLightSource.playerWillAlwaysAbsorb) { return true; }

        return false;        
    }

    
    /// <summary>
    /// Returns true if this light source be absorbed
    /// A light source can always be absorbed by default 
    /// </summary>
    public virtual bool CanBeAbsorbed()
    {        
        return true;
    }
    
    /// <summary>
    /// Applies a knockback force going away from the enemy light source
    /// </summary>
    public virtual void Knockback(LightSource enemyLightSource)
    {
    }
    
    /// <summary>
    /// Called the instant the light depletes to zero 
    /// from the LightEnergy.LightDepleted event
    /// 
    /// Implemented in child classes
    /// </summary>
    protected virtual void OnLightDepleted() {}
          
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
    
     /// <summary>
    /// Cached transform component
    /// </summary>
    public Transform Transform
    {
        get 
        {
            if (this.transform == null) { transform = GetComponent<Transform>(); }
            return transform;
        }
        set { this.transform = value; }
    }
    
    /// <summary>
    /// Cached rigidbody component
    /// </summary>
    public Rigidbody Rigidbody
    {
        get 
        {
            if (this.rigidbody == null) { rigidbody = GetComponent<Rigidbody>(); }
            return rigidbody;
        }
        set { this.rigidbody = value; }
    }
      
    /// <summary>
    /// The LightEnergy component accessor controlling this object's amount of energy
    /// </summary>
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