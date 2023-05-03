using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ViewWindowController : MonoBehaviour
{
    public ViewData Window;
    public WorldSampler World;

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

        float deltaLat = Window.LatAngle * Time.deltaTime/5;
        float deltaLon = Window.LonAngle * Time.deltaTime/5;
        if(TryIncrementRotation(DownPressed, UpPressed, deltaLat, ref Window.ZRotation) ||
           TryIncrementRotation(LeftPressed, RightPressed, deltaLon, ref Window.YRotation))
        {
            World.UpdateViewWindow(Window);
        }

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

            Window.YRotation -= delta.x * (Window.LonAngle / Screen.width);
            Window.ZRotation -= delta.y * (Window.LatAngle / Screen.height);
            World.UpdateViewWindow(Window);
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            float zoom = 1.0f - Input.mouseScrollDelta.y * .1f;

            float xPos = Window.LonAngle * ((Input.mousePosition.x / Screen.width) - 0.5f);
            float yPos = Window.LatAngle * ((Input.mousePosition.y / Screen.height) - 0.5f);

            Zoom(ref Window.ViewAngle, ref Window.YRotation, ref Window.ZRotation, zoom, ViewData.MinViewAngle, Coordinates.MaxLon - Coordinates.MinLon, xPos, yPos);
            World.UpdateViewWindow(Window);
        }
    }

    private void UpdateKeyState(KeyCode key, ref bool state)
    {
        if (Input.GetKeyDown(key))
            state = true;

        if (Input.GetKeyUp(key))
            state = false;
    }

    private bool TryIncrementRotation(bool positive, bool negative, float delta, ref float distance)
    {
        if (positive && !negative)
        {
            distance += delta;
            while (distance < 0)
                distance += Mathf.PI * 2;
            while (distance > Mathf.PI * 2)
                distance -= Mathf.PI * 2;
            return true;

        }

        if (negative && !positive)
        {
            distance -= delta;
            while (distance < 0)
                distance += Mathf.PI * 2;
            while (distance > Mathf.PI * 2)
                distance -= Mathf.PI * 2;
            return true;
        }

        return false;
    }

    private void Zoom(ref float length, ref float posX, ref float posY, float zoom, float min, float max, float zoomPointX, float zoomPointY)
    {
        length *= zoom;

        if(length < min)
            length = min;
        else if(length > max)
            length = max;
        else
        {
            posX += zoomPointX * (1.0f - zoom);
            posY += zoomPointY * (1.0f - zoom);
        }
        
    }

    private float GetAxisRotation(float u1, float v1, float u2, float v2)
    {
        float a1 = CalculatePolarAngle(u1, v1);
        float a2 = CalculatePolarAngle(u2, v2);

        return a2 - a1;
    }

    private float CalculatePolarAngle(float u1, float v1)
    {
        float angle;
        if (v1 == 0.0f)
        {
            if (u1 > 0.0f)
                return Mathf.PI / 2;
            if (u1 < 0.0f)
                return -Mathf.PI / 2;            
            
            return 0.0f;
        }

        return Mathf.Atan(u1 / v1);
    }
}
