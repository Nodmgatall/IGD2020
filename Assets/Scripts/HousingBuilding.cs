
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingBuilding : Building
{
    public Worker workerPrefab;
    void spawnWorker()
    {
        Worker w = Instantiate(workerPrefab);
        w.transform.parent = this.transform;
        w._jobManager = _jobManager;
        w._gameManager = gameManager;
        w.hb = this;
        w._age = Worker.adultAge;
        _workers.Add(w);
    }
    protected override void Start()
    {
        base.Start();

        m_currentCycle = m_cycleTime;
        _workers = new List<Worker>();
        for(int i = 0; i < 2; i++){
            spawnWorker();
        }

        //New instructions can be called after the base's.
    }

    void updateEfficiency(){
        m_efficiency = getWorkerHappiness();
    }
    public override void Update()
    {
        float deltaT = Time.deltaTime * m_efficiency;
        m_currentCycle -= deltaT;
        if(m_currentCycle < 0.0f)
        {
            Debug.Log(m_currentCycle);
            if(_workers.Count < 10){ spawnWorker();}
            m_currentCycle += m_cycleTime;
            updateEfficiency();
        }
    }

};
