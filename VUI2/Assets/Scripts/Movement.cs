using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour {

    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpSpeed = 10f;
    public float airSpeed = 1f;
    public float gravity = 20f;

    private CharacterController controller;

    private Vector3 moveDirection = Vector3.zero;

	// Use this for initialization
	void Start () {
        controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {

        UpdateMovementInput();
        
        if (controller.isGrounded)
        {
            GroundMove();
            
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }
        else
        {
            AirMove();
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
	}

    void GroundMove()
    {
        moveDirection *= Input.GetButton("Run") ? runSpeed : walkSpeed;
    }

    void AirMove()
    {
        moveDirection *= airSpeed;
    }

    void UpdateMovementInput()
    {
        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveDirection = transform.TransformDirection(moveDirection);
    }
}
