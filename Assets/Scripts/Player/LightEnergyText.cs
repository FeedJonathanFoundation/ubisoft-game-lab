using UnityEngine;
using System.Collections;

public class LightEnergyText : MonoBehaviour
{
    private TextMesh text;
    private LightSource lightSource;
    
    void Start()
    {
        text = GetComponent<TextMesh>();
        
        lightSource = GetComponentInParent<LightSource>();
    }
    
    void Update()
    {
        text.text = (int)(lightSource.LightEnergy.CurrentEnergy*100) + "";
        
        float textAngle = transform.parent.eulerAngles.z;
        
        if (textAngle > 90 && textAngle < 270)
        {
            textAngle += 180;
        }
        
        transform.eulerAngles = new Vector3(0,0,textAngle);
    }
}