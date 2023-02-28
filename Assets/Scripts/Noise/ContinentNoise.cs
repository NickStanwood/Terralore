//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[System.Serializable]
//public class ContinentNoise
//{
//    public int Seed;

//    [Range(1000f, 60000f)]
//    public float Scale;         //the larger the value the slower the noise changes

//    [Range(1f, 30)]
//    public int MaxOctaves;      //how many iterations of noise

//    [Range(0.000f, 1.0f)]
//    public float Persistence;   //how fast the amplitude decreases with each octave

//    [Range(1.0f, 10.0f)]
//    public float Lacunarity;    //how fast the scale of each octave decreases

//    public float[,] GetNoise(int width, int height, NoiseWindow window)
//    {
//        int[] seedOffsets = Noise.GetOctaveOffsets(Seed, MaxOctaves);

//        float scale = Scale;
//        List<NoiseOctaveLayer> layers = new List<NoiseOctaveLayer>();
//        for (int octave = 0; octave < MaxOctaves; octave++)
//        {
//            int resolutionX = GetOctaveArrayWidth(scale, window);
//            int resolutionY = GetOctaveArrayHeight(scale, window);
//            if (resolutionX > width || resolutionY > height)
//            {
//                //the resolution of this octave is too detailed to fit into the desired width/height array
//                break;
//            }

//            NoiseOctaveLayer noiseLayer = GetOctaveLayer(scale, window, seedOffsets[octave]);
//            noiseLayer.DebugPrint();
//            layers.Add(noiseLayer);

//            scale /= Lacunarity;
//        }   

//        float max = 0.0f;
//        float min = float.MaxValue;
//        float[,] noiseMap = new float[width, height];
//        for (int y = 0; y < height; y++)
//        {
//            for (int x = 0; x < width; x++)
//            {
//                float amp = 1.0f;
//                float noiseVal = 0.0f;

//                foreach(NoiseOctaveLayer noiseLayer in layers)
//                {
//                    float sampleX = ((float)x / (float)width) * window.Width + window.X;
//                    float sampleY = ((float)y / (float)height) * window.Height + window.Y;
//                    float value = noiseLayer.Sample(sampleX, sampleY);
//                    noiseVal += value * amp;

//                    amp *= Persistence;

//                }

//                if (noiseVal > max)
//                    max = noiseVal;

//                if (noiseVal < min)
//                    min = noiseVal;

//                noiseMap[x, y] = noiseVal;
//            }
//        }
//        Debug.Log($"max noise value: {max}, min noise value: {min}");
//        return Noise.Normalize(noiseMap, max, min);
//    }

//    private NoiseOctaveLayer GetOctaveLayer(float scale, NoiseWindow window, int octaveNoiseOffset)
//    {
//        int sampleWidth = GetOctaveArrayWidth(scale, window);
//        int sampleHeight = GetOctaveArrayHeight(scale, window);
//        float[,] samples = Noise.GenerateNoiseMap(sampleWidth, sampleHeight, octaveNoiseOffset, octaveNoiseOffset, scale);

//        float startX = window.X / scale;
//        float startY = window.Y / scale;
//        return new NoiseOctaveLayer(startX, startY, scale, samples);
//    }

//    private int GetOctaveArrayWidth(float scale, NoiseWindow window)
//    {
//        float startX = window.X / scale;
//        float endX   = (window.X + window.Width) / scale;
//        return (int)(endX - startX) + 1;
//    }

//    private int GetOctaveArrayHeight(float scale, NoiseWindow window)
//    {
//        float startY = window.Y / scale;
//        float endY = (window.Y + window.Height) / scale;
//        return (int)(endY - startY) + 1;
//    }

//    private struct NoiseOctaveLayer
//    {
//        float StartX;
//        float StartY;
//        float Scale;
//        float[,] Noise;

//        public NoiseOctaveLayer(float startX, float startY, float scale, float[,] noise)
//        {
//            StartX = startX;
//            StartY = startY;
//            Scale = scale;
//            Noise = noise;
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="x">absolute coordinate</param>
//        /// <param name="y">absolute coordinate</param>
//        /// <returns></returns>
//        public float Sample(float x, float y)
//        {
//            int xIndex = (int)((x - StartX) / Scale);
//            int yIndex = (int)((y - StartY) / Scale);
//            //Debug.Log($"[{x},{y}] -> [{xIndex},{yIndex}]");

//            return Noise[xIndex, yIndex];
//        }

//        public int Width()
//        {
//            return Noise.GetLength(0);
//        }
//        public int Height()
//        {
//            return Noise.GetLength(1);
//        }

//        public void DebugPrint()
//        {
//            string value = $"== Octave of scale {Scale} ==\n";
//            for (int y = 0; y < Height(); y++)
//            {
//                string row = "[";
//                for (int x = 0; x < Width(); x++)
//                {
//                    row += Noise[x, y] + ", ";
//                }
//                row += "]\n";
//                value += row;
//            }
//            Debug.Log(value);
//        }
//    }
//}
