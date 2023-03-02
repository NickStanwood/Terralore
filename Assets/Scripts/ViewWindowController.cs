using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ViewWindowController : MonoBehaviour
{
    public ViewWindow window;

    private bool RightPressed = false;
    private bool LeftPressed = false;
    private bool UpPressed = false;
    private bool DownPressed = false;

    public UnityEvent<ViewWindow> WindowUpdated;

    void Start()
    {
        if (WindowUpdated == null)
            WindowUpdated = new UnityEvent<ViewWindow>();
    }

    void Update()
    {
        UpdateKeyState(KeyCode.RightArrow, ref RightPressed);
        UpdateKeyState(KeyCode.LeftArrow , ref LeftPressed);
        UpdateKeyState(KeyCode.DownArrow , ref DownPressed);
        UpdateKeyState(KeyCode.UpArrow   , ref UpPressed);

        float deltaX = window.Width * Time.deltaTime;
        float deltaY = window.Height * Time.deltaTime;

        if( TryIncrementDistance(RightPressed, LeftPressed, deltaX, ref window.X))
        {
            //Debug.Log("calling 'WindowUpdated''");
            WindowUpdated.Invoke(window);
        }

        if (TryIncrementDistance(DownPressed, UpPressed, deltaY, ref window.Y))
        {
            //Debug.Log("calling 'WindowUpdated''");
            WindowUpdated.Invoke(window);
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
            return true;

        }

        if (negative && !positive)
        {
            distance -= delta;
            return true;

        }

        return false;
    }
}
