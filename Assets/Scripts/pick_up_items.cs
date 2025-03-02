using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

//public class pick_up_items : MonoBehaviour
//{
//    private PlayerInput playerInput;
//    private bool pickUp;
//    private GameObject playerHands;
//    private bool isHolding = true;
//    private Rigidbody rb;
//    private bool inRange = false;
//    private float throwForce = 1f;
//    private void Awake()
//    {
//        playerInput = GetComponent<PlayerInput>();
//        rb = this.GetComponent<Rigidbody>();
//        Transform[] children = transform.GetComponentsInChildren<Transform>();
        
//        foreach (Transform child in children)
//        {
//            if(child.CompareTag("PlayerHands"))
//            {
//                playerHands = child.gameObject;
//                break;  // Exit loop once we found our hands
//            }
//        }
//    }

//    private void OnEnable()
//    {
//        var controls = playerInput.actions;

//        controls["Interact"].started += Interact;
//    }


//    private void OnTriggerStay(Collider other)
//    {
        
//        if(other.gameObject.tag == ("Item") && !pickUp)
//        {

//                Vector3 playerPos = this.transform.position;
//                other.transform.position = playerHands.transform.position;
//                other.GetComponent<BoxCollider>().enabled = false;
//                isHolding = true;
            
//        }
//        if(other.gameObject.tag == ("Item") && isHolding && pickUp)
//        {
//            other.GetComponent<BoxCollider>().enabled = true;
//            Vector3 throwVelocity = this.rb.linearVelocity;

//            //// Optionally add some upward force to prevent immediate dropping
//            throwVelocity += Vector3.up * 2f;

//            //// Apply the velocity
//            other.GetComponent<Rigidbody>().linearVelocity = throwVelocity * throwForce;
//        }
//    }
//    private void Interact(InputAction.CallbackContext context)
//    {
//        pickUp = !pickUp;
//    }


//}

using UnityEngine;
using UnityEngine.InputSystem;

public class pick_up_items : MonoBehaviour
{
    private float throwForce = 2f;

    private PlayerInput playerInput;
    private Rigidbody playerRb;

    private GameObject playerHands;
    private GameObject heldItem;

    private bool wantsToPickup = false; 
    private bool inRange = false;

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
        inRange = true;
        // If player wants to pickup and we're not holding anything, pickup item when it enters range
        if (wantsToPickup && heldItem == null && other.CompareTag("Item"))
        {
            PickupItem(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {

        // If player wants to pickup and we're not holding anything, pickup item while in range
        if (wantsToPickup && heldItem == null && other.CompareTag("Item"))
        {
            PickupItem(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        inRange = false;
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if(inRange)
        {
            wantsToPickup = !wantsToPickup;
        }
        
        

        // If we're holding an item and player toggles pickup off, drop it
        if (!wantsToPickup && heldItem != null)
        {
            DropItem();
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
