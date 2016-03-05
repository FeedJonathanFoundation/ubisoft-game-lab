using UnityEngine;
using System.Collections;

// Creates a "pulse" ring
// Need to add a transparent material and 
// implement fade out

public class LightPulse : MonoBehaviour
{
    public float degreesPerSegment;
    public float radialScale;
    public float currentAngle;
    public float thisTime = 0f;
    private LineRenderer lineRenderer;
    private int count = 0;
    
	void Awake()
    {
	   lineRenderer = GetComponent<LineRenderer>();
       lineRenderer.SetVertexCount((int)(360 / degreesPerSegment + 1));
       lineRenderer.useWorldSpace = false;
       lineRenderer.SetWidth(0.08f, 0.08f);
       currentAngle = 0f;
	}
	
	void Update()
    {
	   CreatePoints();
	}
    
    void CreatePoints()
    {
        float x;
        float y;
        float z = 0f;
        
        for (int i = 0; i < 360 / degreesPerSegment + 1; i++)
        {
            float t = Time.time; 
            
            x = Mathf.Sin(Mathf.Deg2Rad * currentAngle);
            y = Mathf.Cos(Mathf.Deg2Rad * currentAngle);
            Vector3 position = new Vector3(x,y,z);
            
            lineRenderer.SetPosition(i, position * (t - thisTime) * radialScale);
            currentAngle += degreesPerSegment;
        }
    }
}
