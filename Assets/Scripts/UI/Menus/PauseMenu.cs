using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The volume slider object in the pause menu")]
    private Slider volumeSlider;
    // The UI canvas on which the pause menu is drawn
    private GameObject pauseCanvas;
    // The UI canvas on which the options submenu is drawn
    private GameObject pauseOptionsCanvas;
    /* A switch to decide whether to open or close the pause menu when the escape key is pressed
        0 = not paused, 1 = pause canvas is active and 2 = options submenu is active*/
    private int pauseMode = 0;

    private Button[] menuButtons;
    private int selectedMenuItem;
    //private bool isAxisInUse;
    private float timeLastMove = 0;
    [SerializeField]
    private float timeBetweenButtonChange = 0.2f;
    private float currentTime;

    void Awake()
    {
        // Find and set the canvases
        pauseCanvas = GameObject.Find("pauseCanvas");
        pauseOptionsCanvas = GameObject.Find("optionsCanvas");

        menuButtons = pauseCanvas.GetComponentsInChildren<Button>();
        selectedMenuItem = 0;
        highlightButton();

        // Hide the canvases
        pauseCanvas.SetActive(false);
        pauseOptionsCanvas.SetActive(false);
    }

    void Update()
    {
        currentTime = Time.realtimeSinceStartup;
        
        
        // If the escape key is pressed and the game is not currently paused
        if (Input.GetButtonDown("Pause Menu") && pauseMode == 0)
        {
            pauseMode = 1;
            pauseCanvas.SetActive(true);
            
            // Freezes the game time
            Time.timeScale = 0f;
            // Free the cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        // The escape key is pressed while the pause menu is open
        else if (Input.GetButtonDown("Pause Menu") && pauseMode != 0)
        {
            // The pause menu was open, therefore it it closed and the game is resumed
            if (pauseMode == 1)
            {
                pauseMode = 0;
                pauseCanvas.SetActive(false);

                Time.timeScale = 1f;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            // The options submenu was open, therefore close it and open the options menu
            else
            {
                pauseMode = 1;
                pauseCanvas.SetActive(true);
                pauseOptionsCanvas.SetActive(false);
            }
        }

        if (pauseMode == 1)
        {
            if (Input.GetButtonDown("Select Menu Item"))
            {
                pauseCanvas.SetActive(true);
                menuButtons[selectedMenuItem].onClick.Invoke();
            }
            
              
            
            if (Input.GetAxisRaw("Vertical") > 0 && (currentTime - timeLastMove) >= timeBetweenButtonChange)
            {   
                MoveMenu(1);       
                timeLastMove = currentTime;        
                //isAxisInUse = true;
                
            }
            else if (Input.GetAxisRaw("Vertical") < 0 && (currentTime - timeLastMove) >= timeBetweenButtonChange)
            {
                MoveMenu(-1);
                timeLastMove = currentTime;  
                //isAxisInUse = true;                
            } 
            // else 
            // {
            //     //isAxisInUse = false;    
            // }

            
        }


    }

    private void MoveMenu(int menuMoveDirection)
    {

        if (menuMoveDirection > 0)
        {
            if (selectedMenuItem > 0)
            {
                selectedMenuItem--;
            }
            else
            {
                selectedMenuItem = menuButtons.Length - 1;
            }
        }
        else
        {
            if (selectedMenuItem < menuButtons.Length - 1)
            {
                selectedMenuItem++;
            }
            else
            {
                selectedMenuItem = 0;
            }
        }

        highlightButton();
        
    }

    private void highlightButton()
    {

        ColorBlock colors = menuButtons[selectedMenuItem].colors;

        // reset all buttons
        foreach (Button button in menuButtons) { button.colors = colors; }

        // Set button at selected index to appear highlighter                              
        colors.normalColor = new Color(0.588f, 0.588f, 0.588f);
        colors.highlightedColor = new Color(0.588f, 0.588f, 0.588f);
        menuButtons[selectedMenuItem].colors = colors;
    }

    // When the Resume button is pressed, the pause menu is hidden and the game time resumes
    public void ResumeButton()
    {
        pauseMode = 0;
        pauseCanvas.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // When the Options button is pressed, the pause menu is hidden and the options submenu is shown
    public void OptionsButton()
    {
        pauseMode = 2;
        pauseOptionsCanvas.SetActive(true);
        pauseCanvas.SetActive(false);
    }

    // Listens to the volume slider in the Options menu and sets the global game volume to the slider's value
    public void ChangeVolume()
    {
        float volume = volumeSlider.value * 100;
        AkSoundEngine.SetRTPCValue("MasterVolume", volume);
    }

    // When the Back button is pressed, the options submenu is hidden and the Pause menu is shown
    public void BackButton()
    {
        pauseMode = 1;
        pauseCanvas.SetActive(true);
        pauseOptionsCanvas.SetActive(false);
    }

    // When the Reload button is pressed, the active scene is reloaded from its initial settings and the pause menu is hidden
    public void ReloadButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        ResumeButton();
    }

    // When the Exit To Main Menu button is pressed, the Main Menu scene is loaded
    public void ExitToMainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // When the Exit To Desktop button is pressed, the game is closed
    public void ExitToDesktopButton()
    {
        Application.Quit();
    }
}