using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Current : MonoBehaviour
{
    
    public float strength;
    private bool empty;
    private List<Rigidbody> rbs;
    
    void Start()
    {
        rbs = new List<Rigidbody>();
        empty = true;
    }
    
    void Update()
    {
        if (!empty)
        {
            AddCurrentForce();
        }
    }
    
    void OnTriggerEnter(Collider col) 
    {
        if (col.gameObject.tag == "Player" || col.gameObject.tag == "Fish")
        {
            Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
            if (rb != null) 
            {
                rbs.Add(rb);
                empty = false;
            } 
        }
    }
    
    void OnTriggerExit(Collider col)
    {
        Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
        if (rb != null && rbs.Contains(rb)) 
        {
            rbs.Remove(rb);
            if (rbs.Count == 0)
            {
                empty = true;
            } 
        }
    }
    
    void AddCurrentForce()
    {
        foreach (Rigidbody rb in rbs)
        {
            if (rb != null) 
            {
                rb.AddForce(new Vector3(0f, -1f, 0f) * strength);
            }
        }
    }
    
}
