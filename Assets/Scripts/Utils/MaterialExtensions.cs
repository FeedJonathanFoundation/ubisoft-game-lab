using UnityEngine;
using System.Collections;

public static class MaterialExtensions
{
   
    public static void SetEmissiveLight(GameObject obj, Color color, float time)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        foreach (Material mat in renderer.materials)
        {
            Color startColor = mat.GetColor("_EmissionColor");
            Color endColor = color;
                 
            var i = 0.0f;
            var rate = 1.0f / 1.0f;
            
            while (i < 100) {
                i += rate;                
                mat.SetColor("_EmissionColor", Color.Lerp(startColor, endColor, i));                
            }                
        }

    }
    
}