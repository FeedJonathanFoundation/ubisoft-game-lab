using UnityEngine;
using System.Collections;

/// <summary>
/// A container for a collection of fishes. Used for caching and performance. 
/// </summary>
public class FishSchool : MonoBehaviour
{
    [Tooltip("The initial angle at which the fish will start swimming." +
             " (Angle in degrees, clockwise, relative to +y-axis)")]
    [SerializeField]
    private float defaultWanderAngle;    
    
    private AbstractFish[] fishes;

    void Awake()
    {
        fishes = GetComponentsInChildren<AbstractFish>();
        
        Initialize();
    }
    
    /// <summary>
    /// Should be called when the fish are initially spawned. Sets the fishes' default
    /// wander angle to the one specified in "defaultWanderAngle:float".
    /// </summary>
    public void Initialize()
    {
        for (int i = 0; i < fishes.Length; i++)
        {
            // Set each fish's default wander angle so that they swim in the same direction
            fishes[i].DefaultWanderAngle = defaultWanderAngle;
        }
    }

    /// <summary>
    /// Returns the fishes contained in the school
    /// </summary>
    public AbstractFish[] Fishes
    {
        get { return fishes; }
    }
    
    /// <summary>
    /// The initial angle at which the fish will start swimming. (Angle in degrees, clockwise, relative to +y-axis)
    /// </summary>
    public float DefaultWanderAngle
    {
        get { return defaultWanderAngle; }
        set
        { 
            defaultWanderAngle = value;
            // Reset the default wander angle for each fish
            Initialize();
        }
    }  
}