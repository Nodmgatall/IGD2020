using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    public enum CounterType{Resource, Population};
    public GameManager.ResourceTypes resourceType;
    public GameManager.PopulationTypes popTypes;
    public GameManager gameManager;
    public CounterType counterType;
    Text text;

    public void Update()
    {
        text.text = getCount().ToString();
    }
    public void Start()
    {
        text = GetComponent<Text>();
        text.text = getCount().ToString();
    }
    private float getCount(){
        float count;
        if(counterType == CounterType.Resource){
            count = gameManager.getResourceCount(resourceType);
        }
        else{
            count = gameManager.getPopCount(popTypes);
        }
        return count;
    }
}
