using UnityEngine;

public class AntiWallCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag(Camera.main.tag))
        {
            OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.RTouch);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag(Camera.main.tag))
        {
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        }
    }
}
