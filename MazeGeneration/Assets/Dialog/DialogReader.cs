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

    public int branchedIndex = 0;
    public int index = 0;

    private IEnumerator coroutine;

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
        if (!tmpa.isRunning && branchedIndex < branchedDialogs.Length)
        {
            tmpa.ReadText(branchedDialogs[branchedIndex], TMP_Object);
            branchedIndex++;
        }
    }

    public void DisplayAllBranchedDialog()
    {
        StartCoroutine("GoThroughBranchedDialog");
    }

    private IEnumerator GoThroughBranchedDialog()
    {
        while (branchedIndex != branchedDialogs.Length)
        {
            DisplayBranchedDialog();
            yield return new WaitForSeconds(2);
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
