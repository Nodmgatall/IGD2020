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
    public Vector3[,] m_grid; // vector for grid positions
    public GridType m_gridType;
    public int getNumHorizontal(){return m_numHorizontal;}
    public int getNumVertical(){return m_numVertical;}

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

        m_grid = new Vector3[m_numHorizontal, m_numVertical];
    }

    public Vector3[,] getGrid(){
        return m_grid;
    }

    public float getMaxY(){
        return m_maxY;
    }
    public void create()
    {
        createSquareBasedGrid();
    }
    public void createSquareBasedGrid()
    {
        for(int x = 0; x < m_numHorizontal; x++)
        {
            int removeX = 0;
            for(int y = 0; y < m_numVertical; y++){
                Vector3 pos = m_origin + (x * -m_offsetTopDown); // negative offset so that the 0x chunks are not placed above 0
                pos += m_offsetBotRight * y; //set y coord without needed x offset
                pos -= (removeX * m_offsetTopDown); // add offset on y axis to move tile up
                if(y % 2 == 0) removeX -= 1; // raise offset every second iteration so that tiles are at the proper x pos

                m_grid[x,y] = pos;
            }
        }
    }
}

[Serializable]
public class WorldChunk{
    public Vector3 m_origin;
    public MapGrid m_mapGrid;
    public Tuple<int,int> m_chunkID;
    public GameObject[,] m_tileMap;

    [HideInInspector]
    public GameObject m_root;

    public WorldChunk(Tuple<int,int> p_chunkID,Vector3 p_origin, int p_numHorizontal, int p_numVertical, Vector3 p_offsetTop, Vector3 p_offsetBotRight)
    {
        m_chunkID = p_chunkID;
        m_root = new GameObject();
        m_origin = p_origin;
        m_mapGrid = new MapGrid(p_origin, p_numHorizontal, p_numVertical,p_offsetTop, p_offsetBotRight);
        m_tileMap = new GameObject[p_numHorizontal, p_numVertical];
        m_mapGrid.create();
    }
};

// [ExecuteInEditMode]  // OUTCOMMENT IF YOU WANT THIS TO RUN IN EDITOR
public class WorldGen : MonoBehaviour
{
    // Start is called before the first frame update
    Dictionary<Tuple<int,int>, WorldChunk> m_chunks;

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
    float m_chunkOffsetVertical;
    public int m_numHorizontal;
    public int m_numVertical;
    public int m_chunkSizeX;
    public int m_chunkSizeY;

    Vector3 m_offsetTopDown = new Vector3(0f,0f,10f);
    Vector3 m_offsetBotRight = new Vector3(8.6625f,0f, -5.0f);

    void calculateChunkOffsets(int p_numHorizontal, int p_numVertical)
    {
        Vector3 vectorTopLeftToBotRight = (m_chunkSizeY * m_offsetBotRight);
        m_chunkOffsetHorizontal = vectorTopLeftToBotRight.x;
        m_chunkOffsetVertical =  vectorTopLeftToBotRight.z * 2;
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
        m_chunks = new Dictionary<Tuple<int,int>, WorldChunk>();
    }

    public void LoadMap(){
        calculateChunkOffsets(m_numHorizontal, m_numVertical);
        Debug.Log("LoadChunk called");
        foreach (Transform child in m_map.transform)
        {
            Destroy(child.gameObject);
        }

        for (int x = 0; x < m_numHorizontal; x++){
            for (int y = 0; y < m_numVertical; y++){
                Vector3 curOrigin = new Vector3(x * m_chunkOffsetHorizontal,0,y*m_chunkOffsetVertical);
                var ID = getChunkID(curOrigin);
                //Debug.Log(ID);
                WorldChunk newChunk = new WorldChunk(ID,curOrigin,m_chunkSizeX,m_chunkSizeY, m_offsetTopDown, m_offsetBotRight);
                LoadGrid(ref newChunk, newChunk.m_root);
                m_chunks.Add(ID,newChunk);
            }
        }
    }

    public void LoadGrid(ref WorldChunk p_worldChunk, GameObject p_parent)
    {
        var grid = p_worldChunk.m_mapGrid;
        for (int i = 0; i < grid.getNumHorizontal(); i++){
            for (int j = 0; j  < grid.getNumVertical(); j++){
                var v = grid.m_grid[i,j];
                float x = v.x + m_origin.x;
                float y = v.z + m_origin.z;
                float[] noiseValues = getNoise(x, y, m_origin);
                float height = m_elevationNoise.getNoise(x,y);
                Vector3 newTilePos = new Vector3(v.x,(int) (height * 50 ), v.z);
                newTilePos.x += m_origin.x;
                newTilePos.z += m_origin.z;

                GameObject createdObject;
                GameObject toCreate = standartTile;
                if(noiseValues.Sum() > 0.001f)
                {

                    int prefabIDX = noiseValues.ToList().IndexOf(Mathf.Max(noiseValues));
                    toCreate = getPrefab(prefabIDX);
                    if(prefabIDX == 2)
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
                    createdObject = Instantiate(toCreate,newTilePos, Quaternion.identity);
                }
                else{
                    newTilePos.y = 0.3f * 50.0f;
                    createdObject = Instantiate(toCreate,newTilePos, Quaternion.identity);
                }
                p_worldChunk.m_tileMap[i,j] = createdObject;
                createdObject.GetComponent<Tile>().setCoordText(getChunkID(newTilePos), getTileFromChunk(newTilePos, getChunkID(newTilePos)));
                p_worldChunk.m_tileMap[i,j].transform.parent = p_parent.transform;
            }
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

    public Tuple<int,int> getChunkID(GameObject p_centerObject)
    {
        var pos = p_centerObject.transform.position;
        return getChunkID(pos);
    }
    public Tuple<int,int> getChunkID(Vector3 p_pos)
    {
        var pos = p_pos;
        pos.z -= 5.0f;
        float chunkWidth = m_chunkSizeX * 8.66025f;
        float chunkHeight = m_chunkSizeY * 10.0f;

        int idX =(int)Math.Floor(pos.x / chunkWidth);
        int idY =(int)Math.Ceiling(pos.z / chunkHeight);


        return new Tuple<int,int>(idX, idY);
    }

    public Tuple<int,int> getTileFromChunk(Vector3 pos, Tuple<int,int> chunkID)
    {
        float a = chunkID.Item1 * (float)m_chunkSizeY;
        float b = (chunkID.Item2 * (float)m_chunkSizeX);
        Tuple<int,int> tileID = new Tuple<int,int>(
                (int)( (Math.Round(pos.x / 8.66025f)) - a),
                (int)( (Math.Round((pos.z - 2.5f) / 10.0f)) - b) * -1);
        //Debug.Log(chunkID.ToString() + " " + tileID);
        return tileID;
    }

    public List<Tile> getNeighbours(Tile p_tile)
    {
        Vector3 test = new Vector3 (8.66025f,0, 10.0f);
        Vector3[] dirs = {
            Vector3.Scale(new Vector3(0,0,-1), test),
            Vector3.Scale(new Vector3(0,0,+1),test),
            Vector3.Scale(new Vector3(+1,0,-0.5f),test),
            Vector3.Scale(new Vector3(+1,0,0.5f),test),
            Vector3.Scale(new Vector3(-1,0,-0.5f),test),
            Vector3.Scale(new Vector3(-1,0,0.5f), test)
        };
        var tilePos = p_tile.transform.position;
        var l = new List<Tile>();
        getTileFromChunk(tilePos, getChunkID(tilePos));
        foreach (var x in dirs){
            var pos = tilePos + x;
            Tuple<int,int> chunkID = getChunkID(pos);
            if(m_chunks.ContainsKey(chunkID)){
                var tileID = getTileFromChunk(pos,chunkID);
                Debug.Log("added " + tileID + " " + chunkID);
                l.Add(m_chunks[chunkID].m_tileMap[tileID.Item2, tileID.Item1].GetComponent<Tile>());

            }
            else{
                Debug.Log("ignored out of bounds element");
            }
        }
        return l;
    }

}
