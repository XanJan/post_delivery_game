using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class player_movement : MonoBehaviour
{
    //private InputSystem_Actions action;
    private Rigidbody rb;
    private Vector2 moveInput;
    private float playerSpeed = 10f;
    public float rotationSpeed = 700f;
    private PlayerInput playerInput;

    private LayerMask groundLayer;
    private Boolean isGrounded;
    private float jumpForce = 7f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        groundLayer = LayerMask.GetMask("Ground");
    }
    private void OnEnable()
    {
        
        var controls = playerInput.actions;
    
        controls["Move"].performed += Move;
        controls["Move"].canceled += MoveCanceled;
        controls["Jump"].started += Jump;
    }

     // Capture the move input when it's pressed
    private void Move(InputAction.CallbackContext context)
    {
         
         moveInput = context.ReadValue<Vector2>();
    }

    // Reset the input when the move action is canceled (button release)
    private void MoveCanceled(InputAction.CallbackContext context)
    {
         
         moveInput = Vector2.zero;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            Jump();
        }
    }


    private void FixedUpdate()
    {

        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

        // Normalize direction to prevent faster diagonal movement
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        var moveVelocity = moveDirection * playerSpeed;

        // Move the player using Rigidbody
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);

        // Rotate player based on movement direction
        if (moveVelocity.magnitude > 0.1f)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveVelocity, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        }

    }

    private void Jump()
    {
        // Apply upward force to the player Rigidbody
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false;
        }
    }
    private void CheckIfGrounded()
    {
        // Check if the player is touching the ground using a raycast or overlap
        //isGrounded = Physics.Raycast(transform.position, Vector3.down, groundLayer);
    }
}

