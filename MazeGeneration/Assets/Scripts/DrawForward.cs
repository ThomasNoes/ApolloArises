using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawForward : MonoBehaviour
{
    public GameObject gizmo;

    Vector3 center;

    Vector3 forward;

    Vector3[] points;

    // Start is called before the first frame update
    void Start()
    {
        points = GuardainCalibration.GetBoundaryPoints();
        GuardainCalibration.Calibrate(out center, out forward);

        foreach (Vector3 p in points)
        {
            DrawGizmo(p);
        }
        DrawGizmo(center);
        DrawGizmo(forward);
    }

    private void DrawGizmo(Vector3 p, float scale = 0.05f )
    {
        GameObject temp = Instantiate(gizmo, gameObject.transform);
        temp.transform.position = new Vector3(p.x, 0, p.z);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
