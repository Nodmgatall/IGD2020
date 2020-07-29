using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Building : MonoBehaviour
{
    public Tile.TileTypes[] canBeBuildOn;

    public GameManager gameManager;
    //type: The name of the building
    public String m_type;
    //upkeep: The money cost per minute
    public float m_upkeep;
    //build Cost Money: placement money cost
    public float m_costMoney;
    //build Cost Planks: placement planks cost
    public float m_costPlanks;

    [SerializeField]
    protected float m_efficiency;

    [SerializeField]
    protected float m_currentCycle;
    public float m_cycleTime;

    //tile:R eference to the tile it is built on
    public Tile m_root;
#region Manager References
    public JobManager _jobManager; //Reference to the JobManager
#endregion

#region Workers
    public List<Worker> _workers; //List of all workers associated with this building, either for work or living
#endregion
    protected virtual void Start(){}
    public virtual void Update(){}
    public virtual void calculateEfficiency(List<Tile> p_tileList){m_efficiency = 1.0f;}
    #region Methods   
    public virtual void WorkerAssignedToBuilding(Worker w)
    {
        _workers.Add(w);
    }

    public virtual void WorkerRemovedFromBuilding(Worker w)
    {
        Debug.Log("calling this as expected "+ w.GetAge());
        _workers.Remove(w);
    }
    #endregion

    protected float getWorkerHappiness(){
        float sumhappy = 0.0f;
        foreach(var w in _workers)
        {
            sumhappy += w._happiness;
        }
        return sumhappy/_workers.Count;
    }
}

