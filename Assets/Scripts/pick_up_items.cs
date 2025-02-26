using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class pick_up_items : MonoBehaviour
{
    private PlayerInput playerInput;
    private bool pickUp;
    private GameObject playerHands;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerHands = GetComponentInChildren<GameObject>();
    }

    private void OnEnable()
    {
        var controls = playerInput.actions;

        controls["Interact"].started += Interact;
        controls["Interact"].canceled += InteractCanceled;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == ("Item") && pickUp)
        {
            Vector3 playerPos = this.transform.position;
            other.transform.position = new Vector3(playerPos.x + 0.5f, playerPos.y, playerPos.z);
        }
    }
    private void Interact(InputAction.CallbackContext context)
    {
        pickUp = true;
    }
    private void InteractCanceled(InputAction.CallbackContext context)
    {
        pickUp = false;
    }
  
}
