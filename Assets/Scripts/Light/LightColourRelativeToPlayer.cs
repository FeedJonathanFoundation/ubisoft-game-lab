using UnityEngine;
using System.Collections;

/// <summary>
/// Changes the light's colour based on whether it has more or less energy than the player
/// </summary>
public class LightColourRelativeToPlayer : ColourRelativeToPlayer
{
    // The Light attached to this GameObject
    private Light myLight;
    // The Lights attached to this GameObject's children
    private Light[] childLights;

    /// <summary>
    /// Called when the player or this light source gains/loses light. Updates the light source's colour
    /// based on whether it has more or less light than the player.
    /// <param name="energy"> The energy for the light source that gained/lost light </param>
    /// </summary>
    protected override void OnLightChanged(float energy)
    {
        Color targetColour = GetTargetColour();
        
        // Stop any colour lerping before changing the colour again.
        StopAllCoroutines();
        
        if (MyLight != null)
        {
            StartCoroutine(UpdateColour(MyLight, targetColour));
        }
    }
    
    /// <summary>
    /// Gradually changes the light's colour to the targetColour.
    /// The higher the changeRate, the faster the change occurs.
    /// </summary>
    private IEnumerator UpdateColour(Light light, Color targetColour)
    {
        while (true)
        {
            // Gradually lerp to the target colour
            Color currentColour = light.color;
            Color newColour = Color.Lerp(currentColour, targetColour, changeSpeed * Time.deltaTime);
            
            light.color = newColour;
            
            yield return null;
        }
    }

    // The Light attached to this GameObject
    private Light MyLight
    {
        get
        {
            if (myLight == null) { myLight = GetComponent<Light>(); }
            return myLight;
        }
    }

    // The Lights attached to this GameObject's children
    private Light[] ChildLights
    {
        get
        {
            if (childLights == null) { childLights = GetComponentsInChildren<Light>(); }
            return childLights;
        }
    }
}