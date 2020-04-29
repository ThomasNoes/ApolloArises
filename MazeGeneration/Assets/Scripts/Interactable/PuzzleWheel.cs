using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleWheel : MonoBehaviour
{
    public int spins = 0, amountToActivate = 3;
    public float cooldownTime = 0.3f;
    public PuzzleRobot puzzleRobotRef;
    public bool notFixed;
    private bool cooldown, goingForward, activated;
    private float prevYAngle;
    private HingeJoint hingeJoint;
    private Rigidbody rb;
    private MeshRenderer mr;
    private GameObject handle;
    private Vector3 anchor, axis, initialPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        hingeJoint = GetComponent<HingeJoint>();

        if (hingeJoint == null)
            return;

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
        //initialPos = transform.position;
        //stayInPlace = true;
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

    public void Enable()
    {
        if (rb == null || mr == null)
            return;

        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezePositionX;
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        rb.constraints = RigidbodyConstraints.FreezePositionZ;
        rb.constraints = RigidbodyConstraints.FreezeRotationX;
        rb.constraints = RigidbodyConstraints.FreezeRotationZ;

        mr.enabled = true;
        handle?.SetActive(true);
    }

    public void Disable()
    {
        if (rb == null || mr == null)
            return;

        rb.constraints = RigidbodyConstraints.FreezeAll;

        mr.enabled = false;
        handle?.SetActive(false);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("CogWheel"))
        {
            if (notFixed)
            {
                notFixed = false;
                Enable();
            }
        }
    }
}
