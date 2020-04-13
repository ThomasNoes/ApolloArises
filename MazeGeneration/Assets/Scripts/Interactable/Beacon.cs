using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beacon : MonoBehaviour
{
    public bool isActive;
    public Beacon connectedToBack, connectedToForward;
    private BeaconManager beaconManager;

    private void Start()
    {
        beaconManager = GetComponent<BeaconManager>();

        if (beaconManager == null)
            beaconManager = FindObjectOfType<BeaconManager>();

        if (beaconManager != null)
            beaconManager.beacons.Add(this);
    }

    public void LightBeacon()
    {
        Debug.Log("Beacon on " + gameObject.name + " is lit!");
        // TODO turn on some effect
    }
}
