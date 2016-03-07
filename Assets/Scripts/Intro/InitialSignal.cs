using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InitialSignal : MonoBehaviour
{


    public Text signalPrompt;
    public Text signalText;

    public Transform target;

    private bool receivedSignal = false;

    void Start()
    {
       // "Receive" signal
	   setSignalPrompt();
       signalText.text = "";
	}

    
	void Update()
    {
        if (Input.GetButtonDown("Accept"))
        {
            receivedSignal = true;
        }
        if (receivedSignal)
        {
            // TO DO: Emit blue particle system cone
            setSignalText();
        }
    }
    
    void setSignalPrompt()
    {
        // TO DO: Generate above the player
        signalPrompt.text = "Press X to accept signal";
    }
    
    void setSignalText()
    {
        // TO DO: disappear after a delay
        signalText.text = "FIND ME";
    }
    
    
    
}
