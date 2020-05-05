using UnityEngine;

public class Lever : MonoBehaviour
{
    public int inMaze;
    private bool leverActivated;
    private BeaconManager beaconManager;

    private void Start()
    {
        beaconManager = FindObjectOfType<BeaconManager>();
    }

    public void ActivateBeacon()
    {
        if (leverActivated)
            return;

        beaconManager?.ConnectNextBeacon(inMaze, false);
        leverActivated = true;
    }
}