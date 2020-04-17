using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beacon : MonoBehaviour
{
    public bool isActive;
    public Beacon connectedToBack, connectedToForward;
    public GameObject orb;
    private Renderer orbRenderer;
    private Animator animator;
    private BeaconManager beaconManager;
    private Color startColor, endColor;

    private void Start()
    {
        animator = GetComponent<Animator>();
        beaconManager = GetComponent<BeaconManager>();

        if (beaconManager == null)
            beaconManager = FindObjectOfType<BeaconManager>();

        if (orb != null && beaconManager != null)
        {
            orbRenderer = orb.GetComponent<Renderer>();
            beaconManager.beacons.Add(this);

            if (orbRenderer != null)
            {
                orbRenderer.material.DisableKeyword("_EMISSION");
                orbRenderer.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                orbRenderer.material.SetColor("_EmissionColor", beaconManager.orbStartEmission);
            }
        }
    }

    public void LightBeacon()
    {
        //Debug.Log("Beacon on " + gameObject.name + " is lit!");
        animator?.SetBool("Close", true);

        if (orbRenderer != null && beaconManager != null)
        {
            orbRenderer.material.EnableKeyword("_EMISSION");
            orbRenderer.material.SetColor("_EmissionColor", beaconManager.orbEndEmission);
        }
    }
}
