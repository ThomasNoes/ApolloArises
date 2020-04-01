using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TestCompanionScaler : MonoBehaviour
{
    public bool firstTile;
    public GameObject mainScreen;
    public GameObject mapManager;

    private GameObject tileObj;

    public void Start()
    {
        if (firstTile)
            tileObj = GameObject.Find("Tile R0C0");
        else
            tileObj = GameObject.Find("Tile R2C2");

        if (firstTile)
        {
            Quaternion rot = Quaternion.Euler(0, 90, 0);
            transform.rotation = rot;
        }
        else
        {
            Quaternion rot = Quaternion.Euler(0, 270, 0);
            transform.rotation = rot;
        }
            

        if (tileObj != null)
        {
            PlaceOnTile();
        }
    }

    public void ScaleWithMaze()
    {
        if (mainScreen == null || mapManager == null)
            return;

        mainScreen.transform.localScale = new Vector3(mapManager.transform.GetChild(0).localScale.x, mapManager.transform.GetChild(0).localScale.x, 1.0f);
        PlaceOnTile();
    }

    private void PlaceOnTile()
    {
        if (firstTile)
            transform.position = new Vector3(tileObj.transform.position.x - (tileObj.transform.localScale.x / 2.0f), tileObj.transform.position.y, tileObj.transform.position.z + (tileObj.transform.localScale.z / 4.0f));
        else
            transform.position = new Vector3(tileObj.transform.position.x, tileObj.transform.position.y, tileObj.transform.position.z + (tileObj.transform.localScale.z / 4.0f));
    }
}
