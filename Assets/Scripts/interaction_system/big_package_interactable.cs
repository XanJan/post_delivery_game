using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class big_package_interactable : interactable_object
{
    [SerializeField] private Transform _point1;
    [SerializeField] private Transform _point2;
    [SerializeField] private Transform _point3;
    [SerializeField] private Transform _point4; 
    [SerializeField] private float _pickupWeight = 0;
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private Rigidbody _rb;
    private GameObject[] _spots = new GameObject[4];
    [SerializeField]private Vector3[] _colliderSizes = new Vector3[] { new Vector3(1,1,1),new Vector3(1,2,2),new Vector3(1,2,3),new Vector3(2,2,3),new Vector3(3,2,3)};
    [SerializeField]private Vector3[] _colliderOffsets = new Vector3[] {new Vector3(0,0,0),new Vector3(0,0,-0.5f),new Vector3(0,0,0),new Vector3(-0.5f,0,0),new Vector3(0,0,0)};
    private Dictionary<GameObject,Vector2> _playerMovementInputs = new Dictionary<GameObject, Vector2>();
    protected override void OnInteractStart(interactor context)
    {
        // Add movement
        _playerMovementInputs.Add(context.gameObject,Vector2.zero);

        // Find player hands
        Transform playerHands = null;
        foreach (Transform child in context.transform)
        {
            if (child.CompareTag("PlayerHands"))
            {
                playerHands = child;
                break;  // Exit loop once we found our hands
            }
        }
        _collider.size = _colliderSizes[_activeInteractors.Count];
        _collider.center =_colliderOffsets[_activeInteractors.Count];
        // First player, packet to player
        if(_activeInteractors.Count == 1)
        {
            transform.rotation = context.transform.rotation;
            if(playerHands!=null){transform.position = playerHands.position - (_point1.position - transform.position);}
            else{transform.position = context.transform.position;}
            _spots[0] = context.gameObject;
        }
        else if(_spots[0]==null)
        {
            // Second  player, player to  packet
            context.transform.eulerAngles = transform.eulerAngles;
            if(playerHands!=null){context.transform.position = transform.position + (_point1.position - transform.position) - (playerHands.position-context.transform.position);}
            else{context.transform.position = transform.position;}
            _spots[0] = context.gameObject;
        }
        else if(_spots[1]==null)
        {
            // Second  player, player to  packet
            context.transform.eulerAngles = transform.eulerAngles + 180f * Vector3.up;
            if(playerHands!=null){context.transform.position = transform.position + (_point2.position - transform.position) - (playerHands.position-context.transform.position);}
            else{context.transform.position = transform.position;}
            _spots[1] = context.gameObject;
        }
         else if(_spots[2]==null)
        {
            // Third  player, player to  packet
            context.transform.eulerAngles = transform.eulerAngles + 90f * Vector3.up;
            if(playerHands!=null){context.transform.position = transform.position + (_point3.position - transform.position) - (playerHands.position-context.transform.position);}
            else{context.transform.position = transform.position;}
            _spots[2] = context.gameObject;
        }
        else if(_spots[3]==null)
        {
            // Third  player, player to  packet
            context.transform.eulerAngles = transform.eulerAngles - 90f * Vector3.up;
            if(playerHands!=null){context.transform.position = transform.position + (_point4.position - transform.position) - (playerHands.position-context.transform.position);}
            else{context.transform.position = transform.position;}
            _spots[3] = context.gameObject;
        } 
        // Set constraints
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Set parent to player
        context.transform.parent = transform;

        // Set ignore collisions
        Collider[] colls = context.GetComponents<Collider>();
        if(colls!=null)
        {
            foreach(Collider col in colls)
            {
                Physics.IgnoreCollision(col, _collider,true);
            }
        }
        // Set player rb to iskinematic
        if(context.TryGetComponent<Rigidbody>(out var playerRb))
        {
            playerRb.isKinematic = true;
        }
        // Invoke player holding package bool to true, set movespeed.
        observable_value_collection obvc = context.GetPlayerObservableValueCollection();
        if(obvc!=null)
        {
            try
            {
                obvc.InvokeBool("holdingPackage",true);
                obvc.InvokeFloat("moveSpeedMultiplierPickup",1/(_pickupWeight+1)); // Slow down player relative to weight
            } catch{} // Do nothing on exception
        }
        // Disable player movement
        player_movement playerMovement = context.GetComponent<player_movement>();
        if(playerMovement != null){ 
            playerMovement.enabled = false;
        }
        // Enable package movement
        if(context.TryGetComponent<PlayerInput>(out var res))
        {
            res.actions["move"].performed += cxt => HandleMovementPerformed(context.gameObject,cxt.ReadValue<Vector2>());
            res.actions["move"].canceled += cxt => HandleMovementCancled(context.gameObject);
        }
    }

    protected override void OnInteractEnd(interactor context)
    {
        // Update collider size
        _collider.size = _colliderSizes[_activeInteractors.Count];
        _collider.center =_colliderOffsets[_activeInteractors.Count];
        // Remove input
        _playerMovementInputs.Remove(context.gameObject);
        // Set player parent to null and enable DontDestroyOnLoad again
        context.transform.SetParent(null);
        DontDestroyOnLoad(context);
        // Enable collisions
        Collider[] colls = context.GetComponents<Collider>();
        if(colls!=null)
        {
            foreach(Collider col in colls)
            {
                Physics.IgnoreCollision(_collider,col,false);
            }
        }
        // Set player rb to not be kinematic anymore
        if(context.TryGetComponent<Rigidbody>(out var playerRb))
        {
            playerRb.isKinematic = false;
        }
        // If it was the last player to exit, reset constraints.
        if(_activeInteractors.Count == 0)
        {
            _rb.constraints = RigidbodyConstraints.None;  
        }
        // Invoke player holding package bool to true, reset movespeed.
        observable_value_collection obvc = context.GetPlayerObservableValueCollection();
        if(obvc!=null)
        {
            try
            {
                obvc.InvokeBool("holdingPackage",false); 
                obvc.InvokeFloat("moveSpeedMultiplierPickup",1); // reset movespeed
            } catch{} // Do nothing on exception
            
        }
        // Enable player movement
        player_movement playerMovement = context.GetComponent<player_movement>();
        if(playerMovement != null){ 
            playerMovement.enabled = true;
        }
        // Reset spot
        for(int i = 0 ; i < 4 ; i++)
        {
            if(_spots[i]==context.gameObject){_spots[i]=null;}
        }
    }
    void FixedUpdate()
    {
        if(_playerMovementInputs.Count>0)
        {
            Vector2 acc = Vector2.zero;
            foreach(KeyValuePair<GameObject,Vector2> kvp in _playerMovementInputs)
            {
                
                if(kvp.Key.TryGetComponent<observable_value_collection>(out var res))
                {
                    try{
                        acc += kvp.Value * res.GetObservableFloat("moveSpeedBase").Value * 
                            res.GetObservableFloat("moveSpeedMultiplierPickup").Value * 
                                res.GetObservableFloat("moveSpeedMultiplierEnvironment").Value * 
                                    res.GetObservableFloat("moveSpeedMultiplierOther").Value;
                    }catch{acc += kvp.Value;}
                    
                } else acc += kvp.Value; 
            }
            Vector3 mov = new Vector3(acc.x,0,acc.y);
            _rb.MovePosition(_rb.position + (mov * Time.fixedDeltaTime));
            // Freeze position when not moving. (to prevent unwanted movement applied by mysterious forces)
            if(_activeInteractors.Count>0 && mov.magnitude==0)
            {
                _rb.constraints = RigidbodyConstraints.FreezeAll & ~RigidbodyConstraints.FreezePositionY;
            } 
            else // Players are interacting, and moving
            { 
                _rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
    }

    public void HandleMovementPerformed(GameObject player, Vector2 movement)
    {
        if(_playerMovementInputs.ContainsKey(player)){_playerMovementInputs[player] = movement.normalized;}
    }
    public void HandleMovementCancled(GameObject player)
    {
        if(_playerMovementInputs.ContainsKey(player)){_playerMovementInputs[player] = Vector2.zero;}
    }
}
