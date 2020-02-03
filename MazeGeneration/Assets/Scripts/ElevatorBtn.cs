using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ElevatorBtn : MonoBehaviour
{
    public bool downButton = false;
    public SteamVR_Action_Boolean trigger;
    public SteamVR_Input_Sources inputSource = SteamVR_Input_Sources.Any;

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Collision!! " + other.tag);
        if (other.tag == ("Controller"))
        {
            if (trigger.GetStateDown(inputSource))
            {
                if (downButton)
                {
                    gameObject.GetComponentInParent<ElevatorMovement>().DownBtn();
                }
                else
                {
                    gameObject.GetComponentInParent<ElevatorMovement>().UpBtn();
                }
            }
        }
    }
}
