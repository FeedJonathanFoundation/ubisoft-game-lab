using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour
{
    // The gameobject containing the text
    public GameObject textBox;
    // The gameobject the text is displayed on 
    public Text textDisplay;
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

    void Start()
    {
        if (isActive)
        {
            EnableTextBox();
        }
        else
        {
            DisableTextBox();
        }
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
                    StartCoroutine(TextScroll(textLines[currentLine]));
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
        StartCoroutine(TextScroll(textLines[currentLine]));
    }

    // Hides the textbox and resumes player movement
    public void DisableTextBox()
    {
        textBox.SetActive(false);
        isActive = false;
    }

    // Sets the textbox manager's global variables to new imported text
    public void LoadNewText(TextAsset newText, int startLine, int endLine)
    {
        if (newText != null)
        {
            textLines = new string[1];
            textLines = (newText.text.Split('\n'));
            currentLine = startLine;
            endAtLine = endLine;

            if (endAtLine == 0)
            {
                endAtLine = textLines.Length - 1;
            }
        }
    }
}
