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
    private Vector2 OldMouseCoord;

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

        if(RightPressed || LeftPressed || UpPressed || DownPressed)
        {
            float xPercent = 0.5f;
            if(RightPressed)
                xPercent += Window.LonAngle * Time.deltaTime/100.0f;
            if(LeftPressed)
                xPercent -= Window.LonAngle * Time.deltaTime/100.0f;

            float yPercent = 0.5f; 
            if(UpPressed)
                yPercent += Window.LatAngle * Time.deltaTime/100.0f;
            if (DownPressed)
                yPercent -= Window.LatAngle * Time.deltaTime/100.0f;

            Vector3 origin = Coordinates.MercatorToCartesian(0.5f, 0.5f, Window, 1.0f);
            Vector3 newO = Coordinates.MercatorToCartesian(xPercent, yPercent, Window, 1.0f);
            //Debug.Log($"({origin.x},{origin.y},{origin.z}) -> ({newO.x},{newO.y},{newO.z})");
            Window.XRotation += GetAxisRotation(origin.y, origin.z, newO.y, newO.z);
            Window.YRotation += GetAxisRotation(origin.x, origin.z, newO.x, newO.z);
            Window.ZRotation += GetAxisRotation(origin.x, origin.y, newO.x, newO.y);
            Window.NotifyOfUpdatedValues();
        }
        

        //TryIncrementRotation(DownPressed, UpPressed, deltaLat * Mathf.Cos(yRotation), ref Window.ZRotation);
        //TryIncrementRotation(DownPressed, UpPressed, deltaLat * Mathf.Sin(yRotation), ref Window.XRotation);

        //TryIncrementRotation(LeftPressed, RightPressed, deltaLon * Mathf.Cos(zRotation), ref Window.YRotation);
        //TryIncrementRotation(LeftPressed, RightPressed, deltaLon * Mathf.Sin(zRotation), ref Window.XRotation);

        //Move view based on mouse
        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    TrackMouse = true;
        //    float xPercent = Input.mousePosition.x / Screen.width;
        //    float yPercent = Input.mousePosition.y / Screen.height;
        //    OldMouseCoord = Coordinates.MercatorToCoord(xPercent, yPercent, Window);
        //}
        //if (Input.GetKeyUp(KeyCode.Mouse0))
        //{
        //    TrackMouse = false;
        //}

        //if (TrackMouse)
        //{
        //    float newXPercent = Input.mousePosition.x / Screen.width;
        //    float newYPercent = Input.mousePosition.y / Screen.height;
        //    Vector2 newCoord = Coordinates.MercatorToCoord(newXPercent, newYPercent, Window);

        //    Window.PointsOfInterest = new List<Vector2>();
        //    Window.PointsOfInterest.Add(OldMouseCoord);
        //    Window.PointsOfInterest.Add(newCoord);

        //    deltaLon = newCoord.x - OldMouseCoord.x;
        //    deltaLat = newCoord.y - OldMouseCoord.y;

        //    //Window.YRotation += deltaLon;
        //    //Window.ZRotation += deltaLat;

        //    //Window.XRotation += GetAxisRotation(OldMousePos.y, OldMousePos.z, newPos.y, newPos.z);
        //    //Window.YRotation += GetAxisRotation(OldMousePos.x, OldMousePos.z, newPos.x, newPos.z);
        //    //Window.ZRotation -= GetAxisRotation(OldMousePos.x, OldMousePos.y, newPos.x, newPos.y);

        //    OldMouseCoord = newCoord;
        //    Window.NotifyOfUpdatedValues();
        //}

        //if (Input.mouseScrollDelta.y != 0)
        //{
        //    float zoom = 1.0f - Input.mouseScrollDelta.y * .1f;

        //    float xPos = Input.mousePosition.x * (Window.LonAngle / Screen.width);
        //    float yPos = Window.LatOffset - (Input.mousePosition.y * (Window.LatAngle / Screen.height));

        //    Zoom(ref Window.LonAngle, ref Window.LonOffset, zoom, Window.MinAngle, Coordinates.MaxLon, xPos);
        //    Zoom(ref Window.LatAngle, ref Window.LatOffset, zoom, Window.MinAngle / 2f, Coordinates.MaxLat, yPos);
        //    Window.NotifyOfUpdatedValues();
        //}
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
            Window.NotifyOfUpdatedValues();
            return true;

        }

        if (negative && !positive)
        {
            distance -= delta;
            while (distance < 0)
                distance += Mathf.PI * 2;
            while (distance > Mathf.PI * 2)
                distance -= Mathf.PI * 2;
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

    private float GetAxisRotation(float u1, float v1, float u2, float v2)
    {
        float uMag = (u1 - u2) * (u1 - u2);
        float vMag = (v1 - v2) * (v1 - v2);
        float d = Mathf.Sqrt(uMag + vMag);
        float angle = Mathf.Asin(d / 2.0f);

        if(u1 > 0 && u2 > 0)
        {
            float vDelta = v2 - v1;
            if (vDelta < 0)
                angle *= -1;
        }
        else if(u1 < 0 && u2 < 0)
        {
            float vDelta = v2 - v1;
            if (vDelta > 0)
                angle *= -1;
        }
        else
        {
            float uDelta = u2 - u1;
            if(uDelta > 0)
                angle *= -1;
        }
        //Debug.Log(angle);
        return angle;
    }
}
