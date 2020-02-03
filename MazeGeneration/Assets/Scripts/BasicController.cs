using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicController : MonoBehaviour
{
    CharacterController cr;
    // Start is called before the first frame update
    void Start()
    {
        cr = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        var forward = Vector3.Cross(transform.right, Vector3.up).normalized;
        var right = Vector3.Cross(transform.forward, Vector3.up).normalized;
        var movement = Vector3.zero;

        movement += Input.GetKey(KeyCode.W) ? forward : Vector3.zero;
        movement += Input.GetKey(KeyCode.A) ? right : Vector3.zero;
        movement += Input.GetKey(KeyCode.S) ? -forward : Vector3.zero;
        movement += Input.GetKey(KeyCode.D) ? -right : Vector3.zero;
        movement = movement.normalized;
        movement *= 0.01f;

        cr.Move(movement);
    }
}
