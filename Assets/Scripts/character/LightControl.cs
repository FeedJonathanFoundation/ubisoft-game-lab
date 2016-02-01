using UnityEngine;
using System.Collections;

public class LightControl : MonoBehaviour 
{
	void Update()
    {
		if (Input.GetKeyDown("f")) 
        {
			if (GetComponent<Light>().enabled)
            {
				ToggleLights(false);
			}
			else 
            {
				ToggleLights(true);
			}
		}
	}
    
    public void ToggleLights(bool toggle)
    {
        Behaviour[] lightComponents = GetComponent<Light>().GetComponentsInChildren<Light>();
        foreach (Behaviour childComponent in lightComponents) 
        {
            childComponent.enabled = toggle;
        }
    }
}
