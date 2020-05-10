using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;

    public GameObject mainCamera;
    public float mouseSensitivity = 1.0f;
    public Vector3 lastPosition;
    // Update is called once per frame
    void Update()
    {
        // Code to get the name of object on left click of mouse
        if (Input.GetMouseButtonDown(0)) // When left button of mouse is pressed
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.gameObject.name);
            }
        }

        // Code to move the camera while holding down right mouse button
        if (Input.GetMouseButton(1)) // When right button of mouse held down then its true
        {
            Debug.Log("Inside getmousebutton");
            lastPosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonDown(1)) // When right button of mouse is pressed
        {
            Debug.Log("Inside getmousebuttondown");
            Vector3 delta = Input.mousePosition - lastPosition;
            mainCamera.transform.Translate(delta.x * mouseSensitivity, delta.z * mouseSensitivity, delta.z * mouseSensitivity);
            lastPosition = Input.mousePosition;
        }
    }
}
