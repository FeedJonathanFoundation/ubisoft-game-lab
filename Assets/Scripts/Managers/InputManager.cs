using UnityEngine;
using System.Collections;

public static class InputManager
{
    /// <summary>
    /// Returns the direction in which the left stick is pointing.
    /// </summary>
    public static Vector2 GetLeftStick()
    {
        // Get the direction the left-stick is pointing
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector2 leftStickDirection = new Vector2(horizontalInput,verticalInput);
        
        return leftStickDirection;
    }
}