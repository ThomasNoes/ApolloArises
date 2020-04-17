using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beacon : MonoBehaviour
{
    public bool isActive;
    public Beacon connectedToBack, connectedToForward;
    public GameObject orb;
    private Material orbMaterial;
    private BeaconManager beaconManager;

    private void Start()
    {
        if (orb != null)
        {
            orbMaterial = orb.GetComponent<Material>();

            if (orbMaterial != null)
            {
                orbMaterial.DisableKeyword("_EMISSION");
                orbMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                orbMaterial.SetColor("_EmissionColor", Color.black);
            }
        }

        beaconManager = GetComponent<BeaconManager>();

        if (beaconManager == null)
            beaconManager = FindObjectOfType<BeaconManager>();

        if (beaconManager != null)
            beaconManager.beacons.Add(this);
    }

    public void LightBeacon()
    {
        Debug.Log("Beacon on " + gameObject.name + " is lit!");

        if (orb != null)
        {
            // TODO
        }
    }
}
