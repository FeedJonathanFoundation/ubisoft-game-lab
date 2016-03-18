using UnityEngine;
using System.Collections;

/// <summary>
/// Helper methods for Materials in Unity
///
/// @author - Alex I.
/// @version - 1.0.0
/// </summary>
public class MaterialExtensions
{   
   private GameObject obj;
   private Color color;
   private float time;
    public void SetEmissiveLight(GameObject obj, Color color, float time)
    {         
        this.obj = obj;
        this.color = color;
        this.time = time;
    }
    
    /// <summary>
    /// Smoothly changes current color of material to target color
    /// </summary>
    /// <param name="material">target material</param>
    /// <param name="newColor">target color</param>
    /// <param name="duration">time in seconds</param>
    /// <returns></returns>
    public IEnumerator LerpColor(Material material, Color newColor, float duration)
    {        
        Color startColor = material.GetColor("_EmissionColor");        
        float smoothness = 0.002f; // Smaller values are smoother. Really it's the time between updates.
        float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
        float increment = smoothness / duration; //The amount of change to apply.
        while (progress < 1)
        {
            material.SetColor("_EmissionColor", Color.Lerp(startColor, newColor, progress));
            progress += increment;
            yield return new WaitForSeconds(increment);
        }
    }
    
    /// <summary>
    /// Changes the color of the given material 
    /// </summary>
    /// <param name="material"></param>
    /// <param name="color"></param>
    public void ChangeColor(Material material, Color color)
    {
        material.SetColor("_EmissionColor", color);
    }
       
}