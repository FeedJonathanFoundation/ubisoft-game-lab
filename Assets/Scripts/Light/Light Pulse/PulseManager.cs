using UnityEngine;
using System.Collections;

// Calculates where the pulse should be instantiated
// Need to deactivate pulse game object after a certain period of time
// Need to invoke repeating

public class PulseManager : MonoBehaviour
{
    public Transform player;
    public Transform target;
    public bool activePulse = true;
    Camera camera;
    private float duration;
    private float x;
    private float y;
    
	void Start()
    {
	   camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
       duration = this.GetComponent<ParticleSystem>().duration;
       StartCoroutine(Pulse());
	}
	
	void Update()  
    {

	}
    
    IEnumerator Pulse()
    {
        // Recalculate the position of the pulse each time it loops
        while (activePulse)
        {
            CalculatePosition();
            this.transform.position = new Vector3(x, y, 0f);
            yield return new WaitForSeconds(duration);
        }

    }
    
    void CalculatePosition()
    {
        if (player == null) 
        {
           Debug.Log("Please set a player for the Pulse Manager");
           return; 
        }
        
          if (target == null)
       {
           Debug.Log("Please set a target for the Pulse Manager");
           return;
       }
       
        // Get the bounds of the camera's screen
        Vector3 botLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        Vector3 topLeft = camera.ViewportToWorldPoint(new Vector3(0, 1, camera.nearClipPlane));
        Vector3 botRight = camera.ViewportToWorldPoint(new Vector3(1, 0, camera.nearClipPlane));
        Vector3 topRight = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
        
        // Find the closest two camera points to the target 
        Vector2 camPoint1 = new Vector2(0, 0);
        Vector2 camPoint2 = new Vector2(0, 0);
        
        // Target is on the left of the camera bounds
        if (botLeft.x >= target.position.x)
        {
            camPoint1.x = botLeft.x; 
            camPoint1.y = botLeft.y;
            camPoint2.x = botRight.x;
            camPoint2.y = botRight.y;
            Vector2 intersection = lineIntersection(camPoint1, camPoint2);
            if (intersection.y == botLeft.y && intersection.x >= botLeft.x && intersection.x <= botRight.x)
            {
                x = intersection.x;
                y = intersection.y;
                return;
            }
            else
            {
                // Intersection wasn't between botLeft and botRight, check botLeft and topLeft
                camPoint2.x = topLeft.x;
                camPoint2.y = topLeft.y;
                intersection = lineIntersection(camPoint1, camPoint2);
                x = intersection.x;
                y = intersection.y;
                return;    
            }
        }
        // Target is on the right of the camera bounds
        else if (botRight.x <= target.position.x)
        {
            camPoint1.x = botRight.x; 
            camPoint1.y = botRight.y;
            camPoint2.x = botLeft.x;
            camPoint2.y = botLeft.y;
            Vector2 intersection = lineIntersection(camPoint1, camPoint2);
            if (intersection.y == botLeft.y && intersection.x >= botLeft.x && intersection.x <= botRight.x)
            {
                x = intersection.x;
                y = intersection.y;
                return;
            }
            else
            {
                // Intersection wasn't between botLeft and botRight, check botRight and topRight
                camPoint2.x = topRight.x;
                camPoint2.y = topRight.y;
                intersection = lineIntersection(camPoint1, camPoint2);
                x = intersection.x;
                y = intersection.y;
                return;
            }
        }
    }
    
    Vector2 lineIntersection(Vector2 camPoint1, Vector2 camPoint2)
    {
        // Get A,B,C of first line - points : target to player
        float A1 = player.position.y - target.position.y;
        float B1 = target.position.x - player.position.x;
        float C1 = A1 * target.position.x + B1 * target.position.y;
        
        // Get A,B,C of second line - points : camPoint1 to camPoint2
        float A2 = camPoint2.y - camPoint1.y;
        float B2 = camPoint1.x - camPoint2.x;
        float C2 = A2 * camPoint1.x + B2 * camPoint1.y;
        
        // Get delta and check if the lines are parallel
        float delta = A1 * B2 - A2 * B1;
        if(delta == 0) 
        {
            throw new System.Exception("Lines are parallel");
        }
        
        // now return the Vector2 intersection point
        return new Vector2((B2 * C1 - B1 * C2) / delta, (A1 * C2 - A2 * C1) / delta);
    }
}
