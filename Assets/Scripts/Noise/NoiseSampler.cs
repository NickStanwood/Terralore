using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class NoiseSampler
{
    protected NoiseData NoiseData;

    /// <summary>
    /// abstract class to sample various types of noise
    /// </summary>
    public NoiseSampler(NoiseData data)
    {
        NoiseData = data;
    }

    /// <summary>
    /// Sample a 2D noise field at loaction (x,y)
    /// </summary>
    /// <param name="x">x Coordinate</param>
    /// <param name="y">y Coordinate</param>
    /// <returns>A noise value between 0 and 1</returns>
    public float Sample(double x, double y)
    {
        int seed = NoiseData.Seed;
        float sum = 0f;
        float max = 0f;
        float amp = 1.0f;
        double freq = NoiseData.Frequency;

        for (int i = 0; i < NoiseData.Octaves; i++)
        {
            float noise = SampleSingle(seed++, x*freq, y*freq)*amp;

            sum += noise;
            max += amp;

            amp *= NoiseData.Persistence;
            freq *= NoiseData.Lacunarity;
        }

        float noiseVal = sum / max;

        if (NoiseData.AttenuationCurve != null)
            noiseVal *= NoiseData.AttenuationCurve.Evaluate((float)x, (float)y);

        return noiseVal;
    }

    /// <summary>
    /// Sample a 3D noise field at loaction (x,y,z)
    /// </summary>
    /// <param name="x">x Coordinate</param>
    /// <param name="y">y Coordinate</param>
    /// <param name="z">z Coordinate</param>
    /// <returns>A noise value between 0 and 1</returns>
    public float Sample(double x, double y, double z)
    {
        int seed = NoiseData.Seed;
        float sum = 0f;
        float max = 0f;
        float amp = 1.0f;
        double freq = NoiseData.Frequency;

        for (int i = 0; i < NoiseData.Octaves; i++)
        {
            float noise = SampleSingle(seed++, x*freq, y*freq, z*freq)*amp;

            sum += noise;
            max += amp;

            amp *= NoiseData.Persistence;
            freq *= NoiseData.Lacunarity;
        }

        float noiseVal = sum / max;

        if (NoiseData.AttenuationCurve != null)
            noiseVal *= NoiseData.AttenuationCurve.Evaluate((float)x, (float)y, (float)z);

        return noiseVal;
    }

    /// <summary>
    /// gets a noise value between 0-1
    /// </summary>
    /// <param name="seed"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    protected abstract float SampleSingle(int seed, double x, double y);

    /// <summary>
    /// gets a noise value between 0-1
    /// </summary>
    /// <param name="seed"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    protected abstract float SampleSingle(int seed, double x, double y, double z);



    ////////////////////////////////// helper functions ///////////////////////////////////////
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double FastMin(double a, double b) { return a < b ? a : b; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double FastMax(double a, double b) { return a > b ? a : b; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int FastFloor(double f) { return f >= 0 ? (int)f : (int)f - 1; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int FastRound(double f) { return f >= 0 ? (int)(f + 0.5f) : (int)(f - 0.5f); }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static float Lerp(float a, float b, float t) { return a + t * (b - a); }

    protected static float Normalize(float f) { return (f + 1.0f) / 2.0f; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected float GradCoord(int seed, int xPrimed, int yPrimed, float xd, float yd)
    {
        int hash = Hash(seed, xPrimed, yPrimed);
        hash ^= hash >> 15;
        hash &= 127 << 1;

        float xg = Gradients2D[hash];
        float yg = Gradients2D[hash | 1];

        return xd * xg + yd * yg;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected float GradCoord(int xPrimed, int yPrimed, int zPrimed, float xd, float yd, float zd)
    {
        int hash = Hash(xPrimed, yPrimed, zPrimed);
        hash ^= hash >> 15;
        hash &= 63 << 2;

        float xg = Gradients3D[hash];
        float yg = Gradients3D[hash | 1];
        float zg = Gradients3D[hash | 2];

        return xd * xg + yd * yg + zd * zg;
    }
    // Hashing
    protected const int PrimeX = 501125321;
    protected const int PrimeY = 1136930381;
    protected const int PrimeZ = 1720413743;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Hash(int xPrimed, int yPrimed)
    {
        int hash = NoiseData.Seed ^ xPrimed ^ yPrimed;

        hash *= 0x27d4eb2d;
        return hash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Hash(int xPrimed, int yPrimed, int zPrimed)
    {
        int hash = NoiseData.Seed ^ xPrimed ^ yPrimed ^ zPrimed;

        hash *= 0x27d4eb2d;
        return hash;
    }

    /// ///////////////////////////////////////////////// ARRAYS /////////////////////////////////////////////////
    private static readonly float[] Gradients2D =
    {
         0.130526192220052f,  0.99144486137381f,   0.38268343236509f,   0.923879532511287f,  0.608761429008721f,  0.793353340291235f,  0.793353340291235f,  0.608761429008721f,
         0.923879532511287f,  0.38268343236509f,   0.99144486137381f,   0.130526192220051f,  0.99144486137381f,  -0.130526192220051f,  0.923879532511287f, -0.38268343236509f,
         0.793353340291235f, -0.60876142900872f,   0.608761429008721f, -0.793353340291235f,  0.38268343236509f,  -0.923879532511287f,  0.130526192220052f, -0.99144486137381f,
        -0.130526192220052f, -0.99144486137381f,  -0.38268343236509f,  -0.923879532511287f, -0.608761429008721f, -0.793353340291235f, -0.793353340291235f, -0.608761429008721f,
        -0.923879532511287f, -0.38268343236509f,  -0.99144486137381f,  -0.130526192220052f, -0.99144486137381f,   0.130526192220051f, -0.923879532511287f,  0.38268343236509f,
        -0.793353340291235f,  0.608761429008721f, -0.608761429008721f,  0.793353340291235f, -0.38268343236509f,   0.923879532511287f, -0.130526192220052f,  0.99144486137381f,
         0.130526192220052f,  0.99144486137381f,   0.38268343236509f,   0.923879532511287f,  0.608761429008721f,  0.793353340291235f,  0.793353340291235f,  0.608761429008721f,
         0.923879532511287f,  0.38268343236509f,   0.99144486137381f,   0.130526192220051f,  0.99144486137381f,  -0.130526192220051f,  0.923879532511287f, -0.38268343236509f,
         0.793353340291235f, -0.60876142900872f,   0.608761429008721f, -0.793353340291235f,  0.38268343236509f,  -0.923879532511287f,  0.130526192220052f, -0.99144486137381f,
        -0.130526192220052f, -0.99144486137381f,  -0.38268343236509f,  -0.923879532511287f, -0.608761429008721f, -0.793353340291235f, -0.793353340291235f, -0.608761429008721f,
        -0.923879532511287f, -0.38268343236509f,  -0.99144486137381f,  -0.130526192220052f, -0.99144486137381f,   0.130526192220051f, -0.923879532511287f,  0.38268343236509f,
        -0.793353340291235f,  0.608761429008721f, -0.608761429008721f,  0.793353340291235f, -0.38268343236509f,   0.923879532511287f, -0.130526192220052f,  0.99144486137381f,
         0.130526192220052f,  0.99144486137381f,   0.38268343236509f,   0.923879532511287f,  0.608761429008721f,  0.793353340291235f,  0.793353340291235f,  0.608761429008721f,
         0.923879532511287f,  0.38268343236509f,   0.99144486137381f,   0.130526192220051f,  0.99144486137381f,  -0.130526192220051f,  0.923879532511287f, -0.38268343236509f,
         0.793353340291235f, -0.60876142900872f,   0.608761429008721f, -0.793353340291235f,  0.38268343236509f,  -0.923879532511287f,  0.130526192220052f, -0.99144486137381f,
        -0.130526192220052f, -0.99144486137381f,  -0.38268343236509f,  -0.923879532511287f, -0.608761429008721f, -0.793353340291235f, -0.793353340291235f, -0.608761429008721f,
        -0.923879532511287f, -0.38268343236509f,  -0.99144486137381f,  -0.130526192220052f, -0.99144486137381f,   0.130526192220051f, -0.923879532511287f,  0.38268343236509f,
        -0.793353340291235f,  0.608761429008721f, -0.608761429008721f,  0.793353340291235f, -0.38268343236509f,   0.923879532511287f, -0.130526192220052f,  0.99144486137381f,
         0.130526192220052f,  0.99144486137381f,   0.38268343236509f,   0.923879532511287f,  0.608761429008721f,  0.793353340291235f,  0.793353340291235f,  0.608761429008721f,
         0.923879532511287f,  0.38268343236509f,   0.99144486137381f,   0.130526192220051f,  0.99144486137381f,  -0.130526192220051f,  0.923879532511287f, -0.38268343236509f,
         0.793353340291235f, -0.60876142900872f,   0.608761429008721f, -0.793353340291235f,  0.38268343236509f,  -0.923879532511287f,  0.130526192220052f, -0.99144486137381f,
        -0.130526192220052f, -0.99144486137381f,  -0.38268343236509f,  -0.923879532511287f, -0.608761429008721f, -0.793353340291235f, -0.793353340291235f, -0.608761429008721f,
        -0.923879532511287f, -0.38268343236509f,  -0.99144486137381f,  -0.130526192220052f, -0.99144486137381f,   0.130526192220051f, -0.923879532511287f,  0.38268343236509f,
        -0.793353340291235f,  0.608761429008721f, -0.608761429008721f,  0.793353340291235f, -0.38268343236509f,   0.923879532511287f, -0.130526192220052f,  0.99144486137381f,
         0.130526192220052f,  0.99144486137381f,   0.38268343236509f,   0.923879532511287f,  0.608761429008721f,  0.793353340291235f,  0.793353340291235f,  0.608761429008721f,
         0.923879532511287f,  0.38268343236509f,   0.99144486137381f,   0.130526192220051f,  0.99144486137381f,  -0.130526192220051f,  0.923879532511287f, -0.38268343236509f,
         0.793353340291235f, -0.60876142900872f,   0.608761429008721f, -0.793353340291235f,  0.38268343236509f,  -0.923879532511287f,  0.130526192220052f, -0.99144486137381f,
        -0.130526192220052f, -0.99144486137381f,  -0.38268343236509f,  -0.923879532511287f, -0.608761429008721f, -0.793353340291235f, -0.793353340291235f, -0.608761429008721f,
        -0.923879532511287f, -0.38268343236509f,  -0.99144486137381f,  -0.130526192220052f, -0.99144486137381f,   0.130526192220051f, -0.923879532511287f,  0.38268343236509f,
        -0.793353340291235f,  0.608761429008721f, -0.608761429008721f,  0.793353340291235f, -0.38268343236509f,   0.923879532511287f, -0.130526192220052f,  0.99144486137381f,
         0.38268343236509f,   0.923879532511287f,  0.923879532511287f,  0.38268343236509f,   0.923879532511287f, -0.38268343236509f,   0.38268343236509f,  -0.923879532511287f,
        -0.38268343236509f,  -0.923879532511287f, -0.923879532511287f, -0.38268343236509f,  -0.923879532511287f,  0.38268343236509f,  -0.38268343236509f,   0.923879532511287f,
    };

    private static readonly float[] Gradients3D =
    {
            0, 1, 1, 0,  0,-1, 1, 0,  0, 1,-1, 0,  0,-1,-1, 0,
            1, 0, 1, 0, -1, 0, 1, 0,  1, 0,-1, 0, -1, 0,-1, 0,
            1, 1, 0, 0, -1, 1, 0, 0,  1,-1, 0, 0, -1,-1, 0, 0,
            0, 1, 1, 0,  0,-1, 1, 0,  0, 1,-1, 0,  0,-1,-1, 0,
            1, 0, 1, 0, -1, 0, 1, 0,  1, 0,-1, 0, -1, 0,-1, 0,
            1, 1, 0, 0, -1, 1, 0, 0,  1,-1, 0, 0, -1,-1, 0, 0,
            0, 1, 1, 0,  0,-1, 1, 0,  0, 1,-1, 0,  0,-1,-1, 0,
            1, 0, 1, 0, -1, 0, 1, 0,  1, 0,-1, 0, -1, 0,-1, 0,
            1, 1, 0, 0, -1, 1, 0, 0,  1,-1, 0, 0, -1,-1, 0, 0,
            0, 1, 1, 0,  0,-1, 1, 0,  0, 1,-1, 0,  0,-1,-1, 0,
            1, 0, 1, 0, -1, 0, 1, 0,  1, 0,-1, 0, -1, 0,-1, 0,
            1, 1, 0, 0, -1, 1, 0, 0,  1,-1, 0, 0, -1,-1, 0, 0,
            0, 1, 1, 0,  0,-1, 1, 0,  0, 1,-1, 0,  0,-1,-1, 0,
            1, 0, 1, 0, -1, 0, 1, 0,  1, 0,-1, 0, -1, 0,-1, 0,
            1, 1, 0, 0, -1, 1, 0, 0,  1,-1, 0, 0, -1,-1, 0, 0,
            1, 1, 0, 0,  0,-1, 1, 0, -1, 1, 0, 0,  0,-1,-1, 0
    };
}
