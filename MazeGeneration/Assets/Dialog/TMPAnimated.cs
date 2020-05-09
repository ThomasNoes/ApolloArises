using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMPAnimated : MonoBehaviour
{
    [Header("Sound effects")]
    public VoidEvent textAppear;
    public VoidEvent success, surprise, confetti, remark, question;


    public GameObject proceedButton; 

    private TextMeshProUGUI target;
    private DialogData dialog;

    public bool isRunning = false;

    public bool clearText = true;
    float ClearTextTimer = 6.0f;
    float counter = 0;

    private void Update()
    {
        if (!isRunning && target && clearText)
        {
            counter += Time.deltaTime;
            if (counter > ClearTextTimer)
            {
                target.text = "";
            }
        }
    }

    public bool ReadText(DialogData newDialog, TextMeshProUGUI Target)
    {
        counter = 0;
        isRunning = true;
        target = Target;
        dialog = newDialog;
        StartCoroutine(Read());

        PlaySound();

        if (proceedButton)
        {
            RaiseEvent();
        }

      
        return true;
    }

    IEnumerator Read()
    {
        target.text = "";

        WaitForSeconds delay = new WaitForSeconds(1f/dialog.textSpeed);

        int i = 0;

        while (i < dialog.text.Length)
        {
            target.text += dialog.text[i];
            textAppear?.Raise();
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
                success?.Raise();
                break;
            case DialogData.effect.surprise:
                surprise?.Raise();
                break;
            case DialogData.effect.confetti:
                confetti?.Raise();
                break;
            case DialogData.effect.remark:
                remark?.Raise();
                break;
            case DialogData.effect.question:
                question?.Raise();
                break;
            default:
                break;
        }
    }
    void RaiseEvent()
    {
        if (dialog.ButtonEvent != null)
        {
            proceedButton.SetActive(false);
            Debug.Log(dialog.ButtonEvent.name);
            dialog.ButtonEvent.Raise();
        }
        else if (!proceedButton.activeSelf)
        {
            proceedButton.SetActive(true);
        }
        
    }
}
