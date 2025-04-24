using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class big_package_interactable : interactable_object
{
    [SerializeField] private Transform _point1;
    [SerializeField] private Transform _point2;
    [SerializeField]private float _pickupWeight;
    [SerializeField] private Collider _collider;
    [SerializeField] private Rigidbody _rb;

    private Vector2 _movement;

    private Dictionary<GameObject,Vector2> _playerMovementInputs = new Dictionary<GameObject, Vector2>();

    protected override void OnInteractStart(interactor context)
    {
        
        _playerMovementInputs.Add(context.gameObject,Vector2.zero);
        Transform playerHands = null;
        foreach (Transform child in context.transform)
        {
            if (child.CompareTag("PlayerHands"))
            {
                playerHands = child;
                break;  // Exit loop once we found our hands
            }
        }
        

        // First player, packet to player
        if(_activeInteractors.Count == 1)
        {
            transform.rotation = context.transform.rotation;
            if(playerHands!=null){transform.position = playerHands.position - (_point1.position - transform.position);}
            else{transform.position = context.transform.position;}
        }
        else if(_activeInteractors.Count == 2)
        {
            // Second  player, player to  packet
            context.transform.eulerAngles = transform.eulerAngles + 180f * Vector3.up;
            if(playerHands!=null){context.transform.position = transform.position + (_point2.position - transform.position) - (playerHands.position-context.transform.position);}
            else{context.transform.position = transform.position;}
        }
        context.transform.parent = transform;

        _rb.isKinematic = true;
        _collider.enabled = false;
        if(context.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
        }
        
            //Physics.IgnoreCollision(collider,_collider, true);
        
        // Invoke player holding package bool to true;
        observable_value_collection obvc = context.GetPlayerObservableValueCollection();
        if(obvc!=null)
        {
            try
            {
                obvc.InvokeBool("holdingPackage",true);
                obvc.InvokeFloat("moveSpeedMultiplierPickup",1/(_pickupWeight+1)); // Slow down player relative to weight
            } catch{} // Do nothing on exception
            
        }

        player_movement playerMovement = context.GetComponent<player_movement>();
        if(playerMovement != null){ 
            playerMovement.enabled = false;
        }

        if(context.TryGetComponent<PlayerInput>(out var res))
        {
            
            res.actions["move"].performed += cxt => HandleMovementPerformed(context.gameObject,cxt.ReadValue<Vector2>());
            res.actions["move"].canceled += cxt => HandleMovementCancled(context.gameObject);
        }

    }

    protected override void OnInteractEnd(interactor context)
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        _playerMovementInputs.Remove(context.gameObject);
        context.transform.SetParent(null);
        DontDestroyOnLoad(context);
        if(_activeInteractors.Count == 0)
        {
            _rb.isKinematic = false;
            _collider.enabled = true;
        }
        
        
        if(context.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
        }
        observable_value_collection obvc = context.GetPlayerObservableValueCollection();
        if(obvc!=null)
        {
            try
            {
                obvc.InvokeBool("holdingPackage",false); 
                obvc.InvokeFloat("moveSpeedMultiplierPickup",1); // reset movespeed
            } catch{} // Do nothing on exception
            
        }

        player_movement playerMovement = context.GetComponent<player_movement>();
        if(playerMovement != null){ 
            playerMovement.enabled = true;
        }

        /* if(context.TryGetComponent<PlayerInput>(out var res))
        {
            res.actions["move"].performed -= cxt => HandleMovementPerformed(context.gameObject,cxt.ReadValue<Vector2>());
            res.actions["move"].canceled -= cxt => _playerMovementInputs[context.gameObject] = Vector2.zero;
        } */
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log(_playerMovementInputs.Count);
        if(_activeInteractors.Count>0)
        {
            Vector2 acc = Vector2.zero;
            foreach(KeyValuePair<GameObject,Vector2> kvp in _playerMovementInputs)
            {
                acc += kvp.Value;
            }
            Vector3 mov = new Vector3(acc.x,0,acc.y);
            _rb.MovePosition(_rb.position + mov * Time.fixedDeltaTime);
            Debug.Log(mov.x + " , " + mov.z);
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
