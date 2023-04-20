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
        if (!WindInvalidated || !Application.isPlaying)
            return;

        DestroyWindCurrents();

        if (!windData.ShowWindCurrents)
            return;

        List<Vector3> knots = new List<Vector3>();
        int xSampleFreq = Mathf.Max(Sampler.MapIndexWidth() / WindCurrentColumns, 1);
        int ySampleFreq = Mathf.Max(Sampler.MapIndexHeight() / WindCurrentRows, 1);
        //for (int x = 0; x < WindCurrentColumns; x++)
        //{
        //    for(int y = 0; y < WindCurrentRows; y++)
        //    {
        //        WorldSample s = Sampler.SampleFromIndex(x * xSampleFreq + xSampleFreq/2, y * ySampleFreq + ySampleFreq/2);
        //        knots = CalculateWindCurrentKnots(s.Longitude, s.Latitude);
        //        CreateWindCurrent(knots);
        //    }
        //}

        //=== DEBUGGING
        //int x = 0;
        //int y = 0;
        //WorldSample s = Sampler.SampleFromIndex(x * xSampleFreq + xSampleFreq / 2, y * ySampleFreq + ySampleFreq / 2);
        knots = CalculateWindCurrentKnots(-Mathf.PI + 0.001f, Mathf.PI/2 - 0.001f);
        CreateWindCurrent(knots);
        // ===

        WindInvalidated = false;
    }

    void OnDestroy()
    {
        DestroyWindCurrents();
    }

    private List<Vector3> CalculateWindCurrentKnots(float lon, float lat)
    {
        List<Vector3> knots = new List<Vector3>();
        int knotCount = 2;
        string debugStr = "";
        for (int i = 0; i < knotCount; i++)
        {
            WorldSample s = Sampler.SampleFromCoord(lon, lat);
            s.WorldPos.y += 3;
            knots.Add(s.WorldPos);
            int lonDeg = (int)(lon * 180 / Mathf.PI);
            int latDeg = (int)(lat * 180 / Mathf.PI);
            debugStr += $"({lonDeg},{latDeg}) -> ";
            Vector2 coord = GetNextWindCurrentKnot(lon, lat);
            lon = coord.x;
            lat = coord.y;
        }
        Debug.Log(debugStr);
        return knots;
    }

    private Vector2 GetNextWindCurrentKnot(float lon, float lat)
    {
        Vector2 coord = new Vector2(lon, lat);
        float scale = Mathf.PI/60;
        const float outerBand = Mathf.PI / 3;
        const float innerBand = Mathf.PI / 6;
        if (lat > outerBand)
        {
            coord.x -= Mathf.Sin(lat - outerBand) * scale;
            coord.y -= Mathf.Cos(lat - outerBand) * scale;
        }
        else if (lat > innerBand)
        {
            coord.x += Mathf.Sin(lat - innerBand) * scale;
            coord.y += Mathf.Cos(lat - innerBand) * scale;
        }
        else if (lat >= 0)
        {
            coord.x -= Mathf.Sin(lat) * scale;
            coord.y -= Mathf.Cos(lat) * scale;
        }
        else if (lat > -innerBand)
        {
            coord.x -= Mathf.Sin(lat + innerBand) * scale;
            coord.y += Mathf.Cos(lat + innerBand) * scale;
        }
        else if (lat > -outerBand)
        {
            coord.x += Mathf.Sin(lat + outerBand) * scale;
            coord.y -= Mathf.Cos(lat + outerBand) * scale;
        }
        else
        {
            coord.x -= Mathf.Sin(lat + Mathf.PI / 2) * scale;
            coord.y += Mathf.Cos(lat + Mathf.PI / 2) * scale;
        }
        return coord;
    }

    private LineRenderer CreateWindCurrent(List<Vector3> positions)
    {
        GameObject gObject = new GameObject($"wind{windCurrents.Count}");
        windCurrents.Add(gObject);

        LineRenderer line = gObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.SetColors(windData.StartColour, windData.EndColour);
        line.SetWidth(5, 5);
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
