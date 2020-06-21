using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public WorldGen _worldGen;
    public JobManager _jobManager;
#region Map generation
    private Tile[,] _tileMap; //2D array of all spawned tiles
#endregion

#region Buildings
    public GameObject[] _buildingPrefabs; //References to the building prefabs
    public int _selectedBuildingPrefabIndex = 0; //The current index used for choosing a prefab to spawn from the _buildingPrefabs list
#endregion


#region Resources
    private Dictionary<ResourceTypes, float> _resourcesInWarehouse = new Dictionary<ResourceTypes, float>(); //Holds a number of stored resources for every ResourceType

    //A representation of _resourcesInWarehouse, broken into individual floats. Only for display in inspector, will be removed and replaced with UI later
    [SerializeField]
    private float _ResourcesInWarehouse_Money;
    [SerializeField]
    private float _ResourcesInWarehouse_Fish;
    [SerializeField]
    private float _ResourcesInWarehouse_Wood;
    [SerializeField]
    private float _ResourcesInWarehouse_Planks;
    [SerializeField]
    private float _ResourcesInWarehouse_Wool;
    [SerializeField]
    private float _ResourcesInWarehouse_Clothes;
    [SerializeField]
    private float _ResourcesInWarehouse_Potato;
    [SerializeField]
    private float _ResourcesInWarehouse_Schnapps;
#endregion

#region Enumerations
    public enum ResourceTypes { None,Money, Fish, Wood, Planks, Wool, Clothes, Potato, Schnapps }; //Enumeration of all available resource types. Can be addressed from other scripts by calling GameManager.ResourceTypes
#endregion

    //MYSTUFF
    //--------------------------------------------------------------
    public float m_timePerTick = 10.0f;
    [SerializeField]
    private float m_ecoTickTimer  = 0.0f;
    [SerializeField]
    private bool m_buildModeON = false;

    void ecoTick()
    {
        float laterImplementedSumOfAllUpkeep = 0.0f;
        _resourcesInWarehouse[ResourceTypes.Money] += 100 - laterImplementedSumOfAllUpkeep;
        _resourcesInWarehouse[ResourceTypes.Planks] += 100 - laterImplementedSumOfAllUpkeep;
    }
    public bool buildMode(){return m_buildModeON;}
    public void createBuilding(GameObject p_tile)
    {
        Tile t = p_tile.GetComponent<Tile>();
        placeBuildingOnTile(t);

    }

    //--------------------------------------------------------------
#region MonoBehaviour
    // Start is called before the first frame update
    void Start()
    {
        PopulateResourceDictionary();
    }

    // Update is called once per frame
    void Update()
    {
        m_ecoTickTimer -= Time.deltaTime;
        if(m_ecoTickTimer < 0.0 )
        {
            m_ecoTickTimer += m_timePerTick;
            ecoTick();
        }
        HandleKeyboardInput();
        UpdateInspectorNumbersForResources();
    }
#endregion

#region Methods
    //Makes the resource dictionary usable by populating the values and keys
    void PopulateResourceDictionary()
    {
        _resourcesInWarehouse.Add(ResourceTypes.None, 1000);
        _resourcesInWarehouse.Add(ResourceTypes.Money, 1000);
        _resourcesInWarehouse.Add(ResourceTypes.Fish, 1000);
        _resourcesInWarehouse.Add(ResourceTypes.Wood, 1000);
        _resourcesInWarehouse.Add(ResourceTypes.Planks, 1000);
        _resourcesInWarehouse.Add(ResourceTypes.Wool, 1000);
        _resourcesInWarehouse.Add(ResourceTypes.Clothes, 1000);
        _resourcesInWarehouse.Add(ResourceTypes.Potato, 1000);
        _resourcesInWarehouse.Add(ResourceTypes.Schnapps, 1000);
    }

    //Sets the index for the currently selected building prefab by checking key presses on the numbers 1 to 0
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown("b"))
        {
            m_buildModeON = !m_buildModeON;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _selectedBuildingPrefabIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _selectedBuildingPrefabIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _selectedBuildingPrefabIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _selectedBuildingPrefabIndex = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _selectedBuildingPrefabIndex = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _selectedBuildingPrefabIndex = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            _selectedBuildingPrefabIndex = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            _selectedBuildingPrefabIndex = 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            _selectedBuildingPrefabIndex = 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _selectedBuildingPrefabIndex = 9;
        }
    }

    //Updates the visual representation of the resource dictionary in the inspector. Only for debugging
    void UpdateInspectorNumbersForResources()
    {
        _ResourcesInWarehouse_Fish = _resourcesInWarehouse[ResourceTypes.Fish];
        _ResourcesInWarehouse_Wood = _resourcesInWarehouse[ResourceTypes.Wood];
        _ResourcesInWarehouse_Planks = _resourcesInWarehouse[ResourceTypes.Planks];
        _ResourcesInWarehouse_Wool = _resourcesInWarehouse[ResourceTypes.Wool];
        _ResourcesInWarehouse_Clothes = _resourcesInWarehouse[ResourceTypes.Clothes];
        _ResourcesInWarehouse_Potato = _resourcesInWarehouse[ResourceTypes.Potato];
        _ResourcesInWarehouse_Schnapps = _resourcesInWarehouse[ResourceTypes.Schnapps];
        _ResourcesInWarehouse_Money = _resourcesInWarehouse[ResourceTypes.Money];
    }

    //Checks if there is at least one material for the queried resource type in the warehouse
    public bool HasResourceInWarehoues(ResourceTypes resource, int cnt)
    {
        return _resourcesInWarehouse[resource] >= cnt;
    }

    public bool getResource(ResourceTypes resource, int cnt)
    {
        if(HasResourceInWarehoues(resource,cnt))
        {
            _resourcesInWarehouse[resource] -=cnt;
            return true;
        }
        return false;
    }

    public void addResource(ResourceTypes resource, int cnt)
    {
        _resourcesInWarehouse[resource] += cnt;
    }

    //Checks if the currently selected building type can be placed on the given tile and then instantiates an instance of the prefab
    private void placeBuildingOnTile(Tile t)
    {
        var gameObjPrefab = _buildingPrefabs[_selectedBuildingPrefabIndex];
        Building bluePrint = gameObjPrefab.GetComponent<Building>();
        if(bluePrint.canBeBuildOn.Contains(t._type)){
            if(bluePrint.m_costMoney <  _resourcesInWarehouse[ResourceTypes.Money]){
                if(bluePrint.m_costPlanks < _resourcesInWarehouse[ResourceTypes.Planks]){

                    if(t._building == null){

                        _resourcesInWarehouse[ResourceTypes.Money] -= bluePrint.m_costMoney;
                        _resourcesInWarehouse[ResourceTypes.Planks] -= bluePrint.m_costPlanks;

                        bluePrint._jobManager = _jobManager;
                        bluePrint.gameManager = this;
                        bluePrint.m_root = t;

                        var theThing = Instantiate(_buildingPrefabs[_selectedBuildingPrefabIndex],t.transform.position, Quaternion.identity);
                        var te = findNeighborsOfTile(t);
                        var b = theThing.GetComponent<Building>();
                        t._building = b;
                        b.calculateEfficiency(te);
                    }
                    else{
                        Debug.Log("Selected building can not be build: another building already present");
                    }
                }
                else{
                    Debug.Log("Selected building can not be build: missing planks");
                }
            }
            else{
                Debug.Log("Selected building can not be build: missing founds");
            }
        }
        else{
            Debug.Log("Selected building can not be build on "+ t);
        }
    }

    //Returns a list of all neighbors of a given tile
    private List<Tile> findNeighborsOfTile(Tile t)
    {
        return _worldGen.getNeighbours(t);
    }
#endregion
}
