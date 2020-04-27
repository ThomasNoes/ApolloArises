using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/**
 * Creates a VRLever. Heads up, we're using Euler Angles here so don't try to lever around the 360 angle.  Things will break!
 */ 
[RequireComponent(typeof(ConfigurableJoint), typeof(SVGrabbable))]
public class SVLever : MonoBehaviour {

    public float leverOnAngle = -60;
    public float leverOffAngle = 60;

    public bool leverIsOn = false;
    public bool oneTimeUse = false;
    public bool beaconLever = false;
    public bool leverWasSwitched = false;

    private ConfigurableJoint leverHingeJoint;

    private SVGrabbable grabbable;
    private Lever lever;
    private bool wasGrabbed = false, isActive = true;

    private Vector3 startingEuler, anchor, axis;

    void Start () {
        leverHingeJoint = GetComponent<ConfigurableJoint>();
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

        startingEuler = this.transform.localEulerAngles;

        UpdateHingeJoint();

        leverHingeJoint.anchor = anchor;
        leverHingeJoint.axis = axis;
        lever = transform.parent.GetComponent<Lever>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (!isActive)
            return;

        leverWasSwitched = false;

        float offDistance = Quaternion.Angle(this.transform.localRotation, OffHingeAngle());
        float onDistance = Quaternion.Angle(this.transform.localRotation, OnHingeAngle());

        bool shouldBeOn = (Mathf.Abs(onDistance) < Mathf.Abs(offDistance));
        if (shouldBeOn != leverIsOn) {
            leverIsOn = !leverIsOn;
            leverWasSwitched = true;

            if (beaconLever)
                ActivateBeacon();

            if (oneTimeUse)
                isActive = false;

            UpdateHingeJoint();
        }

        if (wasGrabbed != grabbable.inHand) {
            wasGrabbed = grabbable.inHand;
            UpdateHingeJoint();
        }
	}

    private void UpdateHingeJoint() {
        //JointSpring spring = leverHingeJoint.spring;

        //if (grabbable.inHand) {
        //    leverHingeJoint.useSpring = false;
        //} else {
        //    if (leverIsOn) {
        //        spring.targetPosition = leverOnAngle;
        //    } else {
        //        spring.targetPosition = leverOnAngle;
        //    }
        //    leverHingeJoint.useSpring = true;
        //}

        //leverHingeJoint.spring = spring;
    }

    private Quaternion OnHingeAngle() {
        return Quaternion.Euler(this.leverHingeJoint.axis * leverOnAngle + startingEuler);
    }

    private Quaternion OffHingeAngle() {
        return Quaternion.Euler(this.leverHingeJoint.axis * leverOffAngle + startingEuler);
    }

    private void ActivateBeacon()
    {
        lever?.ActivateBeacon();
    }
}