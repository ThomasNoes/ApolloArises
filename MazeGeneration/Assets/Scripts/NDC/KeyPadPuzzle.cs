using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPadPuzzle : MonoBehaviour
{
    public int password;
    public GameObject safeHandle;
    public GameObject safeDoor;
    private int i = 0;
    private int currentNumber = 0;


    public void InputNumber(int number)
    {
        Debug.Log("Invoked");

        currentNumber = currentNumber + number * (int)Mathf.Pow(10, 3 - i % 4);

        Debug.Log(currentNumber);

        if (i % 4 == 3)
        {
            if (currentNumber == password)
            {
                // Success
                Debug.Log("CORRECT PASSWORD");
                FindObjectOfType<AudioManager>().Play("UnlockSound");
                safeHandle.GetComponent<Valve.VR.InteractionSystem.Interactable>().enabled = true;
                safeHandle.GetComponent<Valve.VR.InteractionSystem.Interactable>().highlightOnHover = true;
                safeHandle.GetComponent<Valve.VR.InteractionSystem.CircularDrive>().enabled = true;
            }
            currentNumber = 0;
        }
        i++;
    }

    public void valveUnlock()
    {
        safeDoor.GetComponent<Valve.VR.InteractionSystem.Interactable>().enabled = true;
        safeDoor.GetComponent<Valve.VR.InteractionSystem.Interactable>().highlightOnHover = true;
        safeDoor.GetComponent<Valve.VR.InteractionSystem.CircularDrive>().enabled = true;
    }

    public void ButtonPressSound()
    {
        FindObjectOfType<AudioManager>().Play("KeypadPress");
    }
}
