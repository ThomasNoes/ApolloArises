using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class TestCompanionScaler : MonoBehaviour
{
    public bool enable, firstTile;
    public GameObject mainScreen, smallScreen;
    public GameObject mapManager;
    public TextMeshProUGUI text;
    private GameObject tileObj, mazeObj;
    MapManager mm;

    public void Start()
    {
        if (!enable)
            return;

        if (firstTile)
            tileObj = GameObject.Find("Tile R0C0");
        else
            tileObj = GameObject.Find("Tile R2C2");

        if (firstTile)
        {
            Quaternion rot = Quaternion.Euler(0, 90, 0) * mapManager.transform.rotation;
            transform.rotation = rot;
        }
        else
        {
            Quaternion rot = Quaternion.Euler(0, 270, 0) * mapManager.transform.rotation;
            transform.rotation = rot;
        }

        if (mapManager != null)
            mazeObj = mapManager.transform.GetChild(0).gameObject;



        if (tileObj != null)
        {
            PlaceOnTile();
        }
    }

    public void ScaleWithMaze()
    {
        if (mainScreen == null || mazeObj == null || !enable)
            return;

        mainScreen.transform.localScale = new Vector3(mazeObj.transform.localScale.x, mazeObj.transform.localScale.x, 1.0f);

        if (smallScreen != null)
            smallScreen.transform.localScale = new Vector3(mazeObj.transform.localScale.x, mazeObj.transform.localScale.x, 1.0f);

        if (text != null)
            text.fontSize = text.fontSize + 0.01f;

        PlaceOnTile();
    }

    private void PlaceOnTile()
    {
        if (firstTile)
            transform.position = new Vector3(tileObj.transform.position.x, tileObj.transform.position.y, tileObj.transform.position.z);
        else
            transform.position = new Vector3(tileObj.transform.position.x, tileObj.transform.position.y, tileObj.transform.position.z);
    }
}
