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


    void Start() 
    {
        // Find and set the canvas objects
        mainMenuCanvas = GameObject.Find ("mainMenuCanvas");
        mainOptionsCanvas = GameObject.Find ("mainOptionsCanvas");
        scenesCanvas = GameObject.Find("scenesCanvas");
        // Enable the player's cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // At start, the options and scenes menus are inactive and only the main menu can be seen
        mainOptionsCanvas.SetActive (false);
        scenesCanvas.SetActive(false);
        // Initialize the volume slider's value to halfway, or 0.5f
        volumeSlider.value = Mathf.MoveTowards(volumeSlider.value, 100.0f, 0.5f);
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
        mainOptionsCanvas.SetActive (true);
        mainMenuCanvas.SetActive (false);
    }
    
    // When the Back button is pressed, hide the Options or Scenes menu and show the Main Menu
    public void BackButton() 
    {
        if (mainOptionsCanvas.activeSelf)
        {
            mainMenuCanvas.SetActive (true);
            mainOptionsCanvas.SetActive (false);            
        }
        else
        {
            mainMenuCanvas.SetActive (true);
            scenesCanvas.SetActive (false);
        }

    }
    
    // Listens to the volume slider in the Options menu and sets the global game volume to the slider's value
    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }
    
    public void FieldButton()
    {
        SceneManager.LoadScene(1);
    }

    public void VolcanoButton()
    {
        SceneManager.LoadScene(2);
    }
    
    public void CaveButton()
    {
        SceneManager.LoadScene(3);
    }	
    
    // When the Exit button is pressed, quit the build 
    public void ExitButton() 
    {
        Application.Quit();
    }
}
