using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class pickup_interactable : interactable_object
{
    [SerializeField]private float _baseThrowForce = 2f;
    [SerializeField]private float _pickupWeight;
    private float _throwForce;
    private bool _inThrowSequence=false;
    private float _throwTimer;
    private float _resetTimer;
    private List<interactor> _activeThrowingInteractors = new List<interactor>();
    private List<interactor> _waitingToBeCanceled = new List<interactor>();
    void Awake()
    {
        _throwForce = _baseThrowForce;
    }
    protected override void OnInteractStart(interactor context)
    {
        
        
        Pickup(context);
    }

    protected override void OnInteractEnd(interactor context)
    {

        _activeThrowingInteractors.Add(context);
        // Start timer
        _inThrowSequence = true;
        // Add Release E callback that triggers OnRelease
        context.GetPlayerObservableValueCollection().GetObservableBool("InteractCanceled").UpdateValue+=HandleInteractCancel;
        AllowInteractions=false;
        
        // OnRelease: If timer >= 0.5seconds, multiply throw force by 4.(And trigger animation) ; Drop
        
    }

    protected override void OnForceInteractEnd(interactor context, interactable_object source)
    {
        Drop(context);
    }

    public void HandleInteractCancel(observable_value<bool> obv)
    {


        if(_throwTimer >= 0.25)
        {
            _throwForce = _baseThrowForce * 3f;
            foreach(interactor context in _activeThrowingInteractors)
            {
                context.GetPlayerObservableValueCollection().InvokeBool("throwPackageAction",true);
            }
        } else {_throwForce = _baseThrowForce;}
        foreach(interactor context in _activeThrowingInteractors)
        {
            Drop(context);
            obv.UpdateValue -= HandleInteractCancel;
        }
        _waitingToBeCanceled = new List<interactor>(_activeThrowingInteractors);
        _activeThrowingInteractors = new List<interactor>();
        _throwTimer = 0;
        AllowInteractions=true;
        _inThrowSequence = false;
        
    }

    void FixedUpdate()
    {
        if(_inThrowSequence)
        {
            foreach(interactor context in _activeThrowingInteractors)
            {
                if(_throwTimer >= 0.25)
                {
                    context.GetPlayerObservableValueCollection().InvokeBool("throwPackageHold",true);
                    
                }
                
            }
            _throwTimer+=Time.fixedDeltaTime;

        } 
        else
        {
            if(_waitingToBeCanceled.Count>0)
            {
                if(_throwTimer <= 0.2)
                {
                    _throwTimer+=Time.fixedDeltaTime;
                } else
                {
                    foreach(interactor context in _waitingToBeCanceled)
                    {
                        context.GetPlayerObservableValueCollection().InvokeBool("throwPackageAction",false);
                        context.GetPlayerObservableValueCollection().InvokeBool("throwPackageHold",false);
                    }
                    _waitingToBeCanceled.Clear();
                    _throwTimer = 0;
                }
            }
            
        }
    }

    private void Pickup(interactor context)
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        GameObject player = context.gameObject;        
        
        Transform playerHands = null;
        foreach (Transform child in player.transform)
        {
            if (child.CompareTag("PlayerHands"))
            {
                playerHands = child;
                break;  // Exit loop once we found our hands
            }
        }
        if(playerHands!=null)
        {
            transform.parent = playerHands;
            transform.localPosition = Vector3.zero;
        } else 
        {
            transform.parent = player.transform;
            transform.localPosition = Vector3.zero;
        }

        // Disable physics if rigidbody is present
        if (TryGetComponent<Rigidbody>(out var itemRb))
        {
            itemRb.isKinematic = true;
        }
        if (TryGetComponent<Collider>(out var collider))
        {
            collider.enabled = false;
        }
        
        // Invoke player holding package bool to true;
        observable_value_collection obvc = context.GetPlayerObservableValueCollection();
        if(obvc!=null)
        {
            try
            {
                obvc.InvokeBool("holdingPackage",true);
                obvc.InvokeFloat("moveSpeedMultiplierPickup",1/(_pickupWeight+1)); // Slow down player relative to weight
            } catch(Exception){} // Do nothing on exception
            
        }
    }


    private void Drop(interactor context)
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        observable_value_collection obvc = context.GetPlayerObservableValueCollection();
        
        GameObject player = context.gameObject;
        // Unparent and re-enable physics
        transform.SetParent(null);
        if (TryGetComponent<Rigidbody>(out var itemRb))
        {
            itemRb.isKinematic = false;
            if(player.TryGetComponent<Rigidbody>(out var playerRb))
            {
                Vector3 throwVelocity = player.transform.forward * _throwForce + (playerRb.linearVelocity*playerRb.linearVelocity.magnitude) + Vector3.up * 3;
                itemRb.linearVelocity = throwVelocity;
            }
        }
        if (TryGetComponent<Collider>(out var collider))
        {
            collider.enabled = true;
        }
        if(obvc!=null)
        {
            try
            {
                obvc.InvokeFloat("moveSpeedMultiplierPickup", 1); 
                obvc.InvokeBool("holdingPackage",false);
            } catch (Exception) {} // Do nothing on exception
            
        }
    }
    
}

