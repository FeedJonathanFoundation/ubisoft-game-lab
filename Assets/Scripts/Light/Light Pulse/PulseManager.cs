using UnityEngine;
using System.Collections;

// Calculates where the pulse should be instantiated
// Need to deactivate pulse game object after a certain period of time
// Need to invoke repeating

public class PulseManager : MonoBehaviour
{
    public GameObject pulse;
    public Transform target;
    public bool activePulse = true;
    Camera camera;
    private float x;
    private float y;
    private LightPulse lightPulse;
    
	void Start()
    {
	   camera = GetComponent<Camera>();
       StartCoroutine(Pulse());
	}
	
	void Update()  
    {

	}
    
    IEnumerator Pulse()
    {
        CalculatePosition();
        GameObject currentPulse = (GameObject)Instantiate(pulse, new Vector3(x, y, 0f), Quaternion.identity);
        lightPulse = currentPulse.GetComponent("LightPulse") as LightPulse;
        yield return new WaitForSeconds(5);
        currentPulse.SetActive(false);
        yield return new WaitForSeconds(3);
        while (activePulse)
        {
            yield return new WaitForSeconds(2);
            CalculatePosition();
            currentPulse.transform.position = new Vector3(x, y, 0f);
            lightPulse.currentAngle = 0f;
            lightPulse.thisTime = Time.time;
            currentPulse.SetActive(true);
            yield return new WaitForSeconds(5);
            currentPulse.SetActive(false);
            yield return new WaitForSeconds(3);
        }

    }
    
    void CalculatePosition()
    {
       if (target == null)
       {
           Debug.Log("Please set a target for the Pulse Manager");
           return;
       }
       
       Vector3 viewPos = camera.WorldToViewportPoint(target.position);
       
       Debug.Log(viewPos.x);
       
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
