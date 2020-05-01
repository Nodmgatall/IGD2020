using UnityEngine;
using System;
using System.Collections;

public struct MapGrid
{
    Vector3 origin;
    Vector3 offsetTopDown;
    Vector3 offestTopLeft;
    int m_radius;
    public Vector3[] grid;

    int numHexTiles;

    public MapGrid(Vector3 p_origin, int p_radius){
        m_radius = p_radius;
        p_radius = p_radius + 1;
        origin = p_origin;

        origin = p_origin;
        offsetTopDown= new Vector3(0f,0f,10.003f);
        offestTopLeft= new Vector3(8.6625f,0f, -5.0f);
        numHexTiles = (3 * (p_radius * p_radius)) - (3 * p_radius) + 1;
        Debug.Log(numHexTiles);
        grid = new Vector3[numHexTiles];
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
                Debug.Log(tileID);
                tileID++;
            }
        }
    }
}

public class WorldGen : MonoBehaviour
{
    // Start is called before the first frame update
    MapGrid map_grid;
    public GameObject myPrefab;
    public int radius;
    public Vector3 origin;
    void Start()
    {
        map_grid = new MapGrid( origin, radius);
        map_grid.createHexShapedBasicGrid();
        foreach (Vector3 x in map_grid.grid){
            Debug.Log(x);
            Instantiate(myPrefab,x, Quaternion.identity);
        }
    }
}
