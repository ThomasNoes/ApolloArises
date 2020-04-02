using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeScaler : MonoBehaviour
{
    public bool enableOculusControls = false, enablePcControls = false;

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

    public float widthStep = 0.1f;
    public float heightStep = 0.1f;

    int row;
    int col;

    public TestCompanionScaler[] companionScalers;
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
        //SetPosition();
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
        if (!enablePcControls)
            return;

        if (Input.GetKeyDown("up"))
            ScaleHeightUp();
        
        if (Input.GetKeyDown("down"))
            ScaleHeightDown();
        
        if (Input.GetKeyDown("left"))
            ScaleWidthDown();
        
        if (Input.GetKeyDown("right"))
            ScaleWidthUp();


        //oculus controls
        if (!enableOculusControls)
            return;

        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick)) //reset
        {
            heightModifier = 0;
            widthModifier = 0;
            SetLocalScale();
        }
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickUp))
            ScaleHeightUp();
        
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickDown))
            ScaleHeightDown();
        
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickRight))
            ScaleWidthUp();
        
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickLeft))
            ScaleWidthDown();       
    }

    public void ScaleWidthDown()
    {
        widthModifier -= widthStep;
        SetLocalScale();
        //SetPosition();

        foreach (TestCompanionScaler cScaler in companionScalers)
        {
            cScaler.ScaleWithMaze();
        }
    }

    public void ScaleWidthUp()
    {
        widthModifier += widthStep;
        SetLocalScale();
        //SetPosition();

        foreach (TestCompanionScaler cScaler in companionScalers)
        {
            cScaler.ScaleWithMaze();
        }
    }

    public void ScaleHeightDown()
    {
        heightModifier -= heightStep;
        SetLocalScale();
    }

    public void ScaleHeightUp()
    {
        heightModifier += heightStep;
        SetLocalScale();
    }
}
