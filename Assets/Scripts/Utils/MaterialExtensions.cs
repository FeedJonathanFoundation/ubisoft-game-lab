using UnityEngine;
using System.Collections;

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
        
        // MonoBehaviour m = new MonoBehaviour();
        // m.StartCoroutine(this.LerpColor());
        
        // Renderer renderer = obj.GetComponent<Renderer>();
        // foreach (Material mat in renderer.materials)
        // {
        //     Color startColor = mat.GetColor("_EmissionColor");
        //     Color endColor = color;
                 
        //     var i = 0.0f;
        //     var rate = 1.0f / 1.0f;
            
        //     while (i < 100) {
        //         i += rate * Time.deltaTime;             
        //         Debug.Log(i);   
        //         mat.SetColor("_EmissionColor", Color.Lerp(startColor, endColor, i));                
        //     }                
        // }

    }
    
    public IEnumerator LerpColor(Material material, Color newColor, float duration)
    {
        //float duration = 0.3f; // This will be your time in seconds.
        
        Color startColor = material.GetColor("_EmissionColor");        
        float smoothness = 0.02f; // Smaller values are smoother. Really it's the time between updates.
        float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
        float increment = smoothness / duration; //The amount of change to apply.
        while (progress < 1)
        {
            material.SetColor("_EmissionColor", Color.Lerp(startColor, newColor, progress));
            progress += increment;
            yield return new WaitForSeconds(increment);
        }
    }
    
    
}