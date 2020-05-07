using UnityEngine;

public class RigidBodyWaker : MonoBehaviour
{
    private Rigidbody rb;
    private HingeJoint hj;
    private bool isAlreadyKinematic, lateStart, ranOnce;
    private Vector3 startPos, axis, anchor;
    private Quaternion startRot;

    private void Start()
    {
        if (ranOnce)
            return;

        ranOnce = true;

        rb = GetComponent<Rigidbody>();
        hj = GetComponent<HingeJoint>();

        if (rb != null)
        {
            rb.sleepThreshold = 0.0f;
            isAlreadyKinematic = rb.isKinematic;
        }

        Invoke("LateStart", 0.2f);
    }

    private void LateStart()
    {
        startPos = transform.position;
        startRot = transform.rotation;

        if (hj != null)
        {
            axis = hj.axis;
            anchor = hj.anchor;
        }

        lateStart = true;
    }

    private void OnEnable()
    {
        if (rb == null || !lateStart)
            return;
        if (!isAlreadyKinematic)
            rb.isKinematic = false;

        rb.WakeUp();

        if (hj != null)
        {
            hj.axis = axis;
            hj.anchor = anchor;
        }
    }

    private void OnDisable()
    {
        if (rb == null)
            return;

        if (!isAlreadyKinematic)
            rb.isKinematic = true;

        // rb.Sleep();
    }
}
