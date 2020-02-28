using UnityEngine;

public class AntiWallCollision : MonoBehaviour
{
    public GameObject cheatCube;
    public bool useVibration = true;
    public bool useVisual = false;
    private float farClippingPlane;
    private bool active = false;

    private void Start()
    {
        farClippingPlane = Camera.main.farClipPlane;
        Invoke("DelayedStart", 2.0f);
    }

    private void DelayedStart()
    {
        active = true;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Wall")
        {
            AntiCheat(true);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "Wall")
        {
            AntiCheat(false);
        }
    }

    private void AntiCheat(bool response) // true: activate, false: stop
    {
        if (!active)
            return;

        if (response)
        {
            if (useVibration)
                OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.RTouch);

            if (useVisual)
                cheatCube?.SetActive(true);
        }
        else
        {
            if (useVibration)
                OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);

            if (useVisual)
                cheatCube?.SetActive(false);
        }
    }
}
