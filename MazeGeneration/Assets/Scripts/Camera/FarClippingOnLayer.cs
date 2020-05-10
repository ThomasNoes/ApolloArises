using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarClippingOnLayer : MonoBehaviour
{
    public float clippingAmount;
    [Range(0, 31)]public int onLayer;
    public bool autoSetDefault = false;
    public float defaultClippingValue;
    private float[] distances;
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;

        if (autoSetDefault)
            defaultClippingValue = mainCam.farClipPlane;

        distances = new float[32];

        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = defaultClippingValue;
        }

        distances[onLayer] = clippingAmount;

        mainCam.layerCullDistances = distances;
    }
}
