using UnityEngine;
using System.Collections;

/// <summary>
/// Changes the GameObject's mass based on its current amount of light energy.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class LightMassModifier : LightAttribute
{
    /// <summary>
    /// Determines the amount of energy points required to have a 1.0 mass.
    /// (Rigidbody.mass = currentLight * lightToMassRatio);
    /// </summary>
    public float lightToMassRatio = 0.1f;

    /// <summary>
    /// The minimum amount of mass this GameObject can have
    /// </summary>
    [Tooltip("The minimum amount of mass this GameObject can have")]
    public float minMass;

    // Don't access directly. Access the Rigidbody property instead
    private new Rigidbody rigidbody;

    public override void OnLightChanged(float currentLight)
    {
        // Scales the rigidbody's mass based on its current amount of light
        Rigidbody.mass = currentLight * lightToMassRatio;

        // Clamp the mass to its minimum value
        Rigidbody.mass = Mathf.Max(minMass, Rigidbody.mass);

        // Debug.Log("Current mass: " + Rigidbody.mass);
    }

    /** Cached Rigidbody component. */
    private Rigidbody Rigidbody
    {
        get
        {
            if (rigidbody == null) { rigidbody = GetComponent<Rigidbody>(); }
            return rigidbody;
        }
    }
}