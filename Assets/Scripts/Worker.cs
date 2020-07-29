using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    public const int adultAge = 4;
    public const int retireAge = 10;
    public const int deathAge = 20;
    [SerializeField]
    public static int numElderly;
    [SerializeField]
    public static int numChildren;
    [SerializeField]
    public static int numAdult;
    [SerializeField]
    public static int numEmployed;
    [SerializeField]
    public static int numUnimployed;
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
        m_currentCycle = -0.0f;
        _happiness = 1.0f;
        _age = 0;
        numChildren++;
        Age();
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
        if(m_currentCycle <= 0.0f)
        {
            //Debug.Log("LOL " + m_currentCycle+ "  " + m_cycleLength +  " " + GetInstanceID().ToString());
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
            case adultAge:
                BecomeOfAge();
                break;
            case retireAge:
                numElderly++;
                numAdult--;
                Retire();
                break;
            default:
                if(_age == deathAge){
                    Die();
                    numElderly--;
                    return false;
                }
                break;
        }
        _age++;
        return true;

    }
    public int GetAge(){return _age;}

    public void BecomeOfAge()
    {
        numChildren--;
        numAdult++;
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
