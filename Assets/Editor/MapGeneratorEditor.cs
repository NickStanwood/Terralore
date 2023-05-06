using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapGenerator mapGen = (MapGenerator)target;

        if (GUILayout.Button("Generate"))
        {
            System.Random rand = new System.Random();
            mapGen.worldSampler.WorldData.HeightData.Seed = rand.Next(0, 100000);
            mapGen.worldSampler.WorldData.MountainData.Seed = rand.Next(0, 100000);
            mapGen.worldSampler.WorldData.HeatData.Seed = rand.Next(0, 100000);
            mapGen.worldSampler.WorldData.NotifyOfUpdatedValues();
        }
    }
}
