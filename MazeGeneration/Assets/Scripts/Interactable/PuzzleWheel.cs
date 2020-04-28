using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
public class PuzzleWheel : MonoBehaviour
{
    public int spins = 0, amountToActivate = 3;
    public float cooldownTime = 0.3f;
    public PuzzleRobot puzzleRobotRef;
    private bool cooldown, goingForward, activated, stayInPlace;
    private float prevYAngle;
    private ConfigurableJoint hingeJoint;
    private Vector3 anchor, axis, initialPos;

    private void Start()
    {
        hingeJoint = GetComponent<ConfigurableJoint>();
        anchor = hingeJoint.anchor;
        axis = hingeJoint.axis;

        hingeJoint.anchor = Vector3.zero;
        hingeJoint.axis = Vector3.zero;

        Invoke("DelayedStart", 0.2f);
    }

    private void DelayedStart()
    {
        hingeJoint.anchor = anchor;
        hingeJoint.axis = axis;
        initialPos = transform.position;
        stayInPlace = true;
    }

    private void FixedUpdate()
    {
        float currentYAngle = transform.rotation.eulerAngles.y;

        if (!currentYAngle.Equals(prevYAngle))
        {
            if (currentYAngle > prevYAngle && Math.Abs(currentYAngle - prevYAngle) < 170)
            {
                //Debug.Log("FORWARD! | " + Math.Abs(currentYAngle - prevYAngle) + " | Prev: " + prevYAngle + " | Current: " + currentYAngle);
                goingForward = true;
            }
            else if (transform.rotation.eulerAngles.y < prevYAngle && Math.Abs(currentYAngle - prevYAngle) < 170)
            {
                //Debug.Log("BACKWARD! | " + Math.Abs(currentYAngle - prevYAngle) + " | Prev: " + prevYAngle + " | Current: " + currentYAngle);
                goingForward = false;
            }

            if (goingForward && !cooldown)
                if (transform.rotation.eulerAngles.y >= 358f)
                {
                    spins++;
                    cooldown = true;
                    Invoke("Cooldown", cooldownTime);

                    if (spins >= amountToActivate && !activated)
                        Activate();
                }
        }

        prevYAngle = currentYAngle;
    }

    private void LateUpdate()
    {
        if (stayInPlace)
            transform.position = initialPos;
    }

    private void Cooldown()
    {
        cooldown = false;
    }

    private void Activate()
    {
        activated = true;
        
        if (puzzleRobotRef != null)
            puzzleRobotRef.SpawnKey();
    }

    private void OnTriggerEnter(Collider col)
    {
        // TODO check when puzzle object collides if this is disabled.
    }
}
