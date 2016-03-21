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
      
    /// <summary>
    /// Smoothly changes current color of material to target color
    /// </summary>
    /// <param name="material">target material</param>
    /// <param name="newColor">target color</param>
    /// <param name="duration">time in seconds</param>
    /// <returns></returns>
    public IEnumerator ChangeColor(Material material, Color newColor, float seconds, float duration)
    {
        Color startColor = material.GetColor("_EmissionColor");
        float progress = 0;
        float increment = 0.0416f; //The amount of change to apply.
        while (progress < 1)
        {
            material.SetColor("_EmissionColor", Color.Lerp(startColor, newColor, progress));
            progress += increment;
            yield return new WaitForSeconds(seconds);
        }
    }

    /// <summary>
    /// Changes the color of the given material 
    /// </summary>
    /// <param name="material"></param>
    /// <param name="color"></param>
    public IEnumerator ChangeColor(Material material, Color color, float seconds)
    {
        Color currentColor = material.GetColor("_EmissionColor");
        material.SetColor("_EmissionColor", color);
        yield return new WaitForSeconds(seconds);

        // Reset color to initial one
        if (seconds > 0) { material.SetColor("_EmissionColor", currentColor); }
    }

    /// <summary>
    /// Current not used
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="alpha"></param>
    /// <param name="isSmooth"></param>
    /// <returns></returns>
    public IEnumerator SetAlpha(Material mat, float alpha, bool isSmooth)
    {
        Color color = mat.GetColor("_EmissionColor");
        Color newColor = color;

        newColor.a = alpha;
        mat.SetColor("_EmissionColor", newColor);

        if (isSmooth)
        {
            float progress = 0;
            float increment = 0.0416f; //The amount of change to apply.
            while (progress < 1)
            {
                mat.SetColor("_Color", Color.Lerp(color, newColor, progress));
                progress += increment;
                yield return null;
            }
        }
        else
        {
            mat.SetColor("_Color", newColor);
            yield return null;
        }


    }

}