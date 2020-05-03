using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float m_speed;
    public float m_zoomSpeed;
    public float m_rotationSpeed;
    public float m_maxHeight = 100;
    public float m_minHeight = 20;
    public float boundaryFractionX = 0.015f;
    public float boundaryFractionY = 0.015f;
    float boundaryX;
    float boundaryY;

    private int theScreenWidth;
    private int theScreenHeight;


    // Start is called before the first frame update
    void Start()
    {
        theScreenWidth = Screen.width;
        theScreenHeight = Screen.height;
        boundaryX = theScreenWidth * boundaryFractionX;
        boundaryY = theScreenHeight * boundaryFractionY;
    }

    // Update is called once per frame
    void FixedUpdate()
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

        float mW = Input.GetAxis("Mouse ScrollWheel");
        if(mW  != 0){
            float newHeight = Mathf.Max(transform.position.y - (mW * m_zoomSpeed), m_minHeight);
            transform.position = new Vector3(transform.position.x,Mathf.Min(newHeight, m_maxHeight),transform.position.z);
            Debug.Log("what now?");
        }
        float mPosX =Input.mousePosition.x;
        float mPosY =Input.mousePosition.y;
        if ( mPosX > theScreenWidth - boundaryX && mPosX < theScreenWidth)
        {
            change.x += speed;
        }

        if (mPosX < 0 + boundaryX && mPosX > 0)
        {
            change.x -= speed;
        }

        if (mPosY > theScreenHeight - boundaryY && mPosY < theScreenHeight)
        {
            change.z += speed;
        }

        if (mPosY < 0 + boundaryY && mPosY > 0)
        {
            change.z -= speed;
        }

        transform.position += change;
    }
}
