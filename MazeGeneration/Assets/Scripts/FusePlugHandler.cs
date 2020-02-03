using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusePlugHandler : MonoBehaviour
{
    private bool inSocket = false;
    [HideInInspector]
    public GameObject socketGoIn;
    [HideInInspector]
    public GameObject socketGoOut;

    // Update is called once per frame
    void Update()
    {
        if (inSocket)
        {
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (!other.GetComponent<fuseSocketHandler>().occupiedSocket)
        {
            return;
        }

        inSocket = true;
        socketGoIn = other.gameObject;

        transform.parent.GetComponent<Valve.VR.InteractionSystem.Hand>().DetachObject(gameObject);

        gameObject.transform.position = new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z);

        EventCallbacks.FusePuzzlePlugIn fpPlugIn = new EventCallbacks.FusePuzzlePlugIn();
        fpPlugIn.sGo = socketGoIn;
        fpPlugIn.go = gameObject;
        fpPlugIn.FireEvent();

    }

    private void OnTriggerExit(Collider other)
    {

        inSocket = false;
        socketGoOut = other.gameObject;

        EventCallbacks.FusePuzzlePlugOut fpPlugOut = new EventCallbacks.FusePuzzlePlugOut();
        fpPlugOut.sGo = socketGoOut;
        fpPlugOut.go = gameObject;
        fpPlugOut.FireEvent();

    }
}
