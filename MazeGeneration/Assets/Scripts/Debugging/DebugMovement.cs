using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class DebugMovement : MonoBehaviour
{
    Rigidbody rb;
    Transform cam;

    public float speed = 6.0f;
    public float rotSpeed = 50.0f;
    public float jumpSpeed = 8.0f;

    bool useWorldSpace = false;

    Vector3 playerForward;
    Vector3 playerRight;

    private Vector3 moveDirection = Vector3.zero;

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationY = 0F;

        


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = transform.GetChild(0);

        Transform tile = GameObject.Find("/MapManager/0 - Maze/Tile R0C0").transform;
        transform.position = new Vector3(tile.position.x, transform.position.y,tile.position.z);
    }

    private void FixedUpdate()
    {
        //switch movement mode
        if (Input.GetKeyDown("space"))
        {

            useWorldSpace = !useWorldSpace;
        }

        playerForward = Vector3.ProjectOnPlane(cam.forward, transform.up).normalized;
        playerRight = Vector3.ProjectOnPlane(cam.right, transform.up).normalized;


        movement();

        turning();




    }

    private void movement()
    {

        //moving 
        if (Input.GetKey("w")) //forward
        {
            if (useWorldSpace)
            {
                rb.AddForce(Vector3.forward * speed);
            }
            else
            {
                rb.AddForce(playerForward * speed);
            }
        }
        if (Input.GetKey("s")) //backward
        {
            if (useWorldSpace)
            {
                rb.AddForce(-Vector3.forward * speed);
            }
            else
            {
                rb.AddForce(-playerForward * speed);
            }
        }
        if (Input.GetKey("a")) // leftward
        {
            if (useWorldSpace)
            {
                rb.AddForce(-Vector3.right * speed);
            }
            else
            {
                rb.AddForce(-playerRight * speed);
            }
        }
        if (Input.GetKey("d")) //rightward
        {
            if (useWorldSpace)
            {
                rb.AddForce(Vector3.right * speed);
            }
            else
            {
                rb.AddForce(playerRight * speed);
            }
        }
    }

    private void turning()
    {
        //turning
        if (Input.GetKey("q")) // leftward
        {
            transform.Rotate(0, -rotSpeed * Time.deltaTime, 0);


        }
        if (Input.GetKey("e")) //rightward
        {

            transform.Rotate(0, rotSpeed * Time.deltaTime, 0);

        }

        if (axes == RotationAxes.MouseXAndY)
        {
            float rotationX = cam.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            cam.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
        else if (axes == RotationAxes.MouseX)
        {
            cam.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
        }
        else
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            cam.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
        }
    }
}
