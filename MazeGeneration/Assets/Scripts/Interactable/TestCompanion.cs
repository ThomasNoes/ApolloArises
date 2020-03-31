using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCompanion : MonoBehaviour
{
    public GameObject companionHead;
    private GameObject mainCamObj;

    private void Start()
    {
        if (companionHead != null)
            mainCamObj = Camera.main.gameObject;
    }

    private void LateUpdate()
    {
        if (mainCamObj == null)
            return;


    }
}