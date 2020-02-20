﻿using UnityEngine;

public class AntiWallCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Wall"))
        {
            OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.RTouch);
            Debug.Log("Wall Entered!");
        }
    }

    private void OnTriggerExit(Collider col)
    {
        Debug.Log("Wall Exited!");

        if (col.gameObject.CompareTag("Wall"))
        {
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        }
    }
}
