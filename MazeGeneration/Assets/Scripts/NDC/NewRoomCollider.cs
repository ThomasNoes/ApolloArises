using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRoomCollider : MonoBehaviour
{
    NotePadScript notePad;
    RaycastClueFound clueTrigger;
    private bool isTriggered = false;
    Collider triggerCollider;
    Valve.VR.InteractionSystem.Player player;
    
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Valve.VR.InteractionSystem.Player>();
        notePad = GameObject.Find("Equipment List").GetComponent<NotePadScript>();
        clueTrigger = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<RaycastClueFound>();
        triggerCollider = player.headCollider;
    }

    void OnTriggerEnter(Collider collider)
    {
        
        if (collider == triggerCollider && isTriggered == false)
        {
            isTriggered = true;
            notePad.NotepadEnterNewRoom();
            clueTrigger.nextClue();
        }
    }
}
