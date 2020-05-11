using UnityEngine;

[RequireComponent(typeof(SVGrabbable), typeof(Rigidbody))]
public class ItemCogwheel : MonoBehaviour
{
    private SVGrabbable grabbable;
    private Rigidbody rb;
    private bool isGrabbed;
    private Vector3 startPos;

    void Start()
    {
        grabbable = GetComponent<SVGrabbable>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        startPos = transform.position;
    }

    void Update()
    {
        if (isGrabbed)
            return;

        if (grabbable.inHand)
        {
            isGrabbed = true;
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    void LateUpdate()
    {
        if (!isGrabbed && !grabbable.inHand)
            transform.position = startPos;
    }
}
