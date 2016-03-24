using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour {
	
	void Update () {
	    if (Input.GetButtonDown("Select Menu Item"))
        {
            SceneManager.LoadScene(0);
        }
	}
}
