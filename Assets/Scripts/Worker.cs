using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
#region Manager References
    public JobManager _jobManager; //Reference to the JobManager
    public GameManager _gameManager;//Reference to the GameManager
#endregion

    public int _age; // The age of this worker
    public float _happiness; // The happiness of this worker
    [SerializeField]
    private float m_currentCycle;
    [SerializeField]
    public float m_cycleLength;
    public HousingBuilding hb;
    public Job m_job;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Worker Created");
        m_currentCycle = m_cycleLength;
        _happiness = 1.0f;
        _age = 0;
    }

    void upkeep()
    {
        float happiness = 1.0f;
        if(!_gameManager.getResource(GameManager.ResourceTypes.Schnapps,1)){ }
        if(!_gameManager.getResource(GameManager.ResourceTypes.Fish,1)){}
        if(!_gameManager.getResource(GameManager.ResourceTypes.Clothes,1)){}
        _happiness = happiness;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaT = Time.deltaTime;
        m_currentCycle -= deltaT;
        if(m_currentCycle < 0.0f)
        {
            //Debug.Log("LOL " + m_currentCycle+ "  " + m_cycleLength +  " " + GetInstanceID().ToString());
            _age++;
            bool alive = Age();
            if(alive)
            {
                upkeep();
            }
            m_currentCycle += m_cycleLength;
        }
    }


    private bool Age()
    {
        switch(_age)
        {
            case 0:
                break;
            case 2:
                BecomeOfAge();
                break;
            case 10:
                Retire();
                break;
            default:
                if(_age > 100){
                    Die();
                    return false;
                }
                break;
        }
        return true;

    }


    public void BecomeOfAge()
    {
        _jobManager.RegisterWorker(this);
    }

    private void Retire()
    {
        Debug.Log("Retiring");
        if(m_job._building != null){
            m_job.RemoveWorker(this);
            _jobManager.RegisterJob(m_job);
            m_job = null;
        }
        else{
            _jobManager.RemoveWorker(this);
        }
    }

    private void Die()
    {
        if(hb != null){
            hb.WorkerRemovedFromBuilding(this);
        }
        else{
            Debug.Log("ERROR HB NIT SET");
        }
        Destroy(this.gameObject, 1f);
    }
}
