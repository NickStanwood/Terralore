using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{

    public WindData windData;
    public WorldSampler Sampler;

    List<GameObject> windCurrents;
    
    // Start is called before the first frame update
    void Start()
    {
        windCurrents = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        DestroyWindCurrents();

        if (!windData.ShowWindCurrents)
            return;

        Vector3[] positions = new Vector3[5];
        for(int i = 0; i < positions.Length; i++)
        {
            float lat = (i * Mathf.PI / positions.Length) - (Mathf.PI / 2);
            positions[i] = Sampler.SampleFromCoord(0.0f, lat).WorldPos;
            positions[i].y += 1;
        }

        //positions[0] = new Vector3(-100, 1, windCurrents.Count * 10);
        //positions[1] = new Vector3( 100, 1, windCurrents.Count * 10);
        //positions[2] = new Vector3( 100, 1, windCurrents.Count * 10 + 50);
        //positions[3] = new Vector3(-100, 1, windCurrents.Count * 10 + 50);
        //positions[4] = new Vector3(-100, 1, windCurrents.Count * 10 + 1);

        LineRenderer line = CreateWindCurrent(positions);
    }

    private LineRenderer CreateWindCurrent( Vector3[] positions )
    {
        GameObject gObject = new GameObject($"wind{windCurrents.Count}");
        windCurrents.Add(gObject);

        LineRenderer line = gObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.SetColors(windData.LineColour, windData.LineColour);
        line.SetWidth(3, 0.1f);
        line.numCapVertices = 3;
        line.numCornerVertices = 3;
        line.positionCount = positions.Length;
        line.SetPositions(positions);

        return line;
    }

    private void DestroyWindCurrents()
    {
        foreach(GameObject o in windCurrents)
        {
            Destroy(o);
        }
        windCurrents.Clear();
    }
}
