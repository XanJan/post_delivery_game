using System;
using Unity.VisualScripting;
using UnityEngine;

public class item_script : MonoBehaviour
{
    //public bool isPickedUp = false;
    private Boolean isGrounded;
    private LayerMask groundLayer;

    public bool isPickedUp     //The Property 
    {
        get;
        set;
    
    }

    private void Start()
    {
        groundLayer = LayerMask.GetMask("Ground");
    }

    private void Update()
    {
        CheckIfGrounded();
        if (isGrounded)
        {
            isPickedUp = false;
        }

    }



    private void CheckIfGrounded()
    {
        // Check if the player is touching the ground using a raycast or overlap
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.5f, groundLayer);
    }
}
