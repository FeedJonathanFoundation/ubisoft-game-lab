using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour
{
    public Texture2D fadeOutTexture; // fade texture
    public float fadeSpeed = 0.8f; // fade speed
    private int drawDepth = -1000; // set on top of all other elements
    private float alpha = 1f; // alpha value 0-1
    private int fadeDir = -1; // fade in = -1, fade out = 1
    
    protected void OnGUI()
    {
        // fade out/in the alpha value using a direction, a speed and Time.deltaTime        
       this.alpha += this.fadeDir * this.fadeSpeed * Time.deltaTime;
       
       // force value betwen 0 and 1
       this.alpha = Mathf.Clamp01(this.alpha);
       
       // set color of GUI
       GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
       GUI.depth = this.drawDepth;
       GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), fadeOutTexture);         
    }
    
    /// <summary>
    /// Fades the screen in the given direction
    /// (fade in = -1, fade out = 1)
    /// </summary>
    /// <param name="direction">Direction of the fade</param>
    /// <returns></returns>
    public float BeginFade(int direction) 
    {
        fadeDir = direction;
        return (fadeSpeed);        
    }
    
    /// <summary>
    /// Starts fade out as new level is loaded
    /// </summary>
    protected void OnLevelWasLoaded()
    {
        BeginFade(-1);
    }
       
}