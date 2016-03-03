using UnityEngine;
using System.Collections;

/// <summary>
/// Changes the GameObject's emissive colour based on its current amount of light energy.
/// </summary>
public class LightEmissiveColourModifier : LightEnergyListener
{

    [Tooltip("The colour used when the light source is at <= minLight")]
    public Color minColour;
    [Tooltip("The colour used when the light source is at >= maxLight")]
    public Color maxColour;

    [Tooltip("The max amount of light needed to use the minColour")]
    public float minLight;
    [Tooltip("The minimum amount of light needed to use the maxColour")]
    public float maxLight;
    
    [Tooltip("The higher the value, the faster the colour changes.")]
    public float changeSpeed;

    [Tooltip("If true, the children's material colours are also changed")]
    public bool updateChildren;

    // The MeshRenderer attached to this GameObject
    private MeshRenderer myRenderer;
    // The MeshRenderer attached to this GameObject's children
    private MeshRenderer[] renderers;

    public override void OnLightChanged(float currentLight)
    {
        // Calculates the percentage of colour to use between [minColour,maxColour]
        float percent = (currentLight - minLight) / (maxLight-minLight);
        
        // Stop any colour lerping before changing the colour again.
        StopAllCoroutines();
        
        if (MyRenderer != null)
        {
            Color targetColour = GetTargetColour(percent);
            StartCoroutine(UpdateEmissiveColour(MyRenderer.material, targetColour));
        }
    }
    
    /// <summary>
    /// Gradually changes the material's emissive colour to the targetColour.
    /// The higher the changeRate, the faster the change occurs.
    /// </summary>
    private IEnumerator UpdateEmissiveColour(Material material, Color targetColour)
    {
        while (true)
        {
            // Gradually lerp to the target colour
            Color currentColour = material.GetColor("_EmissionColor");
            Color newColour = Color.Lerp(currentColour, targetColour, changeSpeed * Time.deltaTime);
            
            material.SetColor("_EmissionColor", newColour);
            
            yield return null;
        }
    }
    
    /// <summary>
    /// Returns a colour between [minColour,maxColour] based
    /// on the given percentage:
    /// percent = 0 ---> colour = minColour
    /// percent = 1 ---> colour = maxColour
    /// </summary>
    private Color GetTargetColour(float percent)
    {
        Color targetColour = Color.Lerp(minColour, maxColour, percent);
        
        return targetColour;
    }

    // The MeshRenderer attached to this GameObject
    private MeshRenderer MyRenderer
    {
        get
        {
            if (myRenderer == null) { myRenderer = GetComponent<MeshRenderer>(); }
            return myRenderer;
        }
    }

    // The MeshRenderer attached to this GameObject's children
    private MeshRenderer[] Renderers
    {
        get
        {
            if (renderers == null) { renderers = GetComponentsInChildren<MeshRenderer>(); }
            return renderers;
        }
    }
}