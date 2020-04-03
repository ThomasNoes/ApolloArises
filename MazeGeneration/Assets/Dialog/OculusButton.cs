using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class OculusButton : MonoBehaviour
{
    public bool isProceed, doNotDisableOnPress;

    public AudioManager am;
    public DialogReader dr;

    [System.Serializable]
    public class ButtonEvent : UnityEvent { }

    public float pressLength;
    public bool pressed;
    public ButtonEvent downEvent;

    Vector3 startPos;
    Rigidbody rb;

    bool registerPress = false, btnInactive, delayRunning;

    void Start()
    {
        if (am == null)
            am = GetComponent<AudioManager>();

        rb = GetComponent<Rigidbody>();

        if (rb != null)
            rb.sleepThreshold = 0.0f;

        startPos = transform.localPosition;
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
            BtnPressed();
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

    public void BtnPressed()
    {
        if (!pressed)
        {
            if (registerPress)
            {
                pressed = true;
                // If we have an event, invoke it
                if (!btnInactive)
                {
                    btnInactive = true;
                    Invoke("DelayedBtnActivate", 1.0f);
                    if (isProceed)
                    {
                        ProceedEvent();
                    }
                    else
                    {
                        downEvent?.Invoke();

                        if (!doNotDisableOnPress)
                            Invoke("SetToNotActive", 0.5f);
                    }
                }
            }
        }
    }

    private void DelayedBtnActivate()
    {
        btnInactive = false;
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

#if UNITY_EDITOR
[CustomEditor(typeof(OculusButton))]
public class OculusButtton_Inspector : UnityEditor.Editor
{
    private GUIStyle headerStyle;

    public override void OnInspectorGUI()
    {
        var script = target as OculusButton;

        DrawDefaultInspector(); // for other non-HideInInspector fields

        if (GUILayout.Button("Press Button"))
        {
            script.BtnPressed();
        }
    }
}
#endif