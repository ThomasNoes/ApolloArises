using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogReader : MonoBehaviour
{
    public bool toNextScene;
    public DialogData[] dialogs;
    public DialogData[] branchedDialogs;
    public TextMeshProUGUI TMP_Object;
    public TestSceneManager tsm;
    private TMPAnimated tmpa;

    public int index = 0;

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
        }
    }

    public void SetBranchedDialogs(DialogData[] branchedDialogs)
    {
        this.branchedDialogs = branchedDialogs;
    }

    public void DisplayBranchedDialog()
    {
        if (!tmpa.isRunning && index < branchedDialogs.Length)
        {
            tmpa.ReadText(branchedDialogs[index], TMP_Object);
            index++;
        }
    }



    public void DisplayBranchedDialogAtIndex(int i)
    {
        if (!tmpa.isRunning)
        {
            tmpa.ReadText(branchedDialogs[i], TMP_Object);
        }

    }
    public void repeatCurrentBranchedDialogAtIndex(int i)
    {
        if (!tmpa.isRunning)
        {
            tmpa.ReadText(branchedDialogs[i], TMP_Object);
        }

    }

    public void DisplayDialogAtIndex(int i)
    {
        if (!tmpa.isRunning)
        {
            tmpa.ReadText(dialogs[i], TMP_Object);
        }
    }

    public void RepeatCurrentDialog()
    {
        if (!tmpa.isRunning)
        {
            tmpa.ReadText(dialogs[index], TMP_Object);
        }
    }

    public void DisplayDialog()
    {
        if (!tmpa.isRunning && index < dialogs.Length)
        {
            tmpa.ReadText(dialogs[index], TMP_Object);
            index++;
        }
        else if (index == dialogs.Length && toNextScene)
        {
            //Debug.Log("next scne!");
            tsm?.NextSceneRandom();
        }

    }
}
