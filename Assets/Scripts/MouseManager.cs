using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;

    public GameObject mainCamera;
    public float speed = 1.0f;
    public int leftBoundary = -180;
    public int rightBoundary = 180;
    public int bottomBoundary = -240;
    public int topBoundary = 200;
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
        if (Input.GetMouseButtonDown(1)) // When right button of mouse is pressed
        {
            lastPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(1)) // When right button of mouse held down then its true
        {
            Vector3 delta = Input.mousePosition - lastPosition;
            mainCamera.transform.Translate(delta.x * speed, 0, delta.y * speed);
            Vector3 cameraPosition = mainCamera.transform.position;
            cameraPosition.x = Mathf.Clamp(cameraPosition.x, leftBoundary, rightBoundary);
            cameraPosition.z = Mathf.Clamp(cameraPosition.z, bottomBoundary, topBoundary);
            Debug.Log(cameraPosition.x + "," + cameraPosition.z + ",,,,,," + cameraPosition);
            mainCamera.transform.position = cameraPosition;
            lastPosition = Input.mousePosition;
        }
    }
}
