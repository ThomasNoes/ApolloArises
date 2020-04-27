using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HingeJoint))]
public class PuzzleWheel : MonoBehaviour
{
    public int spins = 0, amountToActivate = 3;
    public float cooldownTime = 0.3f;
    public PuzzleRobot puzzleRobotRef;
    private bool cooldown, goingForward, activated;
    private float prevYAngle;
    private HingeJoint hingeJoint;
    private Vector3 anchor, axis;

    private void Start()
    {
        hingeJoint = GetComponent<HingeJoint>();
        anchor = hingeJoint.anchor;
        axis = hingeJoint.axis;

        Invoke("DelayedStart", 0.2f);
    }

    private void DelayedStart()
    {
        hingeJoint.anchor = anchor;
        hingeJoint.axis = axis;
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
