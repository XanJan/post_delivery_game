using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class player_interactor : interactor
{
    [SerializeField]private Transform _rayCastFrom;
    [SerializeField]private PlayerInput _playerInput;
    [SerializeField]private float _interactRange;
    [SerializeField]private TextMeshPro _textMeshPro;

    void OnEnable()
    {
        _playerInput.actions["interact"].started += HandleInteract;
    }

    void OnDisable()
    {
        _playerInput.actions["interact"].started -= HandleInteract;
    }

    private void HandleInteract(InputAction.CallbackContext context)
    {
        Ray ray = new Ray(_rayCastFrom.position, _rayCastFrom.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, _interactRange))
        {
            if(hit.collider.TryGetComponent<interactable_object>(out var interactable))
            {
                BeginInteraction(interactable);
            }
            else
            {
                TryEndInteraction();
            }
        } 
        else
        {
            TryEndInteraction();
        }
    }

    // Handle text display...
    void Update()
    {
        // Do nothing if no textmesh is initialized.
        if(_textMeshPro==null){return;}  

        Ray ray = new Ray(_rayCastFrom.position, _rayCastFrom.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, _interactRange))
        {
            if(hit.collider.TryGetComponent<interactable_object>(out var interactable))
            {
                _textMeshPro.transform.SetParent(null);
                _textMeshPro.gameObject.SetActive(true);
                //Update the interaction text.
                switch(interactable)
                {
                    case wagon_storage_interactable wagonInteractable:
                    if(_activeInteractions.Count>0)//Holding package
                    {
                        _textMeshPro.text = wagonInteractable.DropOffText;
                        break;
                    } else {goto default;}
                    case pickup_interactable:
                    if(_activeInteractions.Count>0){ _textMeshPro.text = ""; } else{goto default;}
                    break;
                    default:
                    _textMeshPro.text = interactable.GetInteractionText(); 
                    break;
                }
                
                _textMeshPro.gameObject.transform.position = interactable.gameObject.transform.position + Vector3.up;
                _textMeshPro.transform.LookAt(Camera.main.transform); // Rotate text toward camera
            }
            else { _textMeshPro.gameObject.SetActive(false); _textMeshPro.text = ""; }
        }
        else { _textMeshPro.gameObject.SetActive(false); _textMeshPro.text = ""; }
    }
}