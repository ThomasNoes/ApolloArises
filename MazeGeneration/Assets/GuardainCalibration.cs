using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardainCalibration : MonoBehaviour
{

    public static Vector3[] GetBoundaryPoints()
    {
        return OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
    }

    public static void Calibrate(out Vector3 center, out Vector3 forward)
    {
        Vector3[] boundaryPoints = OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
        IgnoreZ(boundaryPoints);
        center = CalculateCenter(boundaryPoints);
        forward = CalculateForward(center, boundaryPoints);

    }

    private static void IgnoreZ(Vector3[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i].z = 0;
        }
    }

    private static Vector3 CalculateCenter(Vector3[] points)
    {
        //get the sum of points and divide
        Vector3 center = Vector3.zero;
        foreach (Vector3 v in points)
        {
            center += v;
        }
        center = center / points.Length; 
        return center;
    }

    private static Vector3 CalculateForward(Vector3 center, Vector3[] points)
    {
        Vector3 forward = Vector3.zero;
        Vector3 firstEdge = (points[0] + points[1]) / 2;

        forward = (firstEdge - center).normalized;
        return forward;
    }
}
