using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InitialSignal : MonoBehaviour
{
    [Tooltip("Prompts player to open message.")]
    public TextMesh prompt;
    [Tooltip("The message in the hologram signal.")]
    public TextMesh message;
    [Tooltip("The hologram with a projection child.")]
    public Transform target;

    private bool receivedSignal = false;
    private GameObject projection;

    private int count = 0;

    void Start()
    {
        SetPrompt();
        ResetMessage();

        foreach (Transform child in target)
        {
            if (child.gameObject.CompareTag("Projection"))
            {
                projection = child.gameObject;
            }
        }
        if (ProjectionEnabled)
        {
            ToggleProjection();
        }
    }

    
	void Update()
    {
        if (Input.GetButtonDown("Accept"))
        {
            if (count == 0)
            {
                receivedSignal = true;
            }
            else if (count == 2)
            {
                ToggleProjection();
                ResetPrompt();
                ResetMessage();
            }
            count++;
        }
        if (receivedSignal && count == 1)
        {
            ResetPrompt();
            ToggleProjection();
            SetMessage();
            count++;
        }
    }
    
    void SetPrompt()
    {
        prompt.text = "Press X to accept signal";
    }
    
    void SetMessage()
    {
        message.text = "FIND ME";
    }
    
    void ResetPrompt()
    {
        prompt.text = "";
    }
    
    void ResetMessage()
    {
        message.text = "";
    }
    
    void ToggleProjection()
    {
        projection.SetActive(!ProjectionEnabled);
    }
    
    public bool ProjectionEnabled
    {
        get
        { 
            return projection.activeSelf;
        }
    }

}
