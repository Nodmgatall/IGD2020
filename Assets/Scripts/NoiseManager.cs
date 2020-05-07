using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseManager : MonoBehaviour
{

    public float cutoffMin = 0;
    public float cutoffMax = 1;
    public float returnValueIfInRange = -1;
    public float returnValueIfGreaterMax = 1;
    public float returnValueIfSmallerMin = 0;
    public GameObject m_prefab;

    public perlinNoise m_noiseGenerator;
    public NoiseVisualizer m_noiseVis;

    public float getNoise(float x, float y)
    {
        float noiseValue =  m_noiseGenerator.getNoise(x,y);
        if(noiseValue <= cutoffMin)
        {
            return returnValueIfSmallerMin;
        }
        if(noiseValue >= cutoffMax)
        {
            return returnValueIfGreaterMax;
        }
        if(returnValueIfInRange > 0)
        {
            return returnValueIfInRange;
        }
        return noiseValue;
    }

    public GameObject getPrefab()
    {
        return m_prefab;
    }

    public void OnValidate()
    {
        if(m_noiseVis != null)
        {
            m_noiseVis.generateTexture();
        }
    }

    Texture2D tex;
    Renderer m_renderer;

   // void Start()
   // {
   //     if(tex == null)
   //         tex = new Texture2D(width, height);
   //     Debug.Log("Start Called");
   // }

   // void OnValidate()
   // {
   //     m_renderer = GetComponent<Renderer>();
   //     tex = new Texture2D(width, height);
   //     Debug.Log("OnValidate Called");
   //     GenTextures();
   // }

   // public void GenTextures()
   // {
   //     Debug.Log("gen textures called");
   // }


   // }
}
