using UnityEngine;
using System.Collections;

/// <summary>
/// A container for a collection of fishes. Used for caching and performance. 
/// </summary>
public class FishSchool : MonoBehaviour
{
    private AbstractFish[] fishes;

    void Awake()
    {
        fishes = GetComponentsInChildren<AbstractFish>();
    }

    /// <summary>
    /// Returns the fishes contained in the school
    /// </summary>
    public AbstractFish[] Fishes
    {
        get { return fishes; }
    }
}