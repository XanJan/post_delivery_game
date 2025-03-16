using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class player_movement : MonoBehaviour
{
    [SerializeField] private observable_value_collection _obvc;
    private readonly string _movespeedName = "moveSpeed";
    private Rigidbody rb;
    private Vector2 moveInput;
    [SerializeField] private float playerSpeed = 10f;
    public float rotationSpeed = 700f;
    private PlayerInput playerInput;
    private LayerMask groundLayer;
    private Boolean isGrounded;
    private float jumpForce = 7f;

    private void Awake()
    {
        // Makes sure observable float movespeed is obvc.
        if (_obvc != null) { _obvc.AddObservableFloat(_movespeedName); }

        
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
        if(_obvc!=null){_obvc.InvokeFloat(_movespeedName,context.ReadValue<Vector2>().magnitude);}
         
         moveInput = context.ReadValue<Vector2>();
         
    }

    // Reset the input when the move action is canceled (button release)
    private void MoveCanceled(InputAction.CallbackContext context)
    {
        if(_obvc!=null){_obvc.InvokeFloat(_movespeedName,0f);}
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
        CheckIfGrounded();
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
    private void CheckIfGrounded()
    {
        // Check if the player is touching the ground using a raycast or overlap
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.5f, groundLayer);
    }
}

