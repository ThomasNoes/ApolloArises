using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterObjectToPlayArea : MonoBehaviour
{
    private Vector3 playAreaCenter;

    void Start()
    {
        Vector3[] playAreaLocation = OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);

        Debug.Log("geo first vector: " + playAreaLocation[0]);
        Debug.Log("Play area size: " + OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea));
    }

}
