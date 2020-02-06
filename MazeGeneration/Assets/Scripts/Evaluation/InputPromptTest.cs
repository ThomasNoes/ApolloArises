using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class InputPromptTest : MonoBehaviour
{
    public bool enablePrompt;
    public float vibrationDuration = 1;
    public float frequency = 50;

    public SteamVR_Action_Vibration hapticAction;
    public SteamVR_Input_Sources inputSource;
    bool waitingForInput = false;

    public SteamVR_Action_Boolean touchPad;
    public SteamVR_Action_Boolean trigger;


    void Update()
    {
        if (touchPad.GetState(inputSource) && trigger.GetState(inputSource) && waitingForInput == true)
        {
            // Take current player position and save to heatmap
            Debug.Log("Pressed yes!");
            waitingForInput = false;
            GameObject.Find("VRCamera").GetComponent<PlayerTracker>().logPosition = false;
        }
    }

    public void promptPlayer()  // Call this to prompt input from player
    {
        if (enablePrompt) 
        {
            hapticAction.Execute(0, vibrationDuration, frequency, 1, inputSource);
            waitingForInput = true;  
        }

    }
}
