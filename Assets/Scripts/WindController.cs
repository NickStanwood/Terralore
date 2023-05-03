using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{
    public WindData windData;
    public WorldSampler Sampler;
    public bool DisplayWind;
    public int WindCurrentColumns = 10;
    public int WindCurrentRows = 4;
    public int KnotCount = 10;

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
        if (!WindInvalidated || !DisplayWind)
            return;

        DestroyWindCurrents();

        if (!windData.ShowWindCurrents)
            return;

        List<Vector3> knots = new List<Vector3>();
        int xSampleFreq = Mathf.Max(Sampler.MapIndexWidth() / WindCurrentColumns, 1);
        int ySampleFreq = Mathf.Max(Sampler.MapIndexHeight() / WindCurrentRows, 1);
        for (int x = 0; x < WindCurrentColumns; x++)
        {
            for (int y = 0; y < WindCurrentRows; y++)
            {
                int xIndex = x * xSampleFreq + xSampleFreq / 2;
                int yIndex = (y % 2 == 0)
                                ?  y      * ySampleFreq + 4
                                : (y + 1) * ySampleFreq - 4 ;
                WorldSample s = Sampler.SampleFromIndex(xIndex, yIndex);
                knots = CalculateWindCurrentKnots(s.Longitude, s.Latitude);
                CreateWindCurrent(knots);
            }
        }

        WindInvalidated = false;
    }

    void OnDestroy()
    {
        DestroyWindCurrents();
    }

    private List<Vector3> CalculateWindCurrentKnots(float lon, float lat)
    {
        List<Vector3> knots = new List<Vector3>();
        WorldSample lastSample = WorldSample.Empty;
        for (int i = 0; i < KnotCount; i++)
        {
            WorldSample s = Sampler.SampleFromCoord(lon, lat);
            s.WorldPos.y += 1;

            if (lastSample.IsEmpty())
                lastSample = s;
            else if (Mathf.Abs(s.xIndex - lastSample.xIndex) > (Sampler.MapIndexWidth() - 1)/ 2)
                break;

            knots.Add(s.WorldPos);

            Vector2 coord = GetNextWindCurrentKnot(lon, lat);
            lon = coord.x;
            lat = coord.y;
        }
        return knots;
    }

    private Vector2 GetNextWindCurrentKnot(float lon, float lat)
    {
        Vector2 coord = new Vector2(lon, lat);
        float scale = Mathf.PI/60;
        const float outerBand = Mathf.PI / 3;
        const float innerBand = Mathf.PI / 6;
        const float bandSize = Mathf.PI / 6;
        const float bandAngleScale = Mathf.PI / (2 * bandSize);
        if (lat > outerBand)
        {
            float angle = (lat - outerBand) * bandAngleScale;
            coord.x -= Mathf.Cos(angle) * scale;
            coord.y -= Mathf.Sin(angle) * scale;
        }
        else if (lat > innerBand)
        {
            float angle = (lat - innerBand) * bandAngleScale;
            coord.x += Mathf.Sin(angle) * scale;
            coord.y += Mathf.Cos(angle) * scale;
        }
        else if (lat >= 0)
        {
            float angle = (lat) * bandAngleScale;
            coord.x -= Mathf.Cos(angle) * scale;
            coord.y -= Mathf.Sin(angle) * scale;
        }
        else if (lat > -innerBand)
        {
            float angle = (lat + innerBand) * bandAngleScale;
            coord.x -= Mathf.Sin(angle) * scale;
            coord.y += Mathf.Cos(angle) * scale;
        }
        else if (lat > -outerBand)
        {
            float angle = (lat + outerBand) * bandAngleScale;
            coord.x += Mathf.Cos(angle) * scale;
            coord.y -= Mathf.Sin(angle) * scale;
        }
        else
        {
            float angle = (lat + Mathf.PI / 2) * bandAngleScale;
            coord.x -= Mathf.Sin(angle) * scale;
            coord.y += Mathf.Cos(angle) * scale;
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
        line.SetWidth(2, 2);
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
        if (Sampler != null)
        {
            Sampler.OnValuesUpdated.RemoveListener(OnValuesUpdated);
            Sampler.OnValuesUpdated.AddListener(OnValuesUpdated);
        }
    }
}
