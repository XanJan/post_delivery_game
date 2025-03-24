using UnityEngine;
using UnityEngine.InputSystem;

public class throw_items : MonoBehaviour
{
    private PlayerInput playerInput;
    private player_movement playerMovement;
    private Vector2 mousePosition;
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
            mousePosition = context.ReadValue<Vector2>();
            
        }
    }

    private void Update()
    {
        if (looking)
        {
            Debug.Log(mousePosition);
            Vector3 mouseToWorld = cam.ScreenToWorldPoint(mousePosition);

            mouseToWorld.z = transform.position.z;
            Vector3 direction = mouseToWorld - transform.position;

            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            //transform.LookAt(new Vector3(mousePosition.x, this.transform.position.y, mousePosition.y), Vector3.up);
        }

    }
}

