using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    public Slider healthBar;
    private Player player;
    
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        player.LightEnergy.LightChanged += OnLightChanged;
    }
    
    void onDisable()
    {
        player.LightEnergy.LightChanged -= OnLightChanged;
    }
    
    void OnLightChanged(float currentEnergy)
    {
        // Debug.Log("currentEnergy: " + currentEnergy + "\n healthBar: " + healthBar.value);
        healthBar.value = currentEnergy * 100;
    }
}
