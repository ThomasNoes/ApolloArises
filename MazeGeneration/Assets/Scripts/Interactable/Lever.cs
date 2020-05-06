using UnityEngine;

public class Lever : MonoBehaviour
{
    public int inMaze;
    private bool leverCooldown;
    private BeaconManager beaconManager;

    private void Start()
    {
        beaconManager = FindObjectOfType<BeaconManager>();
    }

    public void ActivateBeacon()
    {
        if (leverCooldown)
            return;

        beaconManager?.ConnectNextBeacon(inMaze, false);

        leverCooldown = true;
        Invoke("Cooldown", 5.0f);
    }

    private void Cooldown()
    {
        leverCooldown = false;
    }
}