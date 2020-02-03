using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fuseSocketHandler : MonoBehaviour
{
    public bool occupiedSocket = false;

    private void OnTriggerEnter(Collider other)
    {
        occupiedSocket = true;
    }

    private void OnTriggerExit(Collider other)
    {
        occupiedSocket = false;
    }
}
