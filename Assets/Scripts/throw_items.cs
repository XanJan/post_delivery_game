using UnityEngine;
using UnityEngine.InputSystem;

public class throw_items : MonoBehaviour
{
    private PlayerInput playerInput;
    private player_movement playerMovement;
    private Vector2 targetPosition;
    private bool looking;
    private Camera cam;
    private float rotationSpeed = 700f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<player_movement>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();


    }

    private void OnEnable()
    {
        var controls = playerInput.actions;

        controls["Interact"].performed += InteractPressed;
        controls["Interact"].canceled += StopInteractPressed;

        controls["Look"].performed += LookAround;
    }


    void InteractPressed(InputAction.CallbackContext context)
    {
        playerMovement.enabled = false;
        looking = true;


    }
    void StopInteractPressed(InputAction.CallbackContext context)
    {
        playerMovement.enabled = true;
        looking = false;
    }

    void LookAround(InputAction.CallbackContext context)
    {
        if (looking)
        {
            targetPosition = context.ReadValue<Vector2>();
            
            
        }
    }

    private void Update()
    {
        if (looking)
        {
            Debug.Log(targetPosition);
            Vector3 moveDirection = new Vector3(targetPosition.x, 0f, targetPosition.y);

            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);

        }

    }
}

