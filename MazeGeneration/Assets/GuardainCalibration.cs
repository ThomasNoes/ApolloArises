﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardainCalibration : MonoBehaviour
{

    public bool calibrateGameObject;
    
    public GameObject gizmo;

    public GameObject gizmoRed;

    Vector3 centerOffset;

    Vector3[] points;

    Vector3 c;
    Vector3 f;

    // Start is called before the first frame update
    void Awake()
    {
        if (!Application.isEditor)
        {
            Calibrate(out c, out f);
            if (calibrateGameObject)
            {
                transform.position = new Vector3(c.x, 0, c.z);
                //Debug.Log("Pos: " + transform.position);
                transform.forward = c - f;
                //Debug.Log("Rot: " + transform.rotation.eulerAngles);
            }
        }
    }

    public void Calibrate(out Vector3 center, out Vector3 forward)
    {
        points = GetBoundaryPoints();
        IgnoreY(points);
        SetReferencePoints(out center, out forward);
        DrawCalibrationArea(center, forward);

        //cube.position = new Vector3(center.x, cube.position.y, center.z);

       // cube.forward = center - forward;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DrawCalibrationArea(Vector3 center, Vector3 forward)
    {
        for (int i = 0; i < points.Length; i++)
        {
            //Debug.Log("Corner: " + points[i]);
            DrawGizmo(points[i],gizmo, 0.1f);
        }
        DrawGizmo(center, gizmoRed,0.1f);
        //Debug.Log("Center: " + center);
        DrawGizmo(forward, gizmoRed, 0.1f);
        //Debug.Log("Forward: " + forward);
    }

    private void DrawGizmo(Vector3 p, GameObject gizmo, float scale = 0.05f)
    {
        GameObject temp = Instantiate(gizmo, new Vector3(0,0,0),Quaternion.identity);
        temp.transform.position = new Vector3(p.x, p.y, p.z);
        temp.transform.localScale = new Vector3(scale, scale, scale);

    }

    public Vector3[] GetBoundaryPoints()
    {
        return OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
    }

    public void ScaleObject()
    {
        transform.localScale = new Vector3(GetWidth(points), 0.01f, GetLength(points));
    }

    public float GetWidth(Vector3[] points)
    {
        //Debug.Log("Width: "+GetDistance(points[0], points[1]));
        return GetDistance(points[0], points[1]);
    }

    public float GetLength(Vector3[] points)
    {
        //Debug.Log("length: " + GetDistance(points[1], points[2]));
        return GetDistance(points[1], points[2]);
    }

    private float GetDistance(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }


    public void SetReferencePoints(out Vector3 center, out Vector3 forward)
    {
        Vector3[] boundaryPoints = OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
        IgnoreY(boundaryPoints);

        center = CalculateCenter(boundaryPoints);
        forward = CalculateForward(center, boundaryPoints);

    }

    private void IgnoreY(Vector3[] points)
    {

        for (int i = 0; i < points.Length; i++)
        {
            points[i].y = 1;
        }
    }

    public Vector3 CalculateCenter(Vector3[] points)
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

    private Vector3 CalculateForward(Vector3 center, Vector3[] points)
    {
        Vector3 forward = Vector3.zero;
        Vector3 firstEdge = (points[0] + points[1]) / 2;

        forward = Vector3.Lerp(points[0],points[1],0.5f);
        return forward;
    }
}
