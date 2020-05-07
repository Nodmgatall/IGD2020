using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Wcamera;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 start = Wcamera.transform.position;
        Vector3 direction = Wcamera.transform.forward;
        RaycastHit hit;
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Debug.Log("Testing");
        //}
        if(Physics.Raycast(start, direction, out hit))
        {
            // hit.collider.gameObject.SetActive(false);
            Debug.Log(hit.collider.gameObject.name);
        }
    }
}
