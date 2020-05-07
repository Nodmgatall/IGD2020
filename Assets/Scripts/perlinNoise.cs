using UnityEngine;
using System.Linq;
using System;
public class mapSettings{};

public class perlinNoise : MonoBehaviour
{
    public int width = 128;
    public int height  = 128;
    public float[] scales = {1.0f,1.0f,1.0f};
    public Vector2 pos;
    float offset = 100000;

    public float getNoise(float p_x, float p_y)
    {
        float x =(float) width/2.0f + (p_x + pos.x);
        float y =(float) height/2.0f + (p_y + pos.y);
        float[] f = {1f, 0.6f, 0.3f}; //Rename
        float[] g = new float[3]; //Rename
        float xCoord =(float)x / (float)width ;
        float yCoord =(float)y / (float)height  ;
        for (int i = 0; i < scales.Length; i++){
            xCoord *= scales[i] * f[i];
            yCoord *= scales[i] * f[i];
            g[i] = Mathf.PerlinNoise( offset + xCoord, offset + yCoord);
        }

        return g.Max();
    }
}
