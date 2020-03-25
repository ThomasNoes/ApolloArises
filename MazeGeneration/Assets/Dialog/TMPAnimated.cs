using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMPAnimated : MonoBehaviour
{
    public VoidEvent textAppear, success, surprise, confetti, remark, question;

    private TextMeshProUGUI target;
    private DialogData dialog;

    public bool isRunning = false;

    public bool ReadText(DialogData newDialog, TextMeshProUGUI Target)
    {
        isRunning = true;
        target = Target;
        dialog = newDialog;
        StartCoroutine(Read());

        PlaySound();
      
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
            textAppear.Raise();
            yield return delay;
            i++;
        }

        isRunning = false;
        yield return null;
    }

    void PlaySound()
    {
        switch (dialog.dialogEffect)
        {
            case DialogData.effect.none:
                break;
            case DialogData.effect.success:
                success.Raise();
                break;
            case DialogData.effect.surprise:
                surprise.Raise();
                break;
            case DialogData.effect.confetti:
                confetti.Raise();
                break;
            case DialogData.effect.remark:
                remark.Raise();
                break;
            case DialogData.effect.question:
                question.Raise();
                break;
            default:
                break;
        }
    }
}
