using UnityEngine;
using System;
using System.Linq;
using System.Collections;

public struct MapGrid
{
    Vector3 origin;
    Vector3 offsetTopDown;
    Vector3 offestTopLeft;
    int m_radius;
    public float maxY;
    public Vector3[] grid;

    int numHexTiles;

    public MapGrid(Vector3 p_origin, int p_radius)
    {
        m_radius = p_radius;
        p_radius = p_radius + 1;
        origin = p_origin;

        origin = p_origin;
        offsetTopDown= new Vector3(0f,0f,10.003f);
        offestTopLeft= new Vector3(8.6625f,0f, -5.0f);
        numHexTiles = (3 * (p_radius * p_radius)) - (3 * p_radius) + 1;
        grid = new Vector3[numHexTiles];
        maxY = m_radius * offsetTopDown.z;
    }

    public void createHexShapedBasicGrid()
    {

        int p_radius = m_radius;
        int tileID = 0;
        for (int q = -p_radius; q <= p_radius; q++) {
            int r1 =Math.Max(-p_radius, -q - p_radius);
            int r2 =Math.Min(p_radius, -q + p_radius);
            for (int r = r1; r <= r2; r++) {
                grid[tileID] = origin + q * offestTopLeft + r * offsetTopDown + r * offestTopLeft;
                maxY= Math.Max(maxY, grid[tileID].z);
                tileID++;
            }
        }
    }
}

public class WorldGen : MonoBehaviour
{
    // Start is called before the first frame update
    MapGrid map_grid;

    public GameObject[] noiseGens = new GameObject[6];
    public GameObject[] prefabs = new GameObject[6];
    GameObject map;

    public GameObject elevationNoise;
    public AnimationCurve climate;

    float[] noiseVals;

    float maxYY = 0;
    public int radius = 12;
    public Vector3 origin;
    void Start(){
        map = new GameObject();
        map.transform.parent  = transform;
    }
    public void LoadMap()
    {
        Debug.Log("LoadMap called");
        foreach (Transform child in map.transform)
        {
            Destroy(child.gameObject);
        }
        map_grid = new MapGrid( origin, radius);
        map_grid.createHexShapedBasicGrid();
        foreach (Vector3 v in map_grid.grid){
            float x = v.x;
            float y = v.z;
            float[] noiseVals = getNoise(x, y, origin);
            int tileIDx = getTileIDXFromNoise(noiseVals,x,y);
            var obj = Instantiate(getPrefab(tileIDx),v, Quaternion.identity);
            obj.transform.parent = map.transform;

        }
    }
    float[] getNoise(float x, float y, Vector3 chunkOrigin){
        float[] localNoiseVals = new float[noiseGens.Length];
        for (int i = 0; i < noiseGens.Length; i++)
        {
            if (noiseGens[i] != null){
                perlinNoise p = noiseGens[i].GetComponent(typeof(perlinNoise)) as perlinNoise;
                localNoiseVals[i] = p.getNoise(x,y, chunkOrigin);
            }
        }
        return localNoiseVals;

    }

    bool printOnce = true;
    int getTileIDXFromNoise(float [] noiseValues,float x, float y)
    {
        int retVal =  0;
        perlinNoise heightNoise = elevationNoise.GetComponent(typeof(perlinNoise)) as perlinNoise;
        float heigtAtXY = heightNoise.getNoise(x,y,origin);
        float toEval = Math.Abs(y /(map_grid.maxY));
        float climateValues = climate.Evaluate(toEval);
        maxYY =Math.Max( climateValues, maxYY);
        if(printOnce)
        {
            Debug.Log(toEval);
            Debug.Log(x);
            Debug.Log(y);
            Debug.Log(map_grid.maxY);
            printOnce = false;
        }
        if(climateValues > 1 || climateValues < 0)
        {
            Debug.Log("ERROR: VALUES NOT IN EXPECTED RANGE");
            Debug.Log(climateValues);
        }
        if(climateValues > 0.95)
        {
            //return "sand"
            retVal =  4;
        }
        else if(climateValues > 0.7)
        {
            //retVal =  Grass
            retVal =  5;
        }
        else if(climateValues > 0.55)
        {
            //retVal =  Forest
            retVal =  1;
        }
        else if(climateValues > 0.3)
        {
            //retVal =  Grass
            retVal =  5;
        }
        if(retVal != 0)
        {
            //retVal = noiseValues.ToList().IndexOf(noiseValues.Max());
            if(noiseValues[1] > 0.0f)
            {
                retVal = 2;
            }
            if(noiseValues[2] > 0.0f)
            {
                retVal = 3;
            }
            if(noiseValues[0] > 0.0f)
            {
                retVal = 0;
            }

        }
        return retVal;
    }

    GameObject getPrefab(int val)
    {
        if(prefabs[val] != null)
            return prefabs[val];
        Debug.Log("Error: prefab not set");
        return prefabs[0];
    }
}
