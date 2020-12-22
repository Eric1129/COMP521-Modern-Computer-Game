using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tingyu Shen 260798146
// This class is used for generating 1d perlin noise
public class PerlinNoise
{

    // I just use add cos and sin serves as to calculate the basic noise
    float Noise(float x)
    {
        return Mathf.Cos(x) + Mathf.Sin(x);
    }
    
    float SmoothedNoise(float x)
    {
        return Noise(x) / 2 + Noise(x - 1) / 4 + Noise(x + 1) / 4;
    }

    // I use cosine function to interpolate
    // which take two neibor float with first one's fraction
    float CosInterpolate(float a, float b, float x)
    {
        float ft = x * 3.1415927f;
        float f = (1 - Mathf.Cos(ft)) * 0.5f;
        return a * (1 - f) + b * f;
    }

    // Interpolate function
    float InterpolatedNoise(float x)
    {
        int integer_X = (int) x;
        float fractional_X = x - integer_X;

        float v1 = SmoothedNoise(integer_X);
        float v2 = SmoothedNoise(integer_X + 1);

        return CosInterpolate(v1, v2, fractional_X);
    }

    // this is the function to caculate perlin noise
    // which take a float as input
    public float PerlinNoise_1D(float x)
    {
        float total = 0;
        int p = 4;
        int n = 10 - 1;
        int frequency = 2;
        float amplitude = 1f;

        for (int i = 0; i < n; i++){
            frequency = 2 ^ i;
            amplitude = p ^ i;
            total = total + InterpolatedNoise(x * frequency) * amplitude;
        }
        return total/50;
    } 
}
