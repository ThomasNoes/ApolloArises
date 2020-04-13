using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BeaconManager : MonoBehaviour
{
    public List<Beacon> beacons = new List<Beacon>();

    private void Start()
    {
        
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
                    ConnectBeam(beacons[i], beacons[i - 1]);

                break;
            }
        }
    }

    private void ConnectBeam(Beacon to, Beacon from)
    {
        Debug.Log("Beacon on " + from.gameObject.name + " is connected to " + to.gameObject.name);
        from.connectedToForward = to;
        to.connectedToBack = from;
    }

    public void DisconnectNextBeacon()
    {

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