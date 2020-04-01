using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSceneManager : MonoBehaviour
{
    public GameObject companion1, companion2;
    private GameObject camObj;

    private void Start()
    {
        // TODO: start logging time, start logging wall hits

        camObj = Camera.main.gameObject;

        if (camObj != null && companion1 != null && companion2 != null)
        {
            if (Vector3.Distance(camObj.transform.position, companion1.transform.position) > Vector3.Distance(camObj.transform.position, companion2.transform.position))
            {
                // TODO: choose which companion to be enabled at start
            }
        }
    }

    public void NextSceneRandom()
    {
        Debug.Log("Next random scene!");
    }

    public void NextSceneAtIndex(int index)
    {

    }

    public void NextSceneLastIndex()
    {

    }
}
