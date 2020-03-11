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


    private GameObject mazeSegment;
    // Start is called before the first frame update
    void Start()
    {
        mazeSegment = GameObject.Find("0 - Maze");
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
        }
        if (Input.GetKeyDown("right"))
        {
            widthModifier += widthStep;
            SetLocalScale();
        }

        //oculus controls
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
        }
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickLeft))
        {
            widthModifier -= widthStep;
            SetLocalScale();
        }
    }
}
