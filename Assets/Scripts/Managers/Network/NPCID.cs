using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NPCID : NetworkBehaviour
{

    [SyncVar]
    public string npcID;

    private Transform npcTransform;

    [SerializeField]
    private string npcPrefabName = "Network";

    private bool identified = false;

    // Use this for initialization
    void Start()
    {
        npcTransform = transform;
    }
	
	// Update is called once per frame
	void Update()
    {
        if (!identified)
        {
            SetIdentity();
        }
    }
    
    void SetIdentity()
    {
        if (npcTransform.name == "" || npcTransform.name.Contains(npcPrefabName))
        {
            npcTransform.name = npcID;
        }
        identified = true;
    }
}
