using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    public Slider healthBar;
    private Player player;
    public Image fill;
    
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        player.LightEnergy.LightChanged += OnLightChanged;
    }
    
    void Update()
    {
        if (healthBar.value <= 1500)
        {
            fill.color = Color.red;
        }
        else
        {
            fill.color = Color.white;
        }
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
