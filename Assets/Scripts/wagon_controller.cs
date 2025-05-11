using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class wagon_controller : MonoBehaviour{

    public Transform seatPosition;
    private List<GameObject> playersInside = new List<GameObject>();
    private Dictionary<GameObject, Vector2> playerInputs = new Dictionary<GameObject, Vector2>();
    private GameObject[] _spots = new GameObject[4];
    private Rigidbody rb;
    public float acceleration = 5f; // wagon acceleration
    public float maxSpeed = 10f;    // wagon max speed
    public float turnSpeed = 100f;  // wagon turnspeed
    public float deceleration = 3f; // wagon deceleration when no input

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        for(int i = 0; i < _spots.Length ; i++) _spots[i]=null;
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
            playerRb.constraints |= RigidbodyConstraints.FreezePositionY;
            playerRb.isKinematic = true;
        }

        Collider wagonCollider = GetComponent<Collider>();
        Collider playerCollider = player.GetComponent<Collider>();
        Physics.IgnoreCollision(playerCollider, wagonCollider, true);

        player.transform.SetParent(transform);
        
        int seatOffset = 0;
        for(int i = 0 ; i < _spots.Length ; i++) {
            Debug.Log("beforeIf");
            if(_spots[i] == null)
            {
                Debug.Log("FoundSpot");
                _spots[i]=player;
                seatOffset=i;
                Debug.Log("Offset = " + i);
                break;
                
            }
        }
        player.transform.localPosition = seatPosition.localPosition - new Vector3(0,0,seatOffset/2f);
        player.transform.rotation = seatPosition.rotation;

        playerInputs[player] = Vector2.zero;

        
        observable_value_collection playerObvc = player.GetComponent<observable_value_collection>();
        if(playerObvc != null){
            playerObvc.GetObservableVector2("MovePerformed").UpdateValue += ctx => UpdateInput(player, ctx.Value);
            playerObvc.GetObservableVector2("MoveCanceled").UpdateValue += ctx => UpdateInput(player, Vector2.zero);
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
            playerRb.constraints &= ~RigidbodyConstraints.FreezePositionY;
            playerRb.isKinematic = false;
        }

        Collider wagonCollider = GetComponent<Collider>();
        Collider playerCollider = player.GetComponent<Collider>();
        Physics.IgnoreCollision(playerCollider, wagonCollider, false);

        player.transform.SetParent(null);
        for(int i = 0 ; i < _spots.Length ; i++) {if(_spots[i] == player)_spots[i]=null;}

        player.transform.position = transform.position - transform.forward * 2f;

        playerInputs.Remove(player);
    }

    public bool IsPlayerInWagon(GameObject player){
        return playersInside.Contains(player);
    }

    // update the current input vector for each player
    private void UpdateInput(GameObject player, Vector2 input){
        if(playerInputs.ContainsKey(player)){
            if(input.magnitude < 0.1f){
                playerInputs[player] = Vector2.zero;
                if(player.TryGetComponent<observable_value_collection>(out var obvc)){obvc.InvokeFloat("moveSpeed",0f);}
            }
            else{
                playerInputs[player] = input;
                if(player.TryGetComponent<observable_value_collection>(out var obvc)){obvc.InvokeFloat("moveSpeed",input.magnitude);}
            }
        }
    }

    // handles physics updates for the wagon movement
    private void FixedUpdate(){
        // if no inputs, slow down
        if(playerInputs.Count == 0){
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime * deceleration);
            //UpdatePlayerPositions();
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
            //UpdatePlayerPositions();
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

        int activePlayers = playerInputs.Values.Count(input => input.sqrMagnitude > 0); 

        float playerCountFactor = Mathf.Lerp(0.5f, 1f, (activePlayers - 1) / 3f);
        playerCountFactor = Mathf.Clamp(playerCountFactor, 0.5f, 1f);
        
        Vector3 targetVelocity = alignmentFactor * inputMagnitude * maxSpeed * playerCountFactor * -transform.forward;

        // smoothly interpolate current velocity towards target velocity
        //rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * acceleration);

        Vector3 newVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * acceleration);
        newVelocity += rb.linearVelocity * 0.1f;

        if (newVelocity.magnitude > maxSpeed){
                            newVelocity = newVelocity.normalized * maxSpeed;
            }

        rb.linearVelocity = newVelocity;

        //UpdatePlayerPositions();
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