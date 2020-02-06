using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

[RequireComponent(typeof(Valve.VR.InteractionSystem.Interactable))]
public class ButtonPress : MonoBehaviour
{
    public UnityEvent triggerEvent;

    public SteamVR_Action_Boolean interactionButton;

    void HandHoverUpdate(Valve.VR.InteractionSystem.Hand hand)
    {
        if (interactionButton.GetStateDown(hand.handType))
        {
            //Debug.Log (hand.handType + " clicked " + gameObject.name);
            Debug.Log("Clicked " + gameObject.name);
            triggerEvent.Invoke();
        }
    }
}
