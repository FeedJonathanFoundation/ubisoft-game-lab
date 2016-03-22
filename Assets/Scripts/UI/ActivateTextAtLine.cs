using UnityEngine;

// Sends the text from the collider to the text manager
public class ActivateTextAtLine : MonoBehaviour 
{
    public TextAsset theText;
    private GameObject textBoxManagerObject;
    private TextBoxManager textBoxManager;
    
    private int startLine;
    private int endLine;
    private bool destroyWhenFinished = true;
    
    void Start()
    {
        textBoxManagerObject = GameObject.Find("TextBoxManager");
        textBoxManager = (TextBoxManager) textBoxManagerObject.GetComponent("TextBoxManager");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            textBoxManager.LoadNewText(theText, startLine, endLine);
            textBoxManager.EnableTextBox();

            if (destroyWhenFinished)
            {
                Destroy(gameObject);
            }
        }
    }
}