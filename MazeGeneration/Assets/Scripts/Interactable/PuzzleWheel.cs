using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleWheel : MonoBehaviour
{
    public int spins = 0, amountToActivate = 3;
    public float cooldownTime = 0.3f;
    public PuzzleRobot puzzleRobotRef;
    public PuzzleDialogManager pdm;
    public bool notFixed;
    private bool cooldown, goingForward, activated;
    private float prevYAngle;
    private HingeJoint hingeJoint;
    private Rigidbody rb;
    private MeshRenderer mr;
    public GameObject handle, handleInteractObj;
    private Vector3 anchor, axis, initialPos;
    private AudioSource audioSourceWheel;

    private Quaternion lastRot;
    private Vector3 angularVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mr = GetComponent<MeshRenderer>();
        audioSourceWheel = GetComponent<AudioSource>();
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
                    {
                        pdm?.OnRotateWheelDone();
                        Activate();
                    }
                }
        }

        if (rb != null && handle != null)
        {
            CalculateAngularVelocity();

            if (angularVelocity.magnitude >= 9.0f)
            {
                StartSound();
            }
            else
            {
                StopSound();
            }
        }

        prevYAngle = currentYAngle;
    }

    private void CalculateAngularVelocity()
    {
        var deltaRot = transform.rotation * Quaternion.Inverse(lastRot);
        var eulerRot = new Vector3(Mathf.DeltaAngle(0, deltaRot.eulerAngles.x), Mathf.DeltaAngle(0, deltaRot.eulerAngles.y), Mathf.DeltaAngle(0, deltaRot.eulerAngles.z));

        angularVelocity = eulerRot / Time.fixedDeltaTime;
        lastRot = transform.rotation;
    }


    private void StopSound()
    {
        if (audioSourceWheel == null)
            return;

        if (audioSourceWheel.isPlaying)
        {
            audioSourceWheel.Stop();
        }
    }

    private void StartSound()
    {
        if (audioSourceWheel == null)
            return;

        if (!audioSourceWheel.isPlaying)
        {
            audioSourceWheel.Play();
        }
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

        if (handle != null)
            handle.SetActive(true);

        if (handleInteractObj != null)
            handleInteractObj.SetActive(true);
    }

    public void Disable()
    {
        if (rb == null || mr == null)
            return;

        rb.constraints = RigidbodyConstraints.FreezeAll;

        mr.enabled = false;
        if (handle != null)
            handle.SetActive(false);

        if (handleInteractObj != null)
            handleInteractObj.SetActive(false);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("CogWheel"))
        {
            if (notFixed)
            {
                notFixed = false;
                Enable();

                col.GetComponent<SVGrabbable>()?.ClearActiveController();
                Destroy(col.gameObject);
                pdm?.OnRobotFixed();
            }
        }
    }
}
