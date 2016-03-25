using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField]
    [Tooltip("The volume slider object in the pause menu")]
    private Slider volumeSlider;
    // The canvas which parents the main menu objects
    private GameObject mainMenuCanvas;
    // The canvas which parents the options objects
    private GameObject mainOptionsCanvas;
    // The canvas on which the scenes buttons are
    private GameObject scenesCanvas;

    private Button[] menuButtons;
    private Button[] sceneButtons;
    private Button[] optionsButtons;
    private int selectedMenuItem;

    // 0 = main menu , 1 = option, 2 = scenes
    private int menuMode;
    private float timeBetweenButtonChange = 0.15f;
    private float currentTime;
    private float timeLastMove = 0;

    void Start()
    {
        // Find and set the canvas objects
        mainMenuCanvas = GameObject.Find("mainMenuCanvas");
        mainOptionsCanvas = GameObject.Find("mainOptionsCanvas");
        scenesCanvas = GameObject.Find("scenesCanvas");

        menuButtons = mainMenuCanvas.GetComponentsInChildren<Button>();
        sceneButtons = scenesCanvas.GetComponentsInChildren<Button>();
        optionsButtons = mainOptionsCanvas.GetComponentsInChildren<Button>();
         
        menuMode = 0;
        selectedMenuItem = 0;
        highlightButton(menuButtons);

        // Enable the player's cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // At start, the options and scenes menus are inactive and only the main menu can be seen
        mainOptionsCanvas.SetActive(false);
        scenesCanvas.SetActive(false);
        // Initialize the volume slider's value to halfway, or 0.5f
        volumeSlider.value = Mathf.MoveTowards(volumeSlider.value, 100.0f, 0.5f);        
    }
    
    void Update()
    {
        currentTime = Time.realtimeSinceStartup;
        
        if (Input.GetButtonDown("Select Menu Item"))
        {       
            if (menuMode == 0) {
                if (selectedMenuItem == 0)
                {
                    StartGameButton();
                }
                else if (selectedMenuItem == 1)
                {   
                    selectedMenuItem = 0;             
                    menuMode = 1;    
                    highlightButton(optionsButtons);
                    OptionsButton();                    
                }
                else if (selectedMenuItem == 2)
                {
                    selectedMenuItem = 0;
                    menuMode = 2;
                    highlightButton(sceneButtons);
                    ChangeSceneButton();                    
                }
                else if (selectedMenuItem == 3)
                {
                    ExitButton();
                }                
            } else if (menuMode == 1) {
                
                if (selectedMenuItem == 0)
                {
                    BackButton();
                } 
                               
            } else if (menuMode == 2) {
 
                if (selectedMenuItem == 0)
                {
                    FieldButton();
                }
                else if (selectedMenuItem == 1)
                {
                    VolcanoButton();                    
                }
                else if (selectedMenuItem == 2)
                {
                    CaveButton();
                }
                else if (selectedMenuItem == 3)
                {                    
                    BackButton();
                } 
            }   
            
        }
            
        if (Input.GetAxisRaw("Horizontal") > 0 && (currentTime - timeLastMove) >= timeBetweenButtonChange)
        {   
            if (menuMode == 0) { MoveMenu(-1); }    
            else if (menuMode == 1) { volumeSlider.Select(); ChangeVolume(); }
            else if (menuMode == 2) { MoveScenesMenu(-1); }                   
            timeLastMove = currentTime;        
        }
        else if (Input.GetAxisRaw("Horizontal") < 0 && (currentTime - timeLastMove) >= timeBetweenButtonChange)
        {
            if (menuMode == 0) { MoveMenu(1); }
            else if (menuMode == 1) { volumeSlider.Select(); ChangeVolume(); }
            else if (menuMode == 2) { MoveScenesMenu(1); }
            timeLastMove = currentTime;              
        }            
                       
        if (Input.GetButtonDown("Cancel") && menuMode > 0) 
        {
            this.BackButton();
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

        highlightButton(menuButtons);
        
    }
    
    private void MoveScenesMenu(int menuMoveDirection)
    {

        if (menuMoveDirection > 0)
        {
            if (selectedMenuItem > 0)
            {
                selectedMenuItem--;
            }
            else
            {
                selectedMenuItem = sceneButtons.Length - 1;
            }
        }
        else
        {
            if (selectedMenuItem < sceneButtons.Length - 1)
            {
                selectedMenuItem++;
            }
            else
            {
                selectedMenuItem = 0;
            }
        }

        highlightButton(sceneButtons);
        
    }
    private void highlightButton(Button [] buttons)
    {
        ColorBlock colors = buttons[selectedMenuItem].colors;

        // reset all buttons
        foreach (Button button in buttons) { button.colors = colors; }

        // Set button at selected index to appear highlighter                              
        colors.normalColor = new Color(0.588f, 0.588f, 0.588f, 0.5f);
        colors.highlightedColor = new Color(0.588f, 0.588f, 0.588f, 0.5f);
        buttons[selectedMenuItem].colors = colors;
    }

    // When the Start Game button is pressed, load the second scene in the build
    public void StartGameButton()
    {
        SceneManager.LoadScene(1);
    }

    // When the Change Scene button is pressed, open a submenu to change the scene
    public void ChangeSceneButton()
    {
        mainMenuCanvas.SetActive(false);
        scenesCanvas.SetActive(true);
    }

    // When the Options button is pressed, hide the Main Menu and show the Options menu
    public void OptionsButton()
    {
        mainOptionsCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
    }

    // When the Back button is pressed, hide the Options or Scenes menu and show the Main Menu
    public void BackButton()
    {
        selectedMenuItem = 0;
        menuMode = 0;
        highlightButton(menuButtons);
        if (mainOptionsCanvas.activeSelf)
        {
            mainMenuCanvas.SetActive(true);
            mainOptionsCanvas.SetActive(false);
        }
        else
        {            
            mainMenuCanvas.SetActive(true);
            scenesCanvas.SetActive(false);
        }

    }

    // Listens to the volume slider in the Options menu and sets the global game volume to the slider's value
    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    public void FieldButton()
    {
        SceneManager.LoadScene(2);
    }

    public void VolcanoButton()
    {
        SceneManager.LoadScene(3);
    }

    public void CaveButton()
    {
        SceneManager.LoadScene(4);
    }

    // When the Exit button is pressed, quit the build 
    public void ExitButton()
    {
        Application.Quit();
    }
}
