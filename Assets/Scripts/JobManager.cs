using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobManager : MonoBehaviour
{

    public List<Job> _availableJobs = new List<Job>();
    public List<Worker> _unoccupiedWorkers = new List<Worker>();



#region MonoBehaviour
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleUnoccupiedWorkers();
    }
#endregion


#region Methods

    private void HandleUnoccupiedWorkers()
    {
        while(true){
            if (_unoccupiedWorkers.Count > 0 && _availableJobs.Count > 0)
            {
                Worker w = _unoccupiedWorkers[0];
                Job j = _availableJobs[0];

                w.m_job = j;
                j._building.WorkerAssignedToBuilding(w);

                _unoccupiedWorkers.RemoveAt(0);
                _availableJobs.RemoveAt(0);
            }
            else{
                return;
            }
        }
    }

    public void RegisterJob(Job p_job)
    {
        _availableJobs.Add(p_job);
    }

    public void RemoveJob(Job p_job)
    {
        _availableJobs.Add(p_job);
    }
    public void RegisterWorker(Worker w)
    {
        _unoccupiedWorkers.Add(w);
    }



    public void RemoveWorker(Worker w)
    {
        _unoccupiedWorkers.Remove(w);
    }

#endregion
}
