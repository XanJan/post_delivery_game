//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
//using static UnityEditor.Progress;


public class pick_up_items : MonoBehaviour
{
    private float throwForce = 2f;

    private PlayerInput playerInput;
    private Rigidbody playerRb;

    private GameObject playerHands;
    private GameObject heldItem;

    private bool wantsToPickup = false; 
    private bool inRange = false;
    private wagon_storage nearbyWagon;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRb = GetComponent<Rigidbody>();
        

        Transform[] children = transform.GetComponentsInChildren<Transform>();

        foreach (Transform child in children)
        {
            if (child.CompareTag("PlayerHands"))
            {
                playerHands = child.gameObject;
                break;  // Exit loop once we found our hands
            }
        }
    }


    private void OnEnable()
    {
        var controls = playerInput.actions;
        controls["Interact"].started += OnInteractPerformed;
    }

    private void OnDisable()
    {
        var controls = playerInput.actions;
        controls["Interact"].canceled -= OnInteractPerformed;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If player wants to pickup and we're not holding anything, pickup item when it enters range
        if (other.CompareTag("Package"))
        {
            inRange = true;
        }
        else if(other.CompareTag("Wagon"))
        {
            nearbyWagon = other.GetComponent<wagon_storage>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // If player wants to pickup and we're not holding anything, pickup item while in range
        if (wantsToPickup && heldItem == null && other.CompareTag("Package"))
        {
            PickupItem(other.gameObject);
        }
     }

    private void OnTriggerExit(Collider other)
    {
        inRange = false;
        wantsToPickup = false; 
        if (other.CompareTag("Wagon"))
        {
            nearbyWagon = null;
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    { 
        if(heldItem != null){
            DropItem();
        }
        else if(inRange){
            wantsToPickup = !wantsToPickup;
        }
        else if(nearbyWagon != null && !nearbyWagon.IsEmpty()){
            GameObject package = nearbyWagon.RemovePackage();
            if(package != null){
                PickupItem(package);
            }
        }
    }

    private void PickupItem(GameObject item)
    {
        heldItem = item;

        // Parent to hands and disable physics
        heldItem.transform.parent = playerHands.transform;
        heldItem.transform.localPosition = Vector3.zero;

        if (heldItem.TryGetComponent<Rigidbody>(out var itemRb))
        {
            itemRb.isKinematic = true;
        }

        if (heldItem.TryGetComponent<Collider>(out var collider))
        {
            collider.enabled = false;
        }
    }

    private void DropItem()
    {
        if (heldItem != null)
        {
            // Unparent and re-enable physics
            heldItem.transform.parent = null;

            if (heldItem.TryGetComponent<Rigidbody>(out var itemRb))
            {
                itemRb.isKinematic = false;

                // Apply player's velocity plus some upward force
                Vector3 throwVelocity = playerRb.linearVelocity + (Vector3.up * 2f);
                itemRb.linearVelocity = throwVelocity * throwForce;
            }

            if (heldItem.TryGetComponent<Collider>(out var collider))
            {
                collider.enabled = true;
            }

            heldItem = null;
        }
    }
}
