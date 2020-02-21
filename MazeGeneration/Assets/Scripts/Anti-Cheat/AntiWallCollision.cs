using UnityEngine;

public class AntiWallCollision : MonoBehaviour
{
    public GameObject cheatCube;
    public bool useVibration = true;
    public bool useVisual = false;
    private float farClippingPlane;

    private void Start()
    {
        Debug.Log("Anti cheat ready!");
        farClippingPlane = Camera.main.farClipPlane;
    }

    private void OnTriggerEnter(Collider col)
    {
        Debug.Log("Collided/entered with: " + col.name);
        if (col.tag == "Wall")
        {
            Debug.Log("Wall enter!");
            AntiCheat(true);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        Debug.Log("Collided/exited with: " + col.name);
        if (col.tag == "Wall")
        {
            Debug.Log("Wall exit!");
            AntiCheat(false);
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        Debug.Log("COLLISION!");
    }

    private void AntiCheat(bool response) // true: activate, false: stop
    {
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
