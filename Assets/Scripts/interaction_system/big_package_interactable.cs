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
    [SerializeField] private Collider _collider;
    [SerializeField] private Rigidbody _rb;
    private GameObject[] _spots = new GameObject[4];
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
        // Set parent to player
        context.transform.parent = transform;

        // Set package stuff (unchanged if already picked up)
        _rb.isKinematic = true;
        _collider.enabled = false;

        // Set player kinematic.
        if(context.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
        }
        
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
        // Update 
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
