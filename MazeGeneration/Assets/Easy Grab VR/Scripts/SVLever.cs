using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/**
 * Creates a VRLever. Heads up, we're using Euler Angles here so don't try to lever around the 360 angle.  Things will break!
 */ 
[RequireComponent(typeof(HingeJoint), typeof(SVGrabbable))]
public class SVLever : MonoBehaviour {

    public float leverOnAngle = -60;
    public float leverOffAngle = 60;

    public bool leverIsOn = false;
    public bool oneTimeUse = false;
    public bool beaconLever = false;
    public bool leverWasSwitched = false;

    private HingeJoint leverHingeJoint;
    // private Rigidbody rb;

    private SVGrabbable grabbable;
    private Lever lever;
    private bool wasGrabbed = false, isActive = false, startedOnce = false;

    private Vector3 startingEuler, anchor, axis;

    void Start ()
    {

        if (startedOnce)
            return;

        startedOnce = true;
        leverHingeJoint = GetComponent<HingeJoint>();
        // rb = GetComponent<Rigidbody>();

        anchor = leverHingeJoint.anchor;
        axis = leverHingeJoint.axis;

        //JointLimits limits = leverHingeJoint.limits;
        //limits.max = Mathf.Max(leverOnAngle, leverOffAngle);
        //limits.min = Mathf.Min(leverOnAngle, leverOffAngle);
        //leverHingeJoint.limits = limits;
        //leverHingeJoint.useLimits = true;

        // Get a grabbable on the Lever or one of it's children. You could technically have the grabbable outside of the lever
        // And connect it with a fixed joint, if so just set grabbable to public and set it in editor.
        grabbable = GetComponent<SVGrabbable>();

        startingEuler = transform.localEulerAngles;

        lever = transform.parent.GetComponent<Lever>();
        Invoke("DelayedStart", 0.6f);
    }

    private void DelayedStart()
    {
        leverHingeJoint.anchor = anchor;
        leverHingeJoint.axis = axis;
        isActive = true;
    }

    private void OnEnable()
    {
        if (isActive)
        {
            leverHingeJoint.anchor = anchor;
            leverHingeJoint.axis = axis;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (!isActive || grabbable == null)
            return;

        leverWasSwitched = false;

        float offDistance = Quaternion.Angle(transform.localRotation, OffHingeAngle());
        float onDistance = Quaternion.Angle(transform.localRotation, OnHingeAngle());

        bool shouldBeOn = (Mathf.Abs(onDistance) < Mathf.Abs(offDistance));
        if (shouldBeOn != leverIsOn) {
            leverIsOn = !leverIsOn;
            leverWasSwitched = true;

            if (beaconLever && grabbable.inHand)
            {
                ActivateBeacon();
                grabbable.ClearActiveController();
            }

            if (oneTimeUse)
                isActive = false;
        }

        if (wasGrabbed != grabbable.inHand) {
            wasGrabbed = grabbable.inHand;
        }
	}

    private Quaternion OnHingeAngle() {
        return Quaternion.Euler(leverHingeJoint.axis * leverOnAngle + startingEuler);
    }

    private Quaternion OffHingeAngle() {
        return Quaternion.Euler(leverHingeJoint.axis * leverOffAngle + startingEuler);
    }

    private void ActivateBeacon()
    {
        lever?.ActivateBeacon();
    }
}