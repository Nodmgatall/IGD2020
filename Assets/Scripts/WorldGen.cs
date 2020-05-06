using UnityEngine;
using System;
using System.Linq;
using System.Collections;

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

        m_radius = p_radius;

        m_origin = p_origin;

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
    MapGrid m_mapGrid;

    public GameObject[] m_vectorNoiseGens = new GameObject[6];
    public GameObject[] m_vectorPrefabs = new GameObject[6];
    GameObject m_map; // dummy object to hold generated tiles as children

    public GameObject m_elevationNoise;
    public AnimationCurve m_climate;

    public int radius = 12;
    public Vector3 m_origin;

    void Start(){
        m_map = new GameObject();
        m_map.transform.parent  = transform; // setting parent for tile holder
    }
    public void LoadMap()
    {
        Debug.Log("LoadMap called");
        foreach (Transform child in m_map.transform)
        {
            Destroy(child.gameObject);
        }
        m_mapGrid = new MapGrid( m_origin, radius);
        m_mapGrid.createHexShapedBasicGrid();
        foreach (Vector3 v in m_mapGrid.getGrid()){
            float x = v.x;
            float y = v.z;
            float[] noiseValues = getNoise(x, y, m_origin);
            int tileIDx = getTileIDXFromNoise(noiseValues,x,y);
            var createdObject = Instantiate(getPrefab(tileIDx),v, Quaternion.identity);
            createdObject.transform.parent = m_map.transform;

        }
    }
    float[] getNoise(float x, float y, Vector3 chunkOrigin){
        float[] generatedNoiseValues = new float[m_vectorNoiseGens.Length];
        for (int i = 0; i < m_vectorNoiseGens.Length; i++)
        {
            if (m_vectorNoiseGens[i] != null){
                perlinNoise p = m_vectorNoiseGens[i].GetComponent(typeof(perlinNoise)) as perlinNoise;
                generatedNoiseValues[i] = p.getNoise(x,y, chunkOrigin);
            }
        }
        return generatedNoiseValues;

    }

    int getTileIDXFromNoise(float [] noiseValues,float p_tileXCoord, float p_tileYCoord)
    {
        int vectorIdxOfNewTile =  0;

        perlinNoise heightNoise = m_elevationNoise.GetComponent(typeof(perlinNoise)) as perlinNoise;

        float heigtAtXY = heightNoise.getNoise(p_tileXCoord,p_tileYCoord,m_origin);
        float toEval = Math.Abs(p_tileYCoord /(m_mapGrid.getMaxY()));
        float climateZoneValue = m_climate.Evaluate(toEval);


        //CLIMATE START
        if(climateZoneValue > 0.95)
        {
            //return "sand"
            vectorIdxOfNewTile =  4;
        }
        else if(climateZoneValue > 0.7)
        {
            //vectorIdxOfNewTile =  Grass
            vectorIdxOfNewTile =  5;
        }
        else if(climateZoneValue > 0.55)
        {
            //vectorIdxOfNewTile =  Forest
            vectorIdxOfNewTile =  1;
        }
        else if(climateZoneValue > 0.3)
        {
            //vectorIdxOfNewTile =  Grass
            vectorIdxOfNewTile =  5;
        }
        //CLIMATE END

        //DETAILS rocks mountains, ponds
        if(vectorIdxOfNewTile != 0)
        {
            //vectorIdxOfNewTile = noiseValues.ToList().IndexOf(noiseValues.Max());
            if(noiseValues[1] > 0.0f)
            {
                vectorIdxOfNewTile = 2;
            }
            if(noiseValues[2] > 0.0f)
            {
                vectorIdxOfNewTile = 3;
            }
            if(noiseValues[0] > 0.0f)
            {
                vectorIdxOfNewTile = 0;
            }
        }
        return vectorIdxOfNewTile;
    }

    GameObject getPrefab(int p_vectorIDxTile)
    {
        if(m_vectorPrefabs[p_vectorIDxTile] != null) //dont use unset prefabs
            return m_vectorPrefabs[p_vectorIDxTile];
        Debug.Log("Error: prefab not set"); //print error
        return m_vectorPrefabs[0]; // return the first (hopefully at least that one is set) <<< BAD!
    }
}
