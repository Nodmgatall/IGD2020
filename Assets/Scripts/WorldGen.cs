using UnityEngine;
using System;
using System.Linq;
using System.Collections;

[Serializable]
public class NoiseSettings{

    [Range(0,1)]
    public float cutoffMin = 0;
    [Range(0,1)]
    public float cutoffMax = 1;
};
[Serializable]
public struct MapGrid
{
    Vector3 m_origin;

    //Offsets to next neighbour tile
    Vector3 m_offsetTopDown;
    Vector3 m_offsetTopLeft;

    int m_radius; // radius without center tile
    public float m_maxY; // max absolute scenespace Z value.
    public Vector3[] m_grid; // vector for grid positions

    int m_numHexTiles;

    public MapGrid(Vector3 p_origin, int p_radius)
    {
        int radiusWithCenter = p_radius + 1; // center tile needed to be included here

        m_origin = p_origin;
        m_radius = p_radius;

        m_offsetTopDown = new Vector3(0f,0f,10.003f);
        m_offsetTopLeft = new Vector3(8.6625f,0f, -5.0f);

        m_numHexTiles = (3 * (radiusWithCenter * radiusWithCenter)) - (3 * radiusWithCenter) + 1;
        m_grid = new Vector3[m_numHexTiles];
        m_maxY = m_radius * m_offsetTopDown.z;
    }

    public Vector3[] getGrid(){
        return m_grid;
    }

    public float getMaxY(){
        return m_maxY;
    }

    public void createHexShapedBasicGrid()
    {
        int tileID = 0;
        for (int q = -m_radius; q <= m_radius; q++) {
            int r1 =Math.Max(-m_radius, -q - m_radius);
            int r2 =Math.Min(m_radius, -q + m_radius);
            for (int r = r1; r <= r2; r++) {
                m_grid[tileID] = m_origin + q * m_offsetTopLeft + r * m_offsetTopDown + r * m_offsetTopLeft;
                m_maxY= Math.Max(m_maxY, m_grid[tileID].z);
                tileID++;
            }
        }
    }
}

// [ExecuteInEditMode]  // OUTCOMMENT IF YOU WANT THIS TO RUN IN EDITOR
public class WorldGen : MonoBehaviour
{
    // Start is called before the first frame update
    public MapGrid m_mapGrid;

    public NoiseManager[] m_vectorNoiseGens;
    public GameObject standartTile;
    GameObject m_map; // dummy object to hold generated tiles as children

    public perlinNoise m_elevationNoise;
    public perlinNoise m_forestNoise;
    public NoiseManager m_forestNoiseManager;
    public perlinNoise m_stoneNoise;
    public NoiseManager m_stoneNoiseManager;
    public AnimationCurve m_climate;

    public GameObject m_camera;
    public int radius = 12;
    public Vector3 m_origin;

    void FixedUpdate()
    {
        if(Input.GetKeyUp("l"))
        {
            Debug.Log("CalledNoise");
            LoadMap();
        }
    }
    bool invoked = false;

    void OnValidate()
    {

        if(invoked == false){
            Invoke("LoadMap",1);
            invoked = true;
        }
    }

    void Start(){
        m_mapGrid = new MapGrid( m_origin, radius);
        m_map = new GameObject();
        m_map.transform.parent  = transform; // setting parent for tile holder
    }

    public void LoadMap()
    {
        m_origin = m_camera.transform.position;
        Debug.Log("LoadMap called");
        foreach (Transform child in m_map.transform)
        {
            Destroy(child.gameObject);
        }

        m_mapGrid.createHexShapedBasicGrid();
        foreach (Vector3 v in m_mapGrid.getGrid()){
            float x = v.x + m_origin.x;
            float y = v.z + m_origin.z;
            float[] noiseValues = getNoise(x, y, m_origin);
            float height = m_elevationNoise.getNoise(x,y);
            Vector3 nV = new Vector3(v.x,(int) (height * 10 ), v.z);
            nV.x += m_origin.x;
            nV.z += m_origin.z;

            GameObject createdObject;
            GameObject toCreate = standartTile;
            if(noiseValues.Sum() > 0.001f)
            {

                int genIDX = noiseValues.ToList().IndexOf(Mathf.Max(noiseValues));
                toCreate = getPrefab(genIDX);
                if(genIDX == 2)
                {
                    float forestNoise  = m_forestNoiseManager.getNoise(x,y, ref m_forestNoise);
                    if(forestNoise > 0.0f)
                    {
                        toCreate = m_forestNoiseManager.getPrefab();
                    }
                    else
                    {
                        float stoneNoise  = m_stoneNoiseManager.getNoise(x,y, ref m_stoneNoise);
                        if(stoneNoise > 0.0f)
                        {
                            toCreate = m_stoneNoiseManager.getPrefab();
                        }
                    }
                }
                createdObject = Instantiate(toCreate,nV, Quaternion.identity);
            }
            else{
                nV.y = 0;
                createdObject = Instantiate(toCreate,nV, Quaternion.identity);
            }
            createdObject.transform.parent = m_map.transform;
            invoked = false;
        }
    }
    float[] getNoise(float x, float y, Vector3 chunkOrigin){
        float[] generatedNoiseValues = new float[m_vectorNoiseGens.Length];
        for (int i = 0; i < m_vectorNoiseGens.Length; i++)
        {
            if (m_vectorNoiseGens[i] != null){
                NoiseManager p = m_vectorNoiseGens[i];
                generatedNoiseValues[i] = p.getNoise(x + chunkOrigin.x,y + chunkOrigin.z, ref   m_elevationNoise);
            }
        }
        string str = "";
        foreach (var a in generatedNoiseValues)
        {
            str += a + " ";
        }
        //Debug.Log(str);
        return generatedNoiseValues;

    }


    GameObject getPrefab(int p_vectorIDxTile)
    {
        if(m_vectorNoiseGens[p_vectorIDxTile] != null) //dont use unset prefabs
            return m_vectorNoiseGens[p_vectorIDxTile].getPrefab();
        Debug.Log("Error: prefab not set"); //print error
        return m_vectorNoiseGens[0].getPrefab(); // return the first (hopefully at least that one is set) <<< BAD!
    }

}
