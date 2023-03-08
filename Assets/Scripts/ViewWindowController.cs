using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ViewWindowController : MonoBehaviour
{
    public ViewData Window;

    private bool RightPressed = false;
    private bool LeftPressed = false;
    private bool UpPressed = false;
    private bool DownPressed = false;

    private bool TrackMouse = false;
    private Vector3 MousePosition;

    void Start()
    {
    }

    void Update()
    {
        //Move view based on arrow keys
        UpdateKeyState(KeyCode.RightArrow, ref RightPressed);
        UpdateKeyState(KeyCode.LeftArrow, ref LeftPressed);
        UpdateKeyState(KeyCode.DownArrow, ref DownPressed);
        UpdateKeyState(KeyCode.UpArrow, ref UpPressed);

        float deltaX = Window.Width * Time.deltaTime;
        float deltaY = Window.Height * Time.deltaTime;

        TryIncrementDistance(LeftPressed, RightPressed, deltaX, ref Window.X);
        TryIncrementDistance(DownPressed, UpPressed, deltaY, ref Window.Y);

        //Move view based on mouse
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            TrackMouse = true;
            MousePosition = Input.mousePosition;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            TrackMouse = false;
        }

        if (TrackMouse)
        {
            Vector3 delta = Input.mousePosition - MousePosition;
            MousePosition = Input.mousePosition;

            Window.X -= delta.x * (Window.Width  / Screen.width);
            Window.Y += delta.y * (Window.Height / Screen.height);
            Window.NotifyOfUpdatedValues();
        }

        if(Input.mouseScrollDelta.y != 0)
        {
            float zoom =  1.0f - Input.mouseScrollDelta.y * .1f;

            float xPos = Input.mousePosition.x * (Window.Width / Screen.width);
            float yPos = Window.Height - (Input.mousePosition.y * (Window.Height / Screen.height));

            Zoom(ref Window.Width , ref Window.X, zoom, Window.MinWidth, Window.MaxWidth , xPos);
            Zoom(ref Window.Height, ref Window.Y, zoom, Window.MinHeight, Window.MaxHeight, yPos);
            Window.NotifyOfUpdatedValues();
        }
    }

    private void UpdateKeyState(KeyCode key, ref bool state)
    {
        if (Input.GetKeyDown(key))
            state = true;

        if (Input.GetKeyUp(key))
            state = false;
    }

    private bool TryIncrementDistance(bool positive, bool negative, float delta, ref float distance)
    {
        if (positive && !negative)
        {
            distance += delta;
            Window.NotifyOfUpdatedValues();
            return true;

        }

        if (negative && !positive)
        {
            distance -= delta;
            Window.NotifyOfUpdatedValues();
            return true;

        }

        return false;
    }

    private void Zoom(ref float length, ref float pos, float zoom, float min, float max, float zoomPoint)
    {
        length *= zoom;

        if(length < min)
            length = min;
        else if(length > max)
            length = max;
        else
            pos += zoomPoint * (1.0f - zoom);
        
    }
}
