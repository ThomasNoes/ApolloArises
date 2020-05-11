﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleDialogManager : MonoBehaviour
{
    public DialogData functionDialog;
    public DialogData malFunctionDialog;
    public DialogData endDialog;

    public PuzzleRobot pr;
    public PuzzleWheel pw;
    public DialogReader dr;

    bool hasCollid = false;

    Collider col;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        if (!pr.startsFixed)
        {
            col.enabled = false;
        }
    }
    
    public void OnRotateWheelDone()
    {
        //Debug.Log("RotateWheel done");
        dr.InjectDialog(endDialog);
    }

    public void OnRobotFixed()
    {
        //Debug.Log("robot is fixed");
        dr.InjectDialog(malFunctionDialog);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.name == "Player" || other.transform.root.name == "DebugPlayer")
        {
            if (!hasCollid)
            {
                dr.InjectDialog(functionDialog);
            }
            hasCollid = true;
        }
    }

}
