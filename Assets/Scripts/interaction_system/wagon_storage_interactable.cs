using UnityEngine;

public class wagon_storage_interactable : interactable_object
{
    [SerializeField]private string _dropOffText = "Press E to drop off";
    [SerializeField]private string _wagonFullText = "Wagon is full!";
    [SerializeField]private string _wagonEmptyText = "Wagon is empty!";
    [SerializeField]private wagon_storage _wagon;
    public string DropOffText;
    private string _ogInteractionText;
    void Start()
    {
        _ogInteractionText = _interactionText;
        DropOffText = _dropOffText;
    }

    public void HandleInteractTrigger(interactor context)
    {
        _interactionText = _ogInteractionText;
        bool success = context.TryPop(out var activeInteractable);
        if(success)// Attempt drop off
        {
            context.Push(activeInteractable);
            if(!_wagon.IsFull())
            {
                DropOffText = _dropOffText;
                context.TryEndInteraction();
                _wagon.AddPackage(activeInteractable.gameObject);
            } 
            else
            {
                DropOffText = _wagonFullText;
            }
            activeInteractable.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");            
        } 
        else// Attempt pickup. Does nothing if wagon is empty 
        {
            GameObject package = _wagon.RemovePackage();
            if (package!=null)
            {
                DropOffText = _dropOffText;
                package.layer = LayerMask.NameToLayer("Ignore Raycast");
                if(package.TryGetComponent<interactable_object>(out var interactable))
                {
                    context.BeginInteraction(interactable);
                } else {Debug.Log("Warning : Tried to pick up a package that is not interactable. Attach an " +
                typeof(interactable_object).ToString()+" component to the package gameobject '" + package.name +"'.");}
            } else {_interactionText=_wagonEmptyText;}
        }
    }   
}
