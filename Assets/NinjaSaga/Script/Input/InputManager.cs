using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    [Header("Keyboard keys")]
    public KeyCode Left = KeyCode.LeftArrow;
    public KeyCode Right = KeyCode.RightArrow;
    public KeyCode Up = KeyCode.UpArrow;
    public KeyCode Down = KeyCode.DownArrow;

    public delegate void InputEventHandler(Vector2 dir);
    public static event InputEventHandler onInputEvent;

    [HideInInspector]
    public Vector2 dir;


    private void Update()
    {
        KeyboardControls();
    }

    private void KeyboardControls()
    {
        float x = 0;
        float y = 0;
        if (Input.GetKey(Left)) x = -1;
        if (Input.GetKey(Right)) x = 1;
        if (Input.GetKey(Up)) y = 1;
        if (Input.GetKey(Down)) y = -1;
        dir = new Vector2(x,y);
        InputEvent(dir);
    }
    public static void InputEvent(Vector2 dir)
    {
        if (onInputEvent != null) onInputEvent(dir);
    }
}
