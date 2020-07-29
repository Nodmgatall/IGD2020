using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    public GameManager.ResourceTypes type;
    public int condition;
    GameManager m_gameManager;
    SceneLoader m_sceneLoader;
    StateManager m_stateManager;

    public bool triggerIfLesser;
    public bool triggerIfGreater;

    public Canvas UI;
    bool activated;
    void Start(){
        m_gameManager = FindObjectOfType<GameManager>();
        m_sceneLoader = GetComponent<SceneLoader>();
        m_stateManager = FindObjectOfType<StateManager>();
    }
    public void updateCondition()
    {
        bool conditionMet = false;
        if(triggerIfGreater && condition <= m_gameManager.getResourceCount(type) ){
            conditionMet = true;
        }
        if(triggerIfLesser && condition >= m_gameManager.getResourceCount(type) ){
            conditionMet = true;
        }
        if(!activated && conditionMet){
            UI = Instantiate(UI,new Vector3(0, 0, 0), Quaternion.identity) as Canvas;
            m_stateManager.myQ.Push(UI);
            m_gameManager.enabled = false;
            activated = true;
        }
    }
}
