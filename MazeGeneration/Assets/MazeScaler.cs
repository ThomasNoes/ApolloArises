using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeScaler : MonoBehaviour
{
    public float tileWidth;
    public float height;

    public float minimumWidth = 0.25f;
    public float minimumHeight = 1.5f;

    float minWidthMod;
    float minHeightMod;

    float widthModifier=0;
    float heightModifier=0;

    float widthStep = 0.05f;
    float heightStep = 0.05f;


    private GameObject mazeSegment;
    // Start is called before the first frame update
    void Start()
    {
        mazeSegment = GameObject.Find("0 - Maze");
        SetLocalScale();
        minWidthMod = minimumWidth - tileWidth;
        minHeightMod = minimumHeight - height;
    }

    private void SetLocalScale()
    {
        if (widthModifier < minWidthMod)
        {
            widthModifier = minWidthMod;
        }

        if (heightModifier < minHeightMod)
        {
            heightModifier = minHeightMod;
        }

        mazeSegment.transform.localScale = new Vector3(tileWidth+widthModifier, height+heightModifier, tileWidth + widthModifier);
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
