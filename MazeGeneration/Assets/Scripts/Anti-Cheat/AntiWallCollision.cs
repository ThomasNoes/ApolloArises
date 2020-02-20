using UnityEngine;

public class AntiWallCollision : MonoBehaviour
{
    public bool useVibration = true;
    public bool turnScreenBlack = false;

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Wall"))
        {
            if (useVibration)
                OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.RTouch);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Wall"))
        {
            if (useVibration)
                OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        }
    }
}
