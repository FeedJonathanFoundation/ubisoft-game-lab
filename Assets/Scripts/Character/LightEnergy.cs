using UnityEngine;
using System.Collections;

public class LightEnergy : MonoBehaviour
{
    /// <summary>
    /// The current amount of energy held by the GameObject
    /// </summary>
    private float currentEnergy;
    
    /// <summary>
    /// Adds the given amount of light energy
    /// </summary>
    public void Add(float lightEnergy)
    {
        currentEnergy += lightEnergy;
    }
    
    /// <summary>
    /// The current amount of energy held by the GameObject
    /// </summary>
    public float CurrentEnergy
    {
        get { return currentEnergy; }
    }
}