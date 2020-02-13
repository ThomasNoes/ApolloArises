﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class DebugMovement : MonoBehaviour
{
    CharacterController characterController;

    public float speed = 6.0f;
    public float rotSpeed = 50.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0, rotSpeed * Time.deltaTime, 0);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0, -rotSpeed * Time.deltaTime, 0);
        }

        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection *= speed;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
    }
}