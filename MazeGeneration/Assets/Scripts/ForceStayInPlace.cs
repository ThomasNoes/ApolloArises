using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceStayInPlace : MonoBehaviour
{
    public bool stayInPlace, noRotationX, noRotationY, noRotationZ;
    private float yValue, zValue, xValue;
    private Vector3 initialPos;
    private bool posActive, rotActive, startedOnce;

    //------------------------
    // Init
    //------------------------
    void Start()
    {
        if (startedOnce)
            return;

        startedOnce = true;

        if (stayInPlace)
            Invoke("DelayedStart", 0.4f);
    }

    private void DelayedStart()
    {
        if (stayInPlace)
        {
            initialPos = transform.position;
            posActive = true;
        }
        if (noRotationY)
        {
            yValue = transform.rotation.eulerAngles.y;
            rotActive = true;
        }
        if (noRotationZ)
        {
            zValue = transform.rotation.eulerAngles.z;
            rotActive = true;
        }
        if (noRotationX)
        {
            xValue = transform.rotation.eulerAngles.x;
            rotActive = true;
        }
    }

    void LateUpdate()
    {
        if (stayInPlace && posActive)
            transform.position = initialPos;

        if (rotActive)
        {
            if (!noRotationY)
                yValue = transform.rotation.eulerAngles.y;
            if (!noRotationX)
                xValue = transform.rotation.eulerAngles.x;
            if (!noRotationZ)
                zValue = transform.rotation.eulerAngles.z;

            transform.rotation = Quaternion.Euler(xValue, yValue, zValue);
        }
    }
}
