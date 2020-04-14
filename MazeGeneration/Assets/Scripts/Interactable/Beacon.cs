using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beacon : MonoBehaviour
{
    public bool isActive;
    public Beacon connectedToBack, connectedToForward;
    public GameObject light;
    private BeaconManager beaconManager;

    private void Start()
    {
        if (light != null)
            light.SetActive(false);

        beaconManager = GetComponent<BeaconManager>();

        if (beaconManager == null)
            beaconManager = FindObjectOfType<BeaconManager>();

        if (beaconManager != null)
            beaconManager.beacons.Add(this);
    }

    public void LightBeacon()
    {
        Debug.Log("Beacon on " + gameObject.name + " is lit!");
        
        if (light != null)
            light.SetActive(true);
    }
}
