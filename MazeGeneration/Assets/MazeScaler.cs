using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeScaler : MonoBehaviour
{
    [Header("Size in beginning")]
    public float width;
    public float height;

    [Header("minimum settings")]
    public float minimumWidth = 0.25f;
    public float minimumHeight = 1.5f;
    float minWidthMod;
    float minHeightMod;

    [Header("maximum settings")]
    public float maximumWidth = 1.5f;
    public float maximumHeight = 5f;
    float maxWidthMod;
    float maxHeightMod;

    float widthModifier=0;
    float heightModifier=0;

    float widthStep = 0.05f;
    float heightStep = 0.1f;

    int row;
    int col;

    Transform mainCamTrans;

    private Tile middle;

    private GameObject mazeSegment;
    // Start is called before the first frame update
    void Start()
    {
        mazeSegment = GameObject.Find("0 - Maze");
        mainCamTrans = Camera.main.gameObject.transform;
        middle = GameObject.Find("Tile R1C1").GetComponent<Tile>();
        SetLocalScale();
        minWidthMod = minimumWidth - width;
        minHeightMod = minimumHeight - height;

        maxWidthMod = maximumWidth - width;
        maxHeightMod = maximumHeight - height;


    }

    private void SetLocalScale()
    {
        //checking minimum settings
        if (widthModifier < minWidthMod)
        {
            widthModifier = minWidthMod;
        }
        if (heightModifier < minHeightMod)
        {
            heightModifier = minHeightMod;
        }

        //checking maximum settings
        if (widthModifier > maxWidthMod)
        {
            widthModifier = maxWidthMod;
        }
        if (heightModifier > maxHeightMod)
        {
            heightModifier = maxHeightMod;
        }

        mazeSegment.transform.localScale = new Vector3(width+widthModifier, height+heightModifier, width + widthModifier);
        SetPosition();
    }

    private void SetPosition()
    {
        //Debug.Log("before " + mazeSegment.transform.position);
        float offset = (width + widthModifier);
        //Debug.Log(offset);
        mazeSegment.transform.position = new Vector3(-offset, 0,offset);
        //Debug.Log("after " + mazeSegment.transform.position);
    }

    // Update is called once per frame
    void Update()
    {

        //pc controls
        if (Input.GetKeyDown("up"))
        {
            heightModifier += heightStep;
            SetLocalScale();
        }

        if (Input.GetKeyDown("down"))
        {
            heightModifier -= heightStep;
            SetLocalScale();
        }

        if (Input.GetKeyDown("left"))
        {
            widthModifier -= widthStep;
            SetLocalScale();
            SetPosition();
        }
        if (Input.GetKeyDown("right"))
        {
            widthModifier += widthStep;
            SetLocalScale();
            SetPosition();
        }

        //oculus controls
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick)) //reset
        {
            heightModifier = 0;
            widthModifier = 0;
            SetLocalScale();
        }
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickUp))
        {
            heightModifier += heightStep;
            SetLocalScale();
        }
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickDown))
        {
            heightModifier -= heightStep;
            SetLocalScale();
        }
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickRight))
        {
            widthModifier += widthStep;
            SetLocalScale();
            SetPosition();
        }
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickLeft))
        {
            widthModifier -= widthStep;
            SetLocalScale();
            SetPosition();
        }
    }
}
