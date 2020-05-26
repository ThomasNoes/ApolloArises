using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarClippingOnLayer : MonoBehaviour
{
    public float farClippingAmount = 100.0f;
    [Range(0, 31)]public int onLayer;
    public bool autoSet = false;
    public float defaultClippingValue = 40.0f;
    private float[] distances;
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;

        if (autoSet)
            farClippingAmount = mainCam.farClipPlane;
        else
            mainCam.farClipPlane = farClippingAmount;

        distances = new float[32];

        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = defaultClippingValue;
        }

        distances[onLayer] = farClippingAmount;

        mainCam.layerCullDistances = distances;
    }
}
