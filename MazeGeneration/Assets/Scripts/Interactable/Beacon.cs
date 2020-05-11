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
    [HideInInspector] public List<Door> doorRefs = new List<Door>();
    [HideInInspector] public List<PuzzleRobot> puzzleRobotRefs = new List<PuzzleRobot>();

    private Renderer orbRenderer;
    private Animator animator;
    private BeaconManager beaconManager;
    private Color startColor, endColor;
    private AudioSource audioSource;
    public GameObject electricity;

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

        foreach (var puzzleRobot in puzzleRobotRefs)
        {
            puzzleRobot.TurnOn();
        }

        foreach (var door in doorRefs)
        {
            door.PowerDoor();
        }
    }

    private IEnumerator PlayBeaconSoundWithDelay()
    {
        PlaySound(closingRoof);
        yield return new WaitForSeconds(6.0f);
        PlaySound(orbOn);
        electricity.SetActive(true);
    }

    private void PlaySound(AudioClip audioClip)
    {
        if (audioClip == null || audioSource == null)
            return;

        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
