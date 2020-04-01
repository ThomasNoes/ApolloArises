using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TestCompanionScaler : MonoBehaviour
{
    public string tileId;
    public GameObject mainScreen;
    private GameObject tileObj;

    public void Start()
    {
        if (String.IsNullOrEmpty(tileId))
            return;

        tileObj = GameObject.Find("Tile " + tileId);

        if (tileObj != null)
        {
            transform.position = tileObj.transform.position;
        }
    }
}
