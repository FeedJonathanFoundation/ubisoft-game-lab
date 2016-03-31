using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroTextManager : MonoBehaviour
{
    // The gameobject containing the text
    public GameObject textBox;
    // The gameobject the text is displayed on 
    public Text textDisplay;
    public TextAsset textFile;
    // String array containing the lines of text seperated by \n
    private string[] textLines;
    // The current line (index) of textLines
    private int currentLine;
    // The last line that should be read
    private int endAtLine;
    // Switch to know whether or not the textbox is displayed on screen
    private bool isActive = false;
    // Switch to know whether the text is currently typing out or is stopped
    private bool isTyping = false;
    // Switch to know whether to cancel the typing process and display the full line immediately 
    private bool cancelTyping = false;
    // The speed at which text is written out
    private float typeSpeed = 0.025f;
    private IEnumerator textScroll;

    void Start()
    {
        if (textFile != null)
        {
            textLines = textFile.text.Split('\n');
            endAtLine = textLines.Length - 1;
            EnableTextBox();
        }
        else
        {
            DisableTextBox();
        }
        
        AkSoundEngine.PostEvent("Ambient2", this.gameObject);
        AkSoundEngine.SetState("IMAmb2", "CP2");
    }

    void Update()
    {
        if (!isActive) { return; }

        if (Input.GetButtonDown("Select Menu Item") || Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (!isTyping)
            {
                currentLine += 1;
                if (currentLine > endAtLine)
                {
                    DisableTextBox();
                }
                else
                {
                    if (textScroll != null) { StopCoroutine(textScroll); }
                    textScroll = TextScroll(textLines[currentLine]);
                    StartCoroutine(textScroll);
                }
            }
            else if (isTyping && !cancelTyping)
            {
                cancelTyping = true;
            }
        }
    }

    // Types out the text, character by character, at a steady rate
    private IEnumerator TextScroll(string lineOfText)
    {
        int letter = 0;
        textDisplay.text = "";
        isTyping = true;
        cancelTyping = false;

        // Type out the text until done at typeSpeed speed
        while (isTyping && !cancelTyping && letter < lineOfText.Length - 1)
        {
            textDisplay.text += lineOfText[letter];
            letter += 1;
            yield return new WaitForSeconds(typeSpeed);
        }
        // If cancelTyping is true, exit the loop and siaply all the current line of text
        textDisplay.text = lineOfText;
        isTyping = false;
        cancelTyping = false;
    }

    // Shows the textbox, stops the player from moving and starts scrolling through text
    public void EnableTextBox()
    {
        textBox.SetActive(true);
        isActive = true;
        if (textScroll != null) { StopCoroutine(textScroll); }
        textScroll = TextScroll(textLines[currentLine]);
        StartCoroutine(textScroll);
    }

    // Hides the textbox and resumes player movement
    public void DisableTextBox()
    {
        AkSoundEngine.PostEvent("Ambient2Stop", this.gameObject);
        SceneManager.LoadScene(2);
        
    }
}
