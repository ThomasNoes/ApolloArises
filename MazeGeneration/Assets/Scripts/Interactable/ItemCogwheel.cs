using UnityEngine;

[RequireComponent(typeof(SVGrabbable), typeof(Rigidbody))]
public class ItemCogwheel : MonoBehaviour
{
    private SVGrabbable grabbable;
    private Rigidbody rb;
    private bool isGrabbed;

    void Start()
    {
        grabbable = GetComponent<SVGrabbable>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
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
}
