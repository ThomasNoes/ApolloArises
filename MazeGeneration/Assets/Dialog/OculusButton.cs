using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class OculusButton : MonoBehaviour
{
    public bool isProceed;

    public AudioManager am;
    public DialogReader dr;

    [System.Serializable]
    public class ButtonEvent : UnityEvent { }

    public float pressLength;
    public bool pressed;
    public ButtonEvent downEvent;

    Vector3 startPos;
    Rigidbody rb;

    bool registerPress = false;

    void Start()
    {
        startPos = transform.localPosition;
        rb = GetComponent<Rigidbody>();
        Invoke("DelayRegister", 0.1f) ;
    }

    void DelayRegister()
    {
        registerPress = true;
    }

    void Update()
    {
        // If our distance is greater than what we specified as a press
        // set it to our max distance and register a press if we haven't already
        float distance = Mathf.Abs(transform.localPosition.y - startPos.y);
        if (distance >= pressLength)
        {
            // Prevent the button from going past the pressLength
            transform.localPosition = new Vector3(0, startPos.y - pressLength, 0);
            if (!pressed)
            {
                if (registerPress)
                {
                    pressed = true;
                    // If we have an event, invoke it
                    if (isProceed)
                    {
                        ProceedEvent();
                    }
                    else
                    {
                        downEvent?.Invoke();
                        Invoke("SetToNotActive", 0.5f);
                    }

                }
            }
        }
        else
        {
            // If we aren't all the way down, reset our press
            pressed = false;
        }
        // Prevent button from springing back up past its original position
        if (transform.localPosition.y > startPos.y)
        {

            transform.localPosition = new Vector3(0, startPos.y, 0);
        }
    }

    void SetToNotActive()
    {
        transform.parent.parent.gameObject.SetActive(false);
    }

    void ProceedEvent()
    {
        am.ExternalRaiseAtIndex(0);
        Invoke("ProceedUp", 0.5f);
    }
    void ProceedUp()
    {
        dr.DisplayDialog();
    }

}

