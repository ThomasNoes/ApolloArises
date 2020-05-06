using UnityEngine;

public class RigidBodyWaker : MonoBehaviour
{
    private Rigidbody rb;
    private bool isAlreadyKinematic, lateStart;
    private Vector3 startPos;
    private Quaternion startRot;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.sleepThreshold = 0.0f;
            isAlreadyKinematic = rb.isKinematic;
        }

        Invoke("LateStart", 0.1f);
    }

    private void LateStart()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        lateStart = true;
    }

    private void OnEnable()
    {
        if (rb == null)
            return;
        if (!isAlreadyKinematic)
            rb.isKinematic = false;

        rb.WakeUp();

        if (lateStart)
        {
            //    transform.position = startPos;
            transform.rotation = startRot;
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
