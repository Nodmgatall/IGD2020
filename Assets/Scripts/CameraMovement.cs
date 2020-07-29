using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float m_speed; //movement speed x,z
    public float m_zoomSpeed = 20; // movement y (up/down)
    public float m_rotationSpeed;
    public float m_maxHeight = 100; // maxZoom
    public float m_minHeight = 20; // minZoom
    public float boundaryFractionX = 0.015f;
    public float boundaryFractionY = 0.015f;

    public int leftBoundary = -180;
    public int rightBoundary = 180;
    public int bottomBoundary = -240;
    public int topBoundary = 200;
    float m_boundaryX;
    float m_boundaryY;

    private int theScreenWidth;
    private int theScreenHeight;


    // Start is called before the first frame update
    void Start()
    {
        theScreenWidth = Screen.width;
        theScreenHeight = Screen.height;
        m_boundaryX = theScreenWidth * boundaryFractionX; // TODO: change on resize
        m_boundaryY = theScreenHeight * boundaryFractionY;
        jumpTo(0,0);
    }
    public void handleZoom(float p_deltaMouseWheel){

        float newHeight = Mathf.Max(transform.position.y - (p_deltaMouseWheel * m_zoomSpeed), m_minHeight);
        transform.position = new Vector3(transform.position.x,Mathf.Min(newHeight, m_maxHeight),transform.position.z); //TODO integrate into change vector
    }

    // Update is called once per frame
    void Update()
    {
        float currentHeight = transform.position.y;
        float speed = m_speed * Time.deltaTime *  (currentHeight / 10);
        Vector3 forward = speed * transform.forward;
        Vector3 side = speed * transform.right;
        Vector3 change = new Vector3(0,0,0);

        if(Input.GetKey("w")){change += forward;}
        if(Input.GetKey("s")){change -= forward;}
        if(Input.GetKey("d")){change += side;}
        if(Input.GetKey("a")){change -= side;}

        if(Input.GetKey("q")){ transform.Rotate(new Vector3(0,m_rotationSpeed,0) * -1 * Time.deltaTime);}
        if(Input.GetKey("e")){ transform.Rotate(new Vector3(0,m_rotationSpeed,0)      * Time.deltaTime);}

        transform.position += change;
        if(Input.GetKey("space"))
        {
            jumpTo(0,0);
        }
        clampCamera();
    }
    //TODO refactor checks into MouseManager
    public void moveOnEdge(float p_mousePosX, float p_mousePosY)
    {
        Vector3 change = new Vector3(0,0,0);
        float currentHeight = transform.position.y;
        float speed = m_speed * Time.deltaTime *  (currentHeight / 10);
        if ( p_mousePosX > theScreenWidth - m_boundaryX && p_mousePosX < theScreenWidth)
        {
            change.x += speed;
        }

        if (p_mousePosX < 0 + m_boundaryX && p_mousePosX > 0)
        {
            change.x -= speed;
        }

        if (p_mousePosY > theScreenHeight - m_boundaryY && p_mousePosY < theScreenHeight)
        {
            change.z += speed;
        }

        if (p_mousePosY < 0 + m_boundaryY && p_mousePosY > 0)
        {
            change.z -= speed;
        }
        transform.position += change;

    }

    public void jumpTo(float x, float y)
    {
        float height = transform.position.y;
        float cameraAngle = 75; // TODO: get this from the camera itself

        Vector3 cameraForward = transform.GetChild(0).forward;
        float lenght = height / Mathf.Sin(cameraAngle);
        var offset = (cameraForward * lenght);

        Vector3 newPos = offset + new Vector3(x,0,y);
        newPos.y = height;

        transform.position = newPos;
    }

    void clampCamera(){
        Vector3 cameraPosition = transform.position;
        cameraPosition.x = Mathf.Clamp(cameraPosition.x, leftBoundary, rightBoundary);
        cameraPosition.z = Mathf.Clamp(cameraPosition.z, bottomBoundary, topBoundary);
        transform.position = cameraPosition;
    }
}
