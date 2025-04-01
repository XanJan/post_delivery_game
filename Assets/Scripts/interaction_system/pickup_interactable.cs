using System;
using UnityEngine;

public class pickup_interactable : interactable_object
{
    [SerializeField]private float _throwForce = 2f;
    [SerializeField]private float _pickupWeight;
    public void HandleBegin(interactor context)
    {
        if(context.ActiveInteractionsCount()==1)// No other active interactions in context
        {
            Pickup(context);
        }else
        {
            context.CancelInteraction(this);
            context.TryEndInteraction();
        }
    }

    public void HandleEnd(interactor context)
    {
        Drop(context);
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
        obvc.InvokeBool("holdingPackage",true);
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
                // Apply player's velocity plus some upward force  
                float multiplier = 1;
                try
                {
                    if(obvc != null && obvc.GetObservableFloat("moveSpeed").Value > 0)
                    {
                        multiplier = _throwForce;
                    }
                } catch (Exception){}// Do nothing on exception
                
                Vector3 throwVelocity = player.transform.forward * multiplier + (playerRb.linearVelocity*playerRb.linearVelocity.magnitude) + Vector3.up * 3;
                itemRb.linearVelocity = throwVelocity;
                if(obvc!=null)
                {
                    try
                    {
                        obvc.InvokeFloat("moveSpeedMultiplierPickup",1/(_pickupWeight+1)); // Slow down player relative to weight
                    } catch(Exception){} // Do nothing on exception
                    
                }
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
