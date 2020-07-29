using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StateController : MonoBehaviour
{
    StateManager m_stateManager;
    public void Start(){
        m_stateManager = FindObjectOfType<StateManager>();
    }
    public void popState(){
        Debug.Log("poping state");
        m_stateManager.pop();
    }
    public void pushState(int stateIdx){
    }
}
