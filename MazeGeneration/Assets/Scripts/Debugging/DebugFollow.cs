using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugFollow : MonoBehaviour
{
    public bool findOffsetFromPosition;
    public Vector3 offset;
    public Vector3 startOffset;
    public GameObject followObject;



    // Start is called before the first frame update
    void Start()
    {
        startOffset = transform.position - followObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (findOffsetFromPosition)
        {
            transform.position = followObject.transform.position + startOffset;
        }
        else
        {
            transform.position = followObject.transform.position + offset;
        }

    }
}
