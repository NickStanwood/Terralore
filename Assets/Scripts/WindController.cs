using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{

    public WindData windData;
    public WorldSampler Sampler;
    public int WindCurrentColumns = 10;
    public int WindCurrentRows = 4;

    List<GameObject> windCurrents;
    bool WindInvalidated = false;
    // Start is called before the first frame update
    void Start()
    {
        windCurrents = new List<GameObject>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!WindInvalidated)
            return;

        DestroyWindCurrents();

        if (!windData.ShowWindCurrents)
            return;

        List<Vector3> knots = new List<Vector3>();
        int sampleCount = 5;
        WorldSample lastSample = new WorldSample{xIndex = -1};

        int xSampleFreq = Mathf.Max(Sampler.MapIndexWidth() / WindCurrentColumns, 1);
        int ySampleFreq = Mathf.Max(Sampler.MapIndexHeight() / WindCurrentRows, 1);
        for (int x = 0; x < WindCurrentColumns; x++)
        {
            for(int y = 0; y < WindCurrentRows; y++)
            {
                WorldSample s = Sampler.SampleFromIndex(x * xSampleFreq, y * ySampleFreq);
                knots.Add(s.WorldPos);
                for (int i = 0; i < sampleCount; i++)
                {
                    //add wind velocity and rotation to s world space
                    //sample from new world space
                    knots.Add(s.WorldPos);
                }
            }
        }

        //for(int i = 0; i < sampleCount; i++)
        //{
        //    float lat = (i * (Mathf.PI - epsilon) / sampleCount) - ((Mathf.PI - epsilon) / 2);
        //    WorldSample s = Sampler.SampleFromCoord(Mathf.PI/2, lat);
        //    if(lastSample.xIndex == -1)
        //        lastSample = s;
        //    else if (Mathf.Abs(s.xIndex - lastSample.xIndex) > Sampler.MapIndexWidth() / 4)
        //        break;
        //    else if (Mathf.Abs(s.yIndex - lastSample.yIndex) > Sampler.MapIndexWidth() / 2)
        //        break;

        //    s.WorldPos.y += 1;
        //    positions.Add(s.WorldPos);
        //}

        //LineRenderer line = CreateWindCurrent(positions);
        //WindInvalidated = false;
    }

    private LineRenderer CreateWindCurrent(List<Vector3> positions )
    {
        GameObject gObject = new GameObject($"wind{windCurrents.Count}");
        windCurrents.Add(gObject);

        LineRenderer line = gObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.SetColors(windData.LineColour, windData.LineColour);
        line.SetWidth(1, 1);
        line.numCapVertices = 3;
        line.numCornerVertices = 3;
        line.positionCount = positions.Count;
        line.SetPositions(positions.ToArray());

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

    private void OnValuesUpdated()
    {
        WindInvalidated = true;
        if (!Application.isPlaying)
        {
            Start();
            LateUpdate();
        }
    }

    private void OnValidate()
    {
        if (Sampler != null && Sampler.window != null)
        {
            Sampler.window.OnValuesUpdated.RemoveListener(OnValuesUpdated);
            Sampler.window.OnValuesUpdated.AddListener(OnValuesUpdated);
        }
    }
}
