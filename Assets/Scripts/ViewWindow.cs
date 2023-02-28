using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ViewWindow
{
    public bool MaintainAspectRatio = true;

    //Top left corner
    public float X;     
    public float Y;

    //distance of the window
    public float Width;
    public float Height;

    //how many pixels/points per edge
    public int ResolutionX;
    public int ResolutionY;


    [HideInInspector]
    private float _OldWidth;
    [HideInInspector]
    private float _OldHeight;

    [HideInInspector]
    private float _OldResolutionX;
    [HideInInspector]
    private float _OldResolutionY;

    public float MaxX()
    {
        return X + Width;
    }

    public float MaxY()
    {
        return Y + Height;
    }

    public void OnValidate()
    {
        if(MaintainAspectRatio)
        {
            if(_OldWidth != Width )
            {
                //width is the value that was changed
                float aspectRatio = _OldWidth / Height;
                Height = Width / aspectRatio;
            }
            else if(_OldHeight != Height )
            {
                //height is the value that was changed
                float aspectRatio = Width / _OldHeight;
                Width = aspectRatio * Height;
            }
            
        }
        else
        {
            Width = (Width < 1) ? 1 : Width;
            Height = (Height < 1) ? 1 : Height;
            ResolutionX = (ResolutionX < 16) ? 16 : ResolutionX;
            ResolutionY = (ResolutionY < 16) ? 16 : ResolutionY;
        }

        _OldWidth = Width;
        _OldHeight = Height;
        _OldResolutionX = ResolutionX;
        _OldResolutionY = ResolutionY;        
    }
}
