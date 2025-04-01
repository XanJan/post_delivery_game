using UnityEngine;

public class wagon_front_interactable : interactable_object
{
    [SerializeField] private wagon_controller _wagon;
    [SerializeField] private Transform _interactionTextPosition;
    [SerializeField] private string _holdingWagonString = "holdingWagon";
    public Transform GetInteractionTextPosition(){if(_interactionTextPosition!=null)return _interactionTextPosition; else return transform;}
    public void OnInteractStart(interactor context)
    {
        // Disable player collider
        if (context.TryGetComponent<Collider>(out var collider))
        {
            collider.enabled = false;
        }
        
        if(context.ActiveInteractionsCount() > 0)
        {
            context.TryPop(out var temp);
            context.TryEndInteraction();
            context.Push(temp);
        }
        // Update holdingWagon
        observable_value_collection obvc = context.GetPlayerObservableValueCollection();
        if(obvc!=null){obvc.InvokeBool(_holdingWagonString, true);}
        // Interact with wagon
        _wagon.Interact(context.gameObject);
    }
    public void OnInteractEnd(interactor context)
    {
        // Enable player collider
        if (context.TryGetComponent<Collider>(out var collider))
        {
            collider.enabled = true;
        }
        // Update holdingWagon
        observable_value_collection obvc = context.GetPlayerObservableValueCollection();
        if(obvc!=null){obvc.InvokeBool(_holdingWagonString, false);}
        // Interact with wagon
        _wagon.Interact(context.gameObject);
    }
}
