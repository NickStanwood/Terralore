using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{

    public WindData windData;
    public ViewData window;

    List<LineRenderer> windCurrents;
    
    // Start is called before the first frame update
    void Start()
    {
        windCurrents = new List<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!windData.ShowWindCurrents)
            return;

        if (windCurrents.Count > 3)
            return;

        Vector3[] positions = new Vector3[5];
        positions[0] = new Vector3(-100, 1, windCurrents.Count * 10);
        positions[1] = new Vector3( 100, 1, windCurrents.Count * 10);
        positions[2] = new Vector3( 100, 1, windCurrents.Count * 10 + 50);
        positions[3] = new Vector3(-100, 1, windCurrents.Count * 10 + 50);
        positions[4] = new Vector3(-100, 1, windCurrents.Count * 10 + 1);

        LineRenderer line = CreateWindCurrent(positions);
    }

    private LineRenderer CreateWindCurrent( Vector3[] positions )
    {
        GameObject gObject = new GameObject($"wind{windCurrents.Count}");
        LineRenderer line = gObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.SetColors(windData.LineColour, windData.LineColour);
        line.SetWidth(3, 0.1f);
        line.numCapVertices = 3;
        line.numCornerVertices = 3;
        line.positionCount = positions.Length;
        line.SetPositions(positions);
        windCurrents.Add(line);

        return line;
    }
}
