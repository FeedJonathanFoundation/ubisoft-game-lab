using UnityEngine;
using System.Collections;

public class LightControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (Input.GetKeyDown("f")) {
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
    
    public void ToggleLights (bool toggle)
    {
        foreach (Behaviour childComponent in GetComponent<Light>().GetComponentsInChildren<Light>()) 
        {
            childComponent.enabled = toggle;
        }
    }
}
