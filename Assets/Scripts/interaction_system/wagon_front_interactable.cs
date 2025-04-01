using UnityEngine;

public class wagon_front_interactable : interactable_object
{
    [SerializeField] private wagon_controller _wagon;
    [SerializeField] private Transform _interactionTextPosition;
    public Transform GetInteractionTextPosition(){if(_interactionTextPosition!=null)return _interactionTextPosition; else return transform;}
    public void OnInteractStart(interactor context)
    {
        _wagon.Interact(context.gameObject);
    }
    public void OnInteractEnd(interactor context)
    {
        _wagon.Interact(context.gameObject);
    }
}
