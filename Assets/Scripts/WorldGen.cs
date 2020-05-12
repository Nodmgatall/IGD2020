using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class NoiseSettings{

    [Range(0,1)]
    public float cutoffMin = 0;
    [Range(0,1)]
    public float cutoffMax = 1;
};
public enum GridType{Hex, Square};
[Serializable]
public class MapGrid
{
    Vector3 m_origin;

    //Offsets to next neighbour tile
    Vector3 m_offsetTopDown;
    Vector3 m_offsetBotRight;

    int m_numHorizontal;
    int m_numVertical;
    public float m_maxY; // max absolute scenespace Z value.
    public Vector3[] m_grid; // vector for grid positions
    public GridType m_gridType;

    int m_numHexTiles;

    void init(Vector3 p_origin, GridType p_gridType, Vector3 p_offsetTop, Vector3 p_offsetBotRight)
    {
        m_origin = p_origin;
        m_gridType = p_gridType;
        m_offsetTopDown = p_offsetTop;
        m_offsetBotRight = p_offsetBotRight;
    }
    public MapGrid(Vector3 p_origin, int p_numHorizontal, int p_numVertical, Vector3 p_offsetTop, Vector3 p_offsetBotRight)
    {
        init(p_origin, GridType.Square, p_offsetTop, p_offsetBotRight);

        m_numVertical = p_numVertical;
        m_numHorizontal = p_numHorizontal;
        m_numHexTiles = p_numHorizontal * p_numVertical;

        m_grid = new Vector3[m_numHexTiles];
    }
    public MapGrid(Vector3 p_origin, int p_radius, Vector3 p_offsetTop, Vector3 p_offsetBotRight)
    {
        init(p_origin, GridType.Hex, p_offsetTop, p_offsetBotRight);

        int radiusWithCenter = p_radius + 1; // center tile needed to be included here
        m_numVertical = radiusWithCenter;
        m_numHorizontal = radiusWithCenter;
        m_numHexTiles = (3 * (radiusWithCenter * radiusWithCenter)) - (3 * radiusWithCenter) + 1;

        m_grid = new Vector3[m_numHexTiles];
    }

    public Vector3[] getGrid(){
        return m_grid;
    }

    public float getMaxY(){
        return m_maxY;
    }
    public void create()
    {
        if(m_gridType == GridType.Hex)
        {
            createHexShapedBasicGrid();
        }
        else{
            createSquareBasedGrid();
        }
    }
    public void createSquareBasedGrid()
    {
        int tileID = 0;
        for(int x = 0; x < m_numHorizontal; x++)
        {
            int removeX = 0;
            for(int y = 0; y < m_numVertical; y++){
                Vector3 pos = x * m_offsetTopDown + (m_numVertical * -m_offsetTopDown) + (m_numHorizontal * -m_offsetBotRight);
                pos += m_offsetBotRight * y + removeX * m_offsetTopDown;
                removeX += y % 2;
                m_grid[tileID++] = pos + m_origin;
            }
        }
    }

    public void createHexShapedBasicGrid()
    {
        int radius = m_numVertical;
        int tileID = 0;
        for (int q = -radius; q <= radius; q++) {
            int r1 =Math.Max(-radius, -q - radius);
            int r2 =Math.Min(radius, -q + radius);
            for (int r = r1; r <= r2; r++) {
                m_grid[tileID] = m_origin + q * m_offsetBotRight + r * m_offsetTopDown + r * m_offsetBotRight;
                m_maxY= Math.Max(m_maxY, m_grid[tileID].z);
                tileID++;
            }
        }
    }
}

[Serializable]
public class WorldChunk{
    public Vector3 m_origin;
    public MapGrid m_mapGrid;

    [HideInInspector]
    public GameObject m_root;

    public WorldChunk(Vector3 p_origin, int p_numHorizontal, int p_numVertical, Vector3 p_offsetTop, Vector3 p_offsetBotRight)
    {
        m_root = new GameObject();
        m_mapGrid = new MapGrid(p_origin, p_numHorizontal, p_numVertical,p_offsetTop, p_offsetBotRight);
        m_mapGrid.create();
    }
};

// [ExecuteInEditMode]  // OUTCOMMENT IF YOU WANT THIS TO RUN IN EDITOR
public class WorldGen : MonoBehaviour
{
    // Start is called before the first frame update
    List<WorldChunk> m_chunks;

    public GameObject standartTile;
    GameObject m_map; // dummy object to hold generated tiles as children

    public NoiseManager[] m_vectorNoiseGens;
    public perlinNoise m_elevationNoise;
    public perlinNoise m_forestNoise;
    public perlinNoise m_stoneNoise;

    public NoiseManager m_forestNoiseManager;
    public NoiseManager m_stoneNoiseManager;

    public AnimationCurve m_climate;

    public GameObject m_camera;
    public int radius = 12;
    public Vector3 m_origin;
    float m_chunkOffsetHorizontal;
    float m_chunkOffsetVectical;
    public int m_numHorizontal;
    public int m_numVertical;
    public int m_chunkSizeX;
    public int m_chunkSizeY;

    Vector3 m_offsetTopDown = new Vector3(0f,0f,10.003f);
    Vector3 m_offsetBotRight = new Vector3(8.6625f,0f, -5.0f);

    void calculateChunkOffsets(int p_numHorizontal, int p_numVertical)
    {
        Vector3 vectorTopLeftToBotRight = (m_chunkSizeY * m_offsetBotRight);
        m_chunkOffsetHorizontal = vectorTopLeftToBotRight.x;
        m_chunkOffsetVectical =  vectorTopLeftToBotRight.z * 2;
    }

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
        m_map = new GameObject();
        m_map.transform.parent  = transform; // setting parent for tile holder
        m_chunks = new List<WorldChunk>(m_numVertical * m_numHorizontal);
        Debug.Log(m_chunks.Capacity);
    }

    public void LoadMap(){
        calculateChunkOffsets(m_numHorizontal, m_numVertical);
        Debug.Log("LoadChunk called");
        foreach (Transform child in m_map.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < m_numHorizontal; i++){
            for (int y = 0; y < m_numVertical; y++){
                Vector3 curOrigin = new Vector3(i * m_chunkOffsetHorizontal,0,y*m_chunkOffsetVectical);
                Debug.Log(curOrigin);
                WorldChunk newChunk = new WorldChunk(curOrigin,m_chunkSizeX,m_chunkSizeY, m_offsetTopDown, m_offsetBotRight);
                LoadGrid(ref newChunk.m_mapGrid, newChunk.m_root);
                m_chunks.Add(newChunk);
            }
        }
    }

    public void LoadGrid(ref MapGrid p_grid, GameObject p_parent)
    {
        foreach(Vector3 v in p_grid.getGrid()){
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
            createdObject.transform.parent = p_parent.transform;
        }
        p_parent.transform.parent = m_map.transform;
        invoked = false;
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
