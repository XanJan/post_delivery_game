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

    
    // Stats
    [SerializeField] private string _playerIdValueName= "playerId";
    [SerializeField] private string _nrOfInteractionsValueName = "interactions";
    [SerializeField] private string _nrOfWagonInteractionsValueName = "steerWagonInteractions";
    [SerializeField] private string _nrOfPackagesPickedUp = "packagesPickedUp";

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
                if(TryInteractStart(interactable))
                {
                    stats_manager.Instance.IncIntStat("Player" + Obvc.GetObservableInt(_playerIdValueName).Value + " " + _nrOfInteractionsValueName,1);
                    switch(interactable)
                    {
                        case wagon_front_interactable:
                        stats_manager.Instance.IncIntStat("Player" + Obvc.GetObservableInt(_playerIdValueName).Value + " " +_nrOfWagonInteractionsValueName,1);
                        break;
                        case pickup_interactable:
                        stats_manager.Instance.IncIntStat("Player" + Obvc.GetObservableInt(_playerIdValueName).Value + " " +_nrOfPackagesPickedUp,1);
                        break;
                    }
                } else TryInteractEnd();    
            } else TryInteractEnd();
        } else TryInteractEnd();
    }

    // Handle text display...
    void Update()
    {
        // Do nothing if no textmesh is initialized.
        if(_textMeshPro==null){return;}  

        Ray ray = new Ray(_rayCastFrom.position, _rayCastFrom.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, _interactRange))
        {
            if(hit.collider.TryGetComponent<interactable_object>(out var interactable) && interactable.AllowInteractions)
            {
                _textMeshPro.transform.SetParent(null);
                _textMeshPro.gameObject.SetActive(true);
                _textMeshPro.gameObject.transform.position = interactable.gameObject.transform.position + Vector3.up;
                _textMeshPro.transform.LookAt(Camera.main.transform); // Rotate text toward camera
                //Update the interaction text.
                switch(interactable)
                {
                    case wagon_front_interactable wagonFrontInteractable :
                    _textMeshPro.gameObject.transform.position = wagonFrontInteractable.GetInteractionTextPosition().position;
                    goto default;

                    default:
                    if(_activeInteractions.Count>=MaxInteractions && !interactable.IsTrigger())
                    {
                        _textMeshPro.text = "";
                    } else _textMeshPro.text = interactable.GetInteractionText(this); 
                    break;
                }
            }
            else { _textMeshPro.gameObject.SetActive(false); _textMeshPro.text = ""; }
        }
        else { _textMeshPro.gameObject.SetActive(false); _textMeshPro.text = ""; }
    }
}