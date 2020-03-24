using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMPAnimated : MonoBehaviour
{

    private TextMeshProUGUI target;
    private DialogData dialog;

    public bool isRunning = false;

    public bool ReadText(DialogData newDialog, TextMeshProUGUI Target)
    {
        isRunning = true;
        target = Target;
        dialog = newDialog;
        StartCoroutine(Read());

        Debug.Log("readtext done");
        return true;
    }

    IEnumerator Read()
    {
        Debug.Log("here");
        target.text = "";

        WaitForSeconds delay = new WaitForSeconds(1f/dialog.textSpeed);

        int i = 0;

        while (i < dialog.text.Length)
        {
            target.text += dialog.text[i];
            yield return delay;
            i++;
        }

        isRunning = false;
        yield return null;
    }
}
