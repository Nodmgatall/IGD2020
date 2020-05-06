using UnityEngine;
using System.Linq;
using System;
public class mapSettings{};

public class perlinNoise : MonoBehaviour
{
    public int width = 128;
    public int height  = 128;
    public float[] scales = {1,1,1};
    public float cutoffMin = 0;
    public float cutoffMax = 1;
    public float zoom;
    public Vector2 pos;
    Texture2D tex;
    Texture2D anither;
    Renderer m_renderer;

    void Start()
    {
        if(tex == null)
            tex = new Texture2D(width, height);
        Debug.Log("Start Called");
    }

    void OnValidate()
    {
        m_renderer = GetComponent<Renderer>();
        tex = new Texture2D(width, height);
        Debug.Log("OnValidate Called");
        GenTextures();
    }

    public void GenTextures()
    {
        Debug.Log("gen textures called");
    }

    Texture2D generateTexture(){
        float[,] noise = new float[width, height];
        for(int x = 0; x < width; x++){
            for (int y = 0; y < height; y++)
            {
                noise[x,y] = getNoise(x,y,new Vector3(0,0,0));
                tex.SetPixel(x,y,new Color(noise[x,y], noise[x,y] , noise[x,y]));
            }
        }
        tex.Apply();
        return tex;

    }
    public float getNoise(float x, float y, Vector3 chunkOrigin)
    {
        x = width/2.0f + x + pos.x;
        y = height/2.0f + y + pos.y;
        float[] f = {0.6f, 0.3f, 0.1f}; //Rename
        float sample = 0.0f;
        for (int i = 0; i < scales.Length; i++){
            float xCoord =(float)x / (float)width * scales[i];
            float yCoord =(float)y / (float)height *scales[i];
            sample += f[i] * Mathf.PerlinNoise(scales[i] + xCoord, scales[i] + yCoord);
        }
        if(sample < cutoffMin)
        {
            return 0.0f;
        }
        if(sample > cutoffMax)
        {
            return 1.0f;
        }
        return sample;
    }
}
