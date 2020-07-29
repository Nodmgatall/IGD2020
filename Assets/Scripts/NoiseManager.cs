using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class NoiseManager
{
    public Tile m_prefab;
    public NoiseSettings m_noiseSettings;

    public float getNoise(float x, float y,ref perlinNoise p_noiseGenerator)
    {
        float noiseValue =  p_noiseGenerator.getNoise(x,y);
        if(noiseValue <= m_noiseSettings.cutoffMin || noiseValue >= m_noiseSettings.cutoffMax)
        {
            return 0;
        }
        return noiseValue;
    }

    public Tile getPrefab()
    {
        return m_prefab;
    }

}
