using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMPAnimated : MonoBehaviour
{

    private TextMeshProUGUI target;
    private DialogData dialog;

    public void ReadText(DialogData newDialog, TextMeshProUGUI Target)
    {
        target = Target;
        dialog = newDialog;
        StartCoroutine(Read());
    }

    IEnumerator Read()
    {
        Debug.Log("here");
        target.text = "";

        WaitForSeconds delay = new WaitForSeconds(1f);

        int i = 0;

        while (i < dialog.text.Length)
        {
            target.text += dialog.text[i];
            yield return delay;
            i++;
        }

        yield return delay;
    }
}
