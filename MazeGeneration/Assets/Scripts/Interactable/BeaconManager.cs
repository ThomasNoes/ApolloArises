using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(LineRenderer))]
public class BeaconManager : MonoBehaviour
{
    public List<Beacon> beacons = new List<Beacon>();
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void ConnectNextBeacon()
    {
        for (int i = 0; i < beacons.Count; i++)
        {
            if (!beacons[i].isActive)
            {
                beacons[i].isActive = true;
                beacons[i].LightBeacon();

                if (i > 0)
                    ConnectBeam(i, i - 1);

                break;
            }
        }
    }

    private void ConnectBeam(int indexTo, int indexFrom)
    {
        Debug.Log("Beacon on " + beacons[indexFrom].gameObject.name + " is connected to " + beacons[indexTo].gameObject.name);
        beacons[indexFrom].connectedToForward = beacons[indexTo];
        beacons[indexTo].connectedToBack = beacons[indexFrom];

        lineRenderer.positionCount++;

        if (beacons[indexFrom].light != null && beacons[indexTo].light != null)
        {
            lineRenderer.SetPosition(indexFrom, beacons[indexFrom].light.transform.position);
            lineRenderer.SetPosition(indexTo, beacons[indexTo].light.transform.position);
        }
        else
        {
            lineRenderer.SetPosition(indexFrom, beacons[indexFrom].gameObject.transform.position);
            lineRenderer.SetPosition(indexTo, beacons[indexTo].gameObject.transform.position);
        }
    }

    public void DisconnectNextBeacon()
    {
        // TODO: not yet implemented - will be implemented if needed.
    }
}

#region CustomInspector
#if UNITY_EDITOR
[CustomEditor(typeof(BeaconManager))]
public class BeaconManager_Editor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        var script = target as BeaconManager;

        DrawDefaultInspector();

        if (GUILayout.Button("Connect next beacon!"))
        {
            script.ConnectNextBeacon();
        }
    }
}
#endif
#endregion