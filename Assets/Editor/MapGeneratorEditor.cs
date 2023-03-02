using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;
        
        if(DrawDefaultInspector())
        {
            ViewWindowController vwController = FindObjectOfType<ViewWindowController>();
            if (mapGen.AutoUpdate)
            {
                mapGen.GenerateMap(vwController.Window);
            }
        }

        if (GUILayout.Button("Generate"))
        {
            ViewWindowController vwController = FindObjectOfType<ViewWindowController>();
            mapGen.GenerateMap(vwController.Window);
        }
    }
}
