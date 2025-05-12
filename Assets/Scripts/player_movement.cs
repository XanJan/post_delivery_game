using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class player_movement : MonoBehaviour
{
    [SerializeField] private observable_value_collection _obvc;
    private readonly string _movespeedName = "moveSpeed";
    private Rigidbody rb;
    private Vector2 moveInput;
    public float rotationSpeed = 700f;
    private PlayerInput playerInput;
    private LayerMask groundLayer;
    private Boolean isGrounded;
    private float jumpForce = 7f;

    private float _moveSpeedBase;
    private float _moveSpeedMultiplierPickup;
    private float _moveSpeedMultiplierEnvironment;
    private float _moveSpeedMultiplierOther;
    private bool _firstEnable=true;

    private void Awake()
    {   
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        groundLayer = LayerMask.GetMask("Ground");
    }
    public void OnUpdateMoveSpeedBase(observable_value<float> context){_moveSpeedBase = context.Value; }
    public void OnUpdateMoveSpeedMultiplierPickup(observable_value<float> context){_moveSpeedMultiplierPickup = context.Value;}
    public void OnUpdateMoveSpeedMultiplierEnvironment(observable_value<float> context){_moveSpeedMultiplierEnvironment = context.Value;}
    public void OnUpdateMoveSpeedMultiplierOther(observable_value<float> context){_moveSpeedMultiplierOther = context.Value;}


    private void Start()
    {
        if (_obvc != null) 
        { 
            _moveSpeedBase = _obvc.GetObservableFloat("moveSpeedBase").Value == 0 ? 5 : _obvc.GetObservableFloat("moveSpeedBase").Value;
            _moveSpeedMultiplierPickup = _obvc.GetObservableFloat("moveSpeedMultiplierPickup").Value == 0 ? 1 : _obvc.GetObservableFloat("moveSpeedMultiplierPickup").Value;
            _moveSpeedMultiplierEnvironment = _obvc.GetObservableFloat("moveSpeedMultiplierEnvironment").Value == 0 ? 1 : _obvc.GetObservableFloat("moveSpeedMultiplierEnvironment").Value;
            _moveSpeedMultiplierOther = _obvc.GetObservableFloat("moveSpeedMultiplierOther").Value == 0 ? 1 : _obvc.GetObservableFloat("moveSpeedMultiplierOther").Value;
            try{
            _obvc.GetObservableFloat("moveSpeedBase").UpdateValue += OnUpdateMoveSpeedBase;
            _obvc.GetObservableFloat("moveSpeedMultiplierPickup").UpdateValue += OnUpdateMoveSpeedMultiplierPickup;
            _obvc.GetObservableFloat("moveSpeedMultiplierEnvironment").UpdateValue += OnUpdateMoveSpeedMultiplierEnvironment;
            _obvc.GetObservableFloat("moveSpeedMultiplierOther").UpdateValue += OnUpdateMoveSpeedMultiplierOther;

            _obvc.GetObservableVector2("MovePerformed").UpdateValue+=Move;
            _obvc.GetObservableVector2("MoveCanceled").UpdateValue+=MoveCanceled;
            _obvc.GetObservableBool("JumpStarted").UpdateValue+=Jump;
            } catch(Exception){Debug.Log("Error in getting observable values in player_movement, there may be missing values in the player's observable value collection.");}
        }
    }
    void OnEnable()
    {
        if(_firstEnable)
        {
            _firstEnable=false;
        }
        else
        {
            _obvc.GetObservableFloat("moveSpeedBase").UpdateValue += OnUpdateMoveSpeedBase;
            _obvc.GetObservableFloat("moveSpeedMultiplierPickup").UpdateValue += OnUpdateMoveSpeedMultiplierPickup;
            _obvc.GetObservableFloat("moveSpeedMultiplierEnvironment").UpdateValue += OnUpdateMoveSpeedMultiplierEnvironment;
            _obvc.GetObservableFloat("moveSpeedMultiplierOther").UpdateValue += OnUpdateMoveSpeedMultiplierOther;

            _obvc.GetObservableVector2("MovePerformed").UpdateValue+=Move;
            _obvc.GetObservableVector2("MoveCanceled").UpdateValue+=MoveCanceled;
            _obvc.GetObservableBool("JumpStarted").UpdateValue+=Jump;
        }
        
    }
    void OnDisable()
    {
        _obvc.GetObservableFloat("moveSpeedBase").UpdateValue -= OnUpdateMoveSpeedBase;
        _obvc.GetObservableFloat("moveSpeedMultiplierPickup").UpdateValue -= OnUpdateMoveSpeedMultiplierPickup;
        _obvc.GetObservableFloat("moveSpeedMultiplierEnvironment").UpdateValue -= OnUpdateMoveSpeedMultiplierEnvironment;
        _obvc.GetObservableFloat("moveSpeedMultiplierOther").UpdateValue -= OnUpdateMoveSpeedMultiplierOther;

        _obvc.GetObservableVector2("MovePerformed").UpdateValue-=Move;
        _obvc.GetObservableVector2("MoveCanceled").UpdateValue-=MoveCanceled;
        _obvc.GetObservableBool("JumpStarted").UpdateValue-=Jump;
    }

    // Capture the move input when it's pressed
    private void Move(observable_value<Vector2> context)
    {
        if(_obvc!=null)
        {
            _obvc.InvokeFloat(_movespeedName,context.Value.magnitude);
                    
        }
         
        moveInput = context.Value;
         
    }

    // Reset the input when the move action is canceled (button release)
    private void MoveCanceled(observable_value<Vector2> context)
    {
        if(_obvc!=null){_obvc.InvokeFloat(_movespeedName,0f);}
         moveInput = Vector2.zero;
    }

    private void Jump(observable_value<bool> context)
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

        var moveVelocity = moveDirection * _moveSpeedBase * _moveSpeedMultiplierPickup * _moveSpeedMultiplierEnvironment * _moveSpeedMultiplierOther;
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

