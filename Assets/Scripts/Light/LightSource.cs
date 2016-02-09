using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// If attached to a GameObject, this GameObject can absorb light
/// from other GameObjects with a LightEnergy component
/// <summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(LightEnergy))]
public class LightSource : MonoBehaviour
{
    /// <summary>
    /// If true, this GameObject can absorb other GameObjects with a LightSource
    /// component
    /// </summary>
    public bool canAbsorb;
    
    /// <summary>
    /// The amount of light drained per second from other light sources.
    /// </summary>
    [Tooltip("The higher the value, the faster light is absorbed from other light sources")]
    public float absorptionRate = 5;
    
    /** The light sources this GameObject is touching. */
    private List<LightSource> lightsInContact = new List<LightSource>();
    
    // Cached components
    private LightEnergy lightEnergy;
    
    void Start()
    {
        // Cache components
        lightEnergy = GetComponent<LightEnergy>();
    }
    
    void FixedUpdate()
    {
        // Cycle through each light source being touched by this GameObject
        for(int i = 0; i < lightsInContact.Count; i++)
        {            
            LightSource otherLightSource = lightsInContact[i];
            
            if(otherLightSource == null)
            {
                // Remove the null light source from the list 
                lightsInContact.RemoveAt(i);
                continue;
            }
            
            // If this GameObject can absorb the touched light source
            if(CanAbsorb(otherLightSource))
            {
                LightEnergy lightEnergyToAbsorb = otherLightSource.LightEnergy;
                
                // Calculate the amount of light to absorb from the other light source
                float lightToAbsorb = absorptionRate * Time.fixedDeltaTime;
                
                // Transfer light energy from the other light source to this one
                float lightAbsorbed = lightEnergyToAbsorb.Deplete(lightToAbsorb);
                lightEnergy.Add(lightAbsorbed);
            }
            
        }
    }
    
    /// <summary>
    /// Returns true if this LightSource can absorb the given light source.
    /// </summary>
    public bool CanAbsorb(LightSource otherLightSource)
    {
        // If this light source has more energy than the other one, 
        // return true. This light source can absorb the given argument.
        if(LightEnergy.CurrentEnergy > otherLightSource.LightEnergy.CurrentEnergy)
            return true;
            
        // If this GameObject can absorb light sources but the given argument
        // can't, this GameObject can absorb the given light source
        if(canAbsorb && !otherLightSource.canAbsorb)
            return true; 
            
        return false;
    }
    
    public void OnTriggerEnter(Collider otherCollider)
    {
        //Debug.Log("ENTERRED light absorber: " + otherCollider.name); 
        
        LightSource otherLightSource = otherCollider.GetComponent<LightSource>();
        
        if(otherLightSource)
        {
            // Add the LightSource being touched to the list of lights in contact
            lightsInContact.Add(otherLightSource);
        }  
    }
    
    public void OnTriggerExit(Collider otherCollider)
    {
        //Debug.Log("LEFT light absorber: " + otherCollider.name); 
        
        LightSource otherLightSource = otherCollider.GetComponent<LightSource>();
        
        if(otherLightSource)
        {
            // Remove the LightSource from to the list of lights sources being touched
            lightsInContact.Remove(otherLightSource);
        }  
    }
    
    /// <summary>
    /// The LightEnergy component controlling this object's amount of energy
    /// </summary>
    public LightEnergy LightEnergy
    {
        get { return lightEnergy; }
    }
}