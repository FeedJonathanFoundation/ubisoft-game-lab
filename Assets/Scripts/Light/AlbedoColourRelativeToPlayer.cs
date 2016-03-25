using UnityEngine;
using System.Collections;

/// <summary>
/// Changes the GameObject's albedo colour based on whether it has more or less energy than the player
/// </summary>
public class AlbedoColourRelativeToPlayer : ColourRelativeToPlayer
{    
    // The MeshRenderer attached to this GameObject
    private MeshRenderer myRenderer;
    // The MeshRenderer attached to this GameObject's children
    private MeshRenderer[] renderers;
    // Caches the GameObject's component
    private SkinnedMeshRenderer skinnedMeshRenderer;

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
        
        if (MyRenderer != null)
        {
            StartCoroutine(UpdateAlbedoColour(MyRenderer.material, targetColour));
        }
        
        if (SkinnedMeshRenderer != null)
        {
            StartCoroutine(UpdateAlbedoColour(SkinnedMeshRenderer.material,targetColour));
        }
    }
    
    /// <summary>
    /// Gradually changes the material's albedo colour to the targetColour.
    /// The higher the changeRate, the faster the change occurs.
    /// </summary>
    private IEnumerator UpdateAlbedoColour(Material material, Color targetColour)
    {
        while (true)
        {
            // Gradually lerp to the target colour
            Color currentColour = material.GetColor("_AlbedoColor");
            Color newColour = Color.Lerp(currentColour, targetColour, changeSpeed * Time.deltaTime);
            
            material.SetColor("_AlbedoColor", newColour);
            
            yield return null;
        }
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
    
    private SkinnedMeshRenderer SkinnedMeshRenderer
    {
        get { return skinnedMeshRenderer; }
        set { skinnedMeshRenderer = value; }
    }
}