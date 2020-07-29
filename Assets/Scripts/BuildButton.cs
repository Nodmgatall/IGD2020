using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildButton : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject prefab;
    public void onClick()
    {
        gameManager.setBuildMode(prefab);
    }
}
