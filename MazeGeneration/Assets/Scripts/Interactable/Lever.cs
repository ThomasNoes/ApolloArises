using UnityEngine;

public class Lever : MonoBehaviour
{
    public int inMaze;
    private BeaconManager beaconManager;

    private void Start()
    {
        beaconManager = FindObjectOfType<BeaconManager>();
    }

    public void ActivateBeacon()
    {
        beaconManager?.ConnectNextBeacon(inMaze, false);
    }
}
