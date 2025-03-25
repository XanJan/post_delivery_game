using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class wagon_controller : MonoBehaviour{

    public Transform seatPosition;
    private List<GameObject> playersInside = new List<GameObject>();
    private Dictionary<GameObject, Vector2> playerInputs = new Dictionary<GameObject, Vector2>();

    private Rigidbody rb;
    public float acceleration = 5f;
    public float maxSpeed = 10f;
    public float turnSpeed = 100f;
    public float deceleration = 3f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void OnTriggerEnter(Collider other){
       if(other.CompareTag("Player") && !playersInside.Contains(other.gameObject)){
        playersInside.Add(other.gameObject);
       } 
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player")){
            playersInside.Remove(other.gameObject);
            playerInputs.Remove(other.gameObject);
        }
    }

    public void Interact(GameObject player){
        if(playerInputs.ContainsKey(player)){
            ExitWagon(player);
        }
        else{
            EnterWagon(player);
        }
    }

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
        player.transform.position = seatPosition.position;
        player.transform.rotation = seatPosition.rotation;

        playerInputs[player] = Vector2.zero;
        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        if(playerInput != null){
            playerInput.actions["Move"].performed += ctx => UpdateInput(player, ctx.ReadValue<Vector2>());
            playerInput.actions["Move"].canceled += ctx => UpdateInput(player, Vector2.zero);
        }
    }

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
        player.transform.position = transform.position + transform.forward * 2f;

        playerInputs.Remove(player);
    }

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

    private void FixedUpdate(){
        if(playerInputs.Count == 0){
            return;
        }

        int playerCount = playerInputs.Count;
        float speedMultiplier = Mathf.Clamp(playerCount / 2f, 0.5f, 1f);

        float adjustedMaxSpeed = maxSpeed * speedMultiplier;
        float adjustedAcceleration = acceleration * speedMultiplier;

        Vector2 combinedInput = Vector2.zero;
        foreach(var input in playerInputs.Values){
            combinedInput += input;
        }
        combinedInput /= playerInputs.Count;

        float forwardInput = combinedInput.y;
        float turnInput = combinedInput.x;
        
        // apply acceleration or deceleration
        Vector3 forwardForce = transform.forward * -forwardInput * adjustedMaxSpeed;
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, forwardForce, Time.fixedDeltaTime * adjustedAcceleration);
        
        // apply turning
        if(Mathf.Abs(turnInput) > 0.1f){
            rb.angularVelocity = new Vector3(0, turnInput * turnSpeed * Mathf.Deg2Rad, 0);
        }

        // limit maxspeed
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);

        // slow down if no input
        if (Mathf.Abs(forwardInput) < 0.1f){
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * deceleration);
        }

        // move players with wagon
        foreach (GameObject player in playerInputs.Keys){
            if (player != null){
                player.transform.position = seatPosition.position; 
                player.transform.rotation = seatPosition.rotation; 
            }
        }

    } 
}