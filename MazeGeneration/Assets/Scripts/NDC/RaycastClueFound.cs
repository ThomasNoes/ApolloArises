using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastClueFound : MonoBehaviour
{
    [Header("Attach this script to camera")]
    public int clueColliderLayer;
    public float activationDistance = 5;
    [Tooltip("In seconds")]
    public int gazeTime;
    public int clueAmount;
    public Camera timeDeviceCam;

    private int currentClue = -1;
    private bool timerStarted = false;
    private bool[] clueActive;
    private float currentTime;
    public NotePadScript notePad;

    void Start()
    {
        clueActive = new bool[clueAmount];
        nextClue();
    }


    void Update()
    {

        int layerMask = 1 << clueColliderLayer;
        RaycastHit hit; RaycastHit hit2;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, activationDistance, layerMask)
            && Physics.Raycast(timeDeviceCam.transform.position, timeDeviceCam.transform.TransformDirection(Vector3.forward), out hit2, activationDistance, layerMask) && currentClue != -1)
        {

            if (!timerStarted && clueActive[currentClue])
            {
                timerStarted = true;
                Debug.Log("Timer Started!");
                currentTime = Time.time;
            }

            if (timerStarted)
            {
                if (gazeTime <= Time.time - currentTime)
                {
                    Debug.Log("Clue found!");
                    clueActive[currentClue] = false;
                    timerStarted = false;
                    notePad.NotepadTriggerUpdatePage();
                }
            }

            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.DrawRay(timeDeviceCam.transform.position, timeDeviceCam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.DrawRay(timeDeviceCam.transform.position,timeDeviceCam.transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            timerStarted = false;
        }

    }

    public void nextClue()
    {
        currentClue++;

        if (currentClue <= clueAmount)
        {
            clueActive[currentClue] = true;
        }
    }
}
