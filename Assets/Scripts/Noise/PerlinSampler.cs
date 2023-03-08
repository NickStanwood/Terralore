using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PerlinSampler : NoiseSampler
{
    public PerlinSampler(NoiseData data) : base(data) { }

    protected override float SampleSingle(int seed, double x, double y)
    {

        int x0 = FastFloor(x);
        int y0 = FastFloor(y);

        float xd0 = (float)(x - x0);
        float yd0 = (float)(y - y0);
        float xd1 = xd0 - 1;
        float yd1 = yd0 - 1;

        float xs = InterpQuintic(xd0);
        float ys = InterpQuintic(yd0);

        x0 *= PrimeX;
        y0 *= PrimeY;
        int x1 = x0 + PrimeX;
        int y1 = y0 + PrimeY;

        float xf0 = Lerp(GradCoord(seed, x0, y0, xd0, yd0), GradCoord(seed, x1, y0, xd1, yd0), xs);
        float xf1 = Lerp(GradCoord(seed, x0, y1, xd0, yd1), GradCoord(seed, x1, y1, xd1, yd1), xs);

        return Normalize(Lerp(xf0, xf1, ys) * 1.4247691104677813f);
    }

    protected override float SampleSingle(int seed, double x, double y, double z)
    {
        int x0 = FastFloor(x);
        int y0 = FastFloor(y);
        int z0 = FastFloor(z);

        float xd0 = (float)(x - x0);
        float yd0 = (float)(y - y0);
        float zd0 = (float)(z - z0);
        float xd1 = xd0 - 1;
        float yd1 = yd0 - 1;
        float zd1 = zd0 - 1;

        float xs = InterpQuintic(xd0);
        float ys = InterpQuintic(yd0);
        float zs = InterpQuintic(zd0);

        x0 *= PrimeX;
        y0 *= PrimeY;
        z0 *= PrimeZ;
        int x1 = x0 + PrimeX;
        int y1 = y0 + PrimeY;
        int z1 = z0 + PrimeZ;

        float xf00 = Lerp(GradCoord(x0, y0, z0, xd0, yd0, zd0), GradCoord(x1, y0, z0, xd1, yd0, zd0), xs);
        float xf10 = Lerp(GradCoord(x0, y1, z0, xd0, yd1, zd0), GradCoord(x1, y1, z0, xd1, yd1, zd0), xs);
        float xf01 = Lerp(GradCoord(x0, y0, z1, xd0, yd0, zd1), GradCoord(x1, y0, z1, xd1, yd0, zd1), xs);
        float xf11 = Lerp(GradCoord(x0, y1, z1, xd0, yd1, zd1), GradCoord(x1, y1, z1, xd1, yd1, zd1), xs);

        float yf0 = Lerp(xf00, xf10, ys);
        float yf1 = Lerp(xf01, xf11, ys);

        return Normalize(Lerp(yf0, yf1, zs) * 0.964921414852142333984375f);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float InterpQuintic(float t) { return t * t * t * (t * (t * 6 - 15) + 10); }
}
