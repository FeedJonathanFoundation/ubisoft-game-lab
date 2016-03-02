using UnityEngine;
using System.Collections;

// Calculates where the pulse should be instantiated
// Need to deactivate pulse game object after a certain period of time
// Need to invoke repeating

public class PulseManager : MonoBehaviour
{
    public GameObject pulse;
    public Transform target;
    Camera camera;
    private float x;
    private float y;
    
	void Start()
    {
	   camera = GetComponent<Camera>();
       Pulse();
	}
	
	void Update()
    {
        // InvokeRepeating("Pulse", 2, 10f);
	}
    
    void Pulse()
    {
        CalculatePosition();
        GameObject currentPulse = (GameObject)Instantiate(pulse, new Vector3(x, y, 0f), Quaternion.identity);
        // yield return new WaitForSeconds(3);
        // currentPulse.SetActive(false);
    }
    
    void CalculatePosition()
    {
       if (target == null)
       {
           Debug.Log("Please set a target for the Pulse Manager");
           return;
       }
       
       Vector3 viewPos = camera.WorldToViewportPoint(target.position);
       
       // If on screen, don't generate
       if (viewPos.x > 0f && viewPos.x < 1f && viewPos.y > 0f && viewPos.y < 1f)
       {
           return;
       }
       
       if (viewPos.x > 0.5f)       // right side
       {
           x = 1f;
           y = viewPos.y;
           
       }
       else if (viewPos.x < 0.5f)  // left
       {
           x = 0f;
           y = viewPos.y;
        
       }
       else if (viewPos.y > 0.5f) // top
       {
           x = viewPos.x;
           y = 1f;
        //    top;
       }
       else if (viewPos.y < 0.5f) // bottom
       {
           x = viewPos.x;
           y = 0f;
       }
    }
}
