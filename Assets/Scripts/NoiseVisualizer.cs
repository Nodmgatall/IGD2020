using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NoiseVisualizer : MonoBehaviour
{
    // Start is called before the first frame update
    public NoiseManager m_noiseManager;

    Vector3 m_offsetTopDown = new Vector3(0f,0f,10.003f);
    Vector3 m_offsetTopLeft = new Vector3(8.6625f,0f, -5.0f);
    Texture2D test;
    Renderer m_renderer;
    int m_width;
    int m_height;
    public RawImage image;
    public GameObject camera;
    void Start()
    {
        m_width = m_noiseManager.m_noiseGenerator.width;
        m_height = m_noiseManager.m_noiseGenerator.width;
        test = new Texture2D(m_width, m_height);
        image.texture = test;
        m_renderer = GetComponent<Renderer>();
        m_renderer.material.mainTexture = test;
    }

    public void generateTexture(){
        for(int x = -m_width/2;x < m_width/2; x++){
            for (int y = -m_height/2; y < m_height/2; y++)
            {
                float noise;
                Vector3 pos = (m_offsetTopLeft * x) + (m_offsetTopDown * y);
                float x_c = pos.x;
                float y_c = pos.z;
                noise = m_noiseManager.getNoise(x_c,y_c);

                test.SetPixel(x + (m_width/2), y + (m_height/2), new Color(noise,noise,noise));
            }
        }
        test.Apply();
        //GetComponent<RawImage>().texture.SetPixels(noiseMap);
    }
}
