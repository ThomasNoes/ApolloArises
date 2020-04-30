using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricityOutline : MonoBehaviour
{
    public Transform[] points;

    LineRenderer lr;


    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();

        lr.positionCount = points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            lr.SetPosition(i, points[i].position);
        }


    }

}
