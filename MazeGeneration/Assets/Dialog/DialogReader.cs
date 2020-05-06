﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DialogReader : MonoBehaviour
{
    public bool toNextScene;
    public bool EnableEvent;

    public DialogData[] dialogs;
    public DialogData[] branchedDialogs;
    public TextMeshProUGUI TMP_Object;
    public TestSceneManager tsm;
    private TMPAnimated tmpa;

    DialogData latestDialog;
    public int branchedIndex = 0;
    public int index = 0;

    public UnityEvent EndEvent;


    // Start is called before the first frame update
    void Start()
    {
        if (tmpa == null)
            tmpa = GetComponent<TMPAnimated>();
    }

    // Update is called once per frame
    public void InjectDialog(DialogData dialog)
    {
        if (!tmpa.isRunning)
        {
            tmpa.ReadText(dialog, TMP_Object);
            latestDialog = dialog;
        }
    }

    public void SetBranchedDialogs(DialogData[] branchedDialogs)
    {
        this.branchedDialogs = branchedDialogs;
    }

    public void DisplayBranchedDialog()
    {
        if (!tmpa.isRunning && branchedIndex < branchedDialogs.Length)
        {
            tmpa.ReadText(branchedDialogs[branchedIndex], TMP_Object);
            latestDialog = branchedDialogs[branchedIndex];
            branchedIndex++;
        }
    }

    public void DisplayAllBranchedDialog()
    {
        StartCoroutine("GoThroughBranchedDialog");
    }

    private IEnumerator GoThroughBranchedDialog()
    {
        while (branchedIndex < branchedDialogs.Length)
        {
            DisplayBranchedDialog();
            
            yield return new WaitForSeconds(CalculateWaitTime(latestDialog));
        }
    }
    public void DisplayAllMainDialog()
    {
        StartCoroutine("GoThroughMainDialog");
    }

    private IEnumerator GoThroughMainDialog()
    {
        while (index <= dialogs.Length)
        {
            DisplayDialog();
            yield return new WaitForSeconds(CalculateWaitTime(latestDialog));
        }
    }


    public void DisplayBranchedDialogAtIndex(int i)
    {
        if (!tmpa.isRunning)
        {
            tmpa.ReadText(branchedDialogs[i], TMP_Object);
            latestDialog = branchedDialogs[i];
        }

    }

    public void DisplayDialogAtIndex(int i)
    {
        if (!tmpa.isRunning)
        {
            tmpa.ReadText(dialogs[i], TMP_Object);
            latestDialog = dialogs[i];
        }
    }

    public void RepeatlatestDialog()
    {
        if (!tmpa.isRunning)
        {
            tmpa.ReadText(latestDialog, TMP_Object);

        }
    }

    public void DisplayDialog()
    {
        if (!tmpa.isRunning && index < dialogs.Length)
        {
            tmpa.ReadText(dialogs[index], TMP_Object);
            latestDialog = dialogs[index];
            index++;

        }
        else if (index == dialogs.Length && toNextScene)
        {
            //Debug.Log("next scne!");
            FinalTestSceneManager.instance.GoToNextScene();
        }
        else if (index == dialogs.Length && EnableEvent)
        {
            EndEvent.Invoke();
        }

    }

    private float CalculateWaitTime(DialogData dd, float scale = 2f)
    {
        //Debug.Log((float)dd.text.Length * (1 / (float)dd.textSpeed) * scale);
        return (float)dd.text.Length * (1 / (float)dd.textSpeed)*scale;
    }

    public void TestEvent()
    {
        Debug.Log("spawn a key");
    }

}
