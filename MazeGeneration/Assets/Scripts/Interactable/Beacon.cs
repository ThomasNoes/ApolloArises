using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Beacon : MonoBehaviour
{
    public bool isActive;
    public Beacon connectedToBack, connectedToForward;
    public GameObject orb;
    public AudioClip closingRoof, orbOn;

    private Renderer orbRenderer;
    private Animator animator;
    private BeaconManager beaconManager;
    private Color startColor, endColor;
    private AudioSource audioSource;

    private void Start()
    {
        animator = GetComponent<Animator>();
        beaconManager = GetComponent<BeaconManager>();
        audioSource = GetComponent<AudioSource>();

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
        StartCoroutine(PlayBeaconSoundWithDelay());

        if (orbRenderer != null && beaconManager != null)
        {
            orbRenderer.material.EnableKeyword("_EMISSION");
            orbRenderer.material.SetColor("_EmissionColor", beaconManager.orbEndEmission);
        }
    }

    private IEnumerator PlayBeaconSoundWithDelay()
    {
        PlaySound(closingRoof);
        yield return new WaitForSeconds(6.0f);
        PlaySound(orbOn);
    }

    private void PlaySound(AudioClip audioClip)
    {
        if (audioClip == null || audioSource == null)
            return;

        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
