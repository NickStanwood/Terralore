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
    public float Width    = 2000;
    public float Height   = 1000;

    public float MaxWidth = 2000;
    public float MaxHeight= 1000;

    //how many pixels/points per edge
    public int ResolutionX = 256;
    public int ResolutionY = 128;

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
                Width = Mathf.Min(Width, MaxWidth);

                //width is the value that was changed
                float aspectRatio = _OldWidth / Height;
                Height = Width / aspectRatio;
            }
            else if(_OldHeight != Height )
            {
                Height = Mathf.Min(Height, MaxHeight);

                //height is the value that was changed
                float aspectRatio = Width / _OldHeight;
                Width = aspectRatio * Height;
            }
            
        }

        Width   = (Width < 1  || float.IsNaN(Width)  || float.IsInfinity(Width))  ? 1 : Width;
        Height  = (Height < 1 || float.IsNaN(Height) || float.IsInfinity(Height)) ? 1 : Height;
        if(Width > MaxWidth || Height > MaxHeight)
        {
            Width = MaxWidth;
            Height = MaxHeight;
        }

        ResolutionX = (ResolutionX < 16) ? 16 : ResolutionX;
        ResolutionY = (ResolutionY < 16) ? 16 : ResolutionY;

        _OldWidth = Width;
        _OldHeight = Height;
        _OldResolutionX = ResolutionX;
        _OldResolutionY = ResolutionY;        
    }
}
