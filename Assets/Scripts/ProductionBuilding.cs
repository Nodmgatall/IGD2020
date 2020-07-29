using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


/*
   Task 2: Buildings Create a "Building" script. Implement the following features:

   1) Parameters: Your script should contain a number of adjustable parameters:
   -type: The name of the building
   -upkeep: The money cost per minute
   -build Cost Money: placement money cost
   -build Cost Planks: placement planks cost
   -tile: Reference to the tile it is built on

   -Efficiency value: Calculated based on the surrounding tile types
   -Resource Generation interval: If operating at 100% efficiency, this is the time in
   seconds it takes for one production cycle to finish
   -Output Count: The number of output resources per generation cycle (for example the Sawmill produces 2
   planks at a time)

   -Can be built on tile types: A restriction on which types of tiles it can be
   placed on -Efficiency Scales With Neighboring Tiles: A choice if its efficiency
   scales with a specific type of surrounding tile -Minimum/Maximum Neighbors: The
   minimum and maximum number of surrounding tiles its efficiency scales with
   (0-6)

   -Input Resources: A choice for input resource types (0, 1 or 2 types) -Output
Resource: A choice for output resource type


2) Production and efficiency Input and output should directly connect to the
GameManager's warehouse system. In each cycle, the production building
retrieves the required resources from the warehouse (1 each). It takes the
production building one generation cycle interval in seconds to generate its
output.

However, based on the surrounding tiles this process can take longer. If not
enough required empty tiles are available, its efficiency is reduced. For
example, if the maximum of surrounding forest tiles is 4, and the actual count
is 3, then it will operate at 75% efficiency. If the count is 1, then it will
operate at 25% efficiency. However, if the count is below the defined minimum,
efficiency is set to 0% instead. An efficiency value of 75% translates as
following: for every second passed, the production cycle is progressed by a
value of 0.75.
*/

[Serializable]
class EfficiencyRequirements
{
    public Tile.TileTypes[] efficiencyBooster;
    public int[] m_minTile;
    public int[] m_maxTile;
}
public class ProductionBuilding : Building
{
    [SerializeField]
    private EfficiencyRequirements m_efficiencyRequirements;
    //Min and Max for sourounding tile boni

    public GameManager.ResourceTypes[] m_inputs;
    public int[] m_requiredInputsCnt;
    public GameManager.ResourceTypes m_output;
    [SerializeField]
    private bool m_resourceRequired = true;
    public int m_numOut;
    public int m_jobCount;

    protected override void Start()
    {
        base.Start();
        _jobs = new List<Job>();
        for(int i = 0; i < m_jobCount; i++){
            _jobs.Add(new Job(this));
            _jobManager.RegisterJob(_jobs[_jobs.Count - 1]);
        }
        //New instructions can be called after the base's.
    }

#region Jobs
    public List<Job> _jobs; // List of all available Jobs. Is populated in Start()
#endregion

    override public void calculateEfficiency(List<Tile> p_tileList)
    {
        Debug.Log(p_tileList.Count);
        foreach( var x in p_tileList)
        {
            Debug.Log(x.transform.gameObject.name);
        }
        m_root._neighborTiles = p_tileList;
        m_efficiency = 1.0f;
        for( int i = 0; i < m_efficiencyRequirements.efficiencyBooster.Length; i++)
        {
            int numPresent = 0;
            foreach(var x in p_tileList)
            {
                if(x._type == m_efficiencyRequirements.efficiencyBooster[i])
                {
                    numPresent++;
                }
            }
            Debug.Log("numPresent " + numPresent + "/" + p_tileList.Count);
            int min = m_efficiencyRequirements.m_minTile[i];
            int max = m_efficiencyRequirements.m_maxTile[i];
            if(numPresent< min)
            {
                m_efficiency = 0.0f;
            }
            else if (numPresent >= max)
            {
                m_efficiency = 1.0f;
            }
            else{
                float step =1.0f / ((float)max - (float)min);
                m_efficiency = (numPresent - min) * step;
            }
        }
        if(_workers.Count == 0){
            m_efficiency = 0.0f;
        }
        else{
            m_efficiency *= m_jobCount/_jobs.Count;
        }
        if(m_efficiency > 0.0f)
        {
            m_efficiency *= getWorkerHappiness();
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        if(!m_resourceRequired){
            float deltaT = Time.deltaTime * m_efficiency;
            m_currentCycle -= deltaT;
            if(m_currentCycle < 0.0f)
            {
                gameManager.addResource(m_output, m_numOut);
                m_resourceRequired = true;
            }
        }
    }
    void LateUpdate()
    {
        if(m_resourceRequired){
            for(int i = 0; i < m_inputs.Length;i++)
            {
                if(!gameManager.getResource(m_inputs[i], m_requiredInputsCnt[i])){
                    return;
                }
            }
            m_resourceRequired = false;
            m_currentCycle += m_cycleTime;
        }
    }
}
