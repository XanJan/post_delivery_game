using UnityEngine;

public class wagon_storage_interactable : interactable_object
{
    [SerializeField]private string _dropOffText = "Press %interactButton% to drop off";
    [SerializeField] private string _pickupText = "Press %interactButton% to pick up";
    [SerializeField]private string _wagonFullText = "Wagon is full!";
    [SerializeField]private string _wagonEmptyText = "Wagon is empty!";
    [SerializeField]private wagon_storage _wagon;
    public void PlayerIsHoldingPackage(bool b)
    {
        if(b && !_wagon.IsFull() ){InteractionText = _dropOffText;}
        else if(!b && !_wagon.IsEmpty()){InteractionText = _pickupText;}
    }

    public void HandleInteractTrigger(interactor context)
    {
        InteractionText = _defaultInteractionText;
        bool success = context.TryPopInteraction(out var activeInteractable);
        if(success)// Attempt drop off
        {
            context.PushInteraction(activeInteractable);
            if(!_wagon.IsFull())
            {
                InteractionText = _dropOffText;
                context.TryEndInteraction();
                _wagon.AddPackage(activeInteractable.gameObject);
            } 
            else
            {
                InteractionText = _wagonFullText;
            }
            activeInteractable.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");            
        } 
        else// Attempt pickup. Does nothing if wagon is empty 
        {
            GameObject package = _wagon.RemovePackage();
            if (package!=null)
            {
                InteractionText = _dropOffText;
                package.layer = LayerMask.NameToLayer("Ignore Raycast");
                if(package.TryGetComponent<interactable_object>(out var interactable))
                {
                    context.BeginInteraction(interactable);
                } else {Debug.Log("Warning : Tried to pick up a package that is not interactable. Attach an " +
                typeof(interactable_object).ToString()+" component to the package gameobject '" + package.name +"'.");}
            } else {InteractionText=_wagonEmptyText;}
        }
    }   
}
