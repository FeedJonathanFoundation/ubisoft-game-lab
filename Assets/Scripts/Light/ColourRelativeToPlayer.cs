using UnityEngine;
using System.Collections;

/// <summary>
/// Changes the GameObject's colour based on whether it has more or less energy than the player
/// </summary>
public abstract class ColourRelativeToPlayer : MonoBehaviour
{

    [Tooltip("The colour used when this light source ")]
    public Color minColour;
    [Tooltip("The colour used when the light source is at >= maxLight")]
    public Color maxColour;
    
    [Tooltip("The higher the value, the faster the colour changes.")]
    public float changeSpeed;

    [Tooltip("If true, the children's colours are also changed")]
    public bool updateChildren;
    
    private LightSource lightSource;
    private static Player player;   // Do not access directly. Use Player property instead.
    private static bool playerSearched; // If true, the player has been searched using GameObject.FindTag().

    public virtual void Awake()
    {
        // Cache the GameObject's components
        lightSource = GetComponentInParent<LightSource>();
        
        // Set the initial colour of the light source.
        OnLightChanged(lightSource.LightEnergy.CurrentEnergy);
    }
    
    void OnEnable()
    {
        Player.LightEnergy.LightChanged += OnLightChanged;
        lightSource.LightEnergy.LightChanged += OnLightChanged;
    }
    
    void OnDisable()
    {
        if (Player != null) { Player.LightEnergy.LightChanged -= OnLightChanged; }
        lightSource.LightEnergy.LightChanged -= OnLightChanged;
    }

    /// <summary>
    /// Called when the player or this light source gains/loses light. Updates the light source's colour
    /// based on whether it has more or less light than the player.
    /// <param name="energy"> The energy for the light source that gained/lost light </param>
    /// </summary>
    protected abstract void OnLightChanged(float energy);
    
    /// <summary>
    /// Returns a colour between based on this light source's current energy
    /// lightEnergy < player ---> colour = minColour
    /// lightEnergy > player ---> colour = maxColour
    /// </summary>
    protected virtual Color GetTargetColour()
    {
        float currentEnergy = lightSource.LightEnergy.CurrentEnergy;
        float playerEnergy = Player.LightEnergy.CurrentEnergy;
        
        Color targetColour = Color.white;
        
        if (currentEnergy > playerEnergy) { targetColour = maxColour; }
        else { targetColour = minColour; }
        
        return targetColour;
    }

    /// <summary>
    /// Returns the player in the game.
    /// </summary>
    protected static Player Player
    {
        get 
        {
            // Only search for the player once
            if (!playerSearched && player == null)
            {
                GameObject playerObject = GameObject.FindWithTag("Player");
                // Debug.Log("Found player (EmissiveColourRelativeToPlayer.cs): " + playerObject);
                player = playerObject.GetComponentInParent<Player>();
                
                playerSearched = true;
            }
            
            return player;
        }
    }
}