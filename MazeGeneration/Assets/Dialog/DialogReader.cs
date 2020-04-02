using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogReader : MonoBehaviour
{
    public bool toNextScene;
    public DialogData[] dialogs;
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
    void Update()
    {
        
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
            Debug.Log("next scne!");
            tsm.NextSceneRandom();
        }

    }
}
