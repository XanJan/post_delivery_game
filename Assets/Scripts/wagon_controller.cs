using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class wagon_controller : MonoBehaviour{

    public Transform seatPosition;
    private List<GameObject> playersInside = new List<GameObject>();
    private Dictionary<GameObject, Vector2> playerInputs = new Dictionary<GameObject, Vector2>();

    private Rigidbody rb;
    public float acceleration = 5f; // wagon acceleration
    public float maxSpeed = 10f;    // wagon max speed
    public float turnSpeed = 100f;  // wagon turnspeed
    public float deceleration = 3f; // wagon deceleration when no input

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // add palyer to list when entering trigger
    private void OnTriggerEnter(Collider other){
       if(other.CompareTag("Player") && !playersInside.Contains(other.gameObject)){
        playersInside.Add(other.gameObject);
       } 
    }

    // when player leaves trigger, remove from list as well as their input
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player")){
            playersInside.Remove(other.gameObject);
            playerInputs.Remove(other.gameObject);
        }
    }

    // handle an interact call from a player, toggles either exit/ enter
    public void Interact(GameObject player){
        if(playerInputs.ContainsKey(player)){
            ExitWagon(player);
        }
        else{
            EnterWagon(player);
        }
    }

    // put a player in the wagon and pause its physics
    private void EnterWagon(GameObject player){
        player_movement playerMovement = player.GetComponent<player_movement>();
        if(playerMovement != null){ 
            playerMovement.enabled = false;
        }

        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if(playerRb != null){
            playerRb.useGravity = false;
        }

        Collider wagonCollider = GetComponent<Collider>();
        Collider playerCollider = player.GetComponent<Collider>();
        Physics.IgnoreCollision(playerCollider, wagonCollider, true);

        player.transform.SetParent(transform);
        int seatOffset = playersInside.Count - 1;
        player.transform.position = seatPosition.position - transform.forward * seatOffset;
        player.transform.rotation = seatPosition.rotation;

        playerInputs[player] = Vector2.zero;
        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        if(playerInput != null){
            playerInput.actions["Move"].performed += ctx => UpdateInput(player, ctx.ReadValue<Vector2>());
            playerInput.actions["Move"].canceled += ctx => UpdateInput(player, Vector2.zero);
        }
    }

    // remove a player from the wagon and reinstate its physics
    private void ExitWagon(GameObject player){
        if(!playerInputs.ContainsKey(player)){
            return;
        }

        player_movement playerMovement = player.GetComponent<player_movement>();
        if(playerMovement != null){
            playerMovement.enabled = true;
        }

        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if(playerRb != null){
            playerRb.useGravity = true;
        }

        Collider wagonCollider = GetComponent<Collider>();
        Collider playerCollider = player.GetComponent<Collider>();
        Physics.IgnoreCollision(playerCollider, wagonCollider, false);

        player.transform.SetParent(null);
        player.transform.position = transform.position - transform.forward * 2f;

        playerInputs.Remove(player);
    }

    // update the current input vector for each player
    private void UpdateInput(GameObject player, Vector2 input){
        if(playerInputs.ContainsKey(player)){
            if(input.magnitude < 0.1f){
                playerInputs[player] = Vector2.zero;
            }
            else{
                playerInputs[player] = input;
            }
        }
    }

    // handles physics updates for the wagon movement
    private void FixedUpdate(){
        // if no inputs, slow down
        if(playerInputs.Count == 0){
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * deceleration);
            UpdatePlayerPositions();
            return;
        }

        // sum active player inputs
        Vector2 combinedInput = Vector2.zero;
        foreach(var input in playerInputs.Values){
            if(input.magnitude > 0.1f){
                combinedInput += input;
            }
        }
        
        // invert input since it was inverted (((((((?????????)))))))
        combinedInput = -combinedInput;

        // if resulting input is negligible, decelerate
        if(combinedInput.magnitude < 0.1f){
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * deceleration);
            UpdatePlayerPositions();
            return;
        }

        float inputMagnitude = combinedInput.magnitude;
        combinedInput = Vector2.ClampMagnitude(combinedInput, 1f);

        // convert 2D input into normalized world-space direction
        Vector3 desiredDirection = new Vector3(combinedInput.x, 0, combinedInput.y).normalized;

        // calculate angle between wagons facing direction and desired moving-direction
        float angleDifference = Vector3.SignedAngle(transform.forward, desiredDirection, Vector3.up);

        // calculate rotation posible and ensure smoothness
        float maxTurnAngle = turnSpeed * Time.fixedDeltaTime;
        float clampedTurnAngle = Mathf.Clamp(angleDifference, -maxTurnAngle, maxTurnAngle);
        transform.Rotate(0, clampedTurnAngle, 0);

        // recalculate angle difference after rotation
        angleDifference = Vector3.SignedAngle(transform.forward, desiredDirection, Vector3.up);

        // calculate alignment factor (how far of from desired direction is wagon facing)
        float alignmentFactor = Mathf.Clamp01(1f - Mathf.Abs(angleDifference) / 90f);

        Vector3 targetVelocity = alignmentFactor * inputMagnitude * maxSpeed * -transform.forward;

        // smoothly interpolate current velocity towards target velocity
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * acceleration);

        UpdatePlayerPositions();
    } 

    // ensure that player models stays in the correct position during wagon movement
    private void UpdatePlayerPositions(){
        foreach (GameObject player in playerInputs.Keys){
            if(player != null){
                player.transform.position = seatPosition.position;
                player.transform.rotation = seatPosition.rotation;
            }
        }
    }
}