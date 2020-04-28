using UnityEngine;

[RequireComponent(typeof(OVRGrabbable), typeof(Rigidbody))]
public class ItemCogwheel : MonoBehaviour
{
    private OVRGrabbable ovrGrabbable;
    private Rigidbody rb;
    private bool isGrabbed;

    void Start()
    {
        ovrGrabbable = GetComponent<OVRGrabbable>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    void Update()
    {
        if (isGrabbed)
            return;

        if (ovrGrabbable.isGrabbed)
        {
            isGrabbed = true;
            rb.constraints = RigidbodyConstraints.None;
        }
    }
}
