using UnityEngine;
using System.Linq;
using System;
public class mapSettings{};


[Serializable]
public class perlinNoise
{
    [HideInInspector]
    public int width = 128;
    [HideInInspector]
    public int height  = 128;
    [HideInInspector]
    public Vector2 pos;
    [HideInInspector]
    float offset = 100000;
    public float roughness = 0.03f;
    public float strength = 1;
    public float nheight = 0;

    public float getNoise(float p_x, float p_y)
    {
        float g = Mathf.PerlinNoise( offset + (p_x *roughness) , offset +  (p_y * roughness) );

        return (g * strength) + nheight;
    }
}
