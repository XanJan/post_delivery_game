using System;
using UnityEngine;

public class wagon_storage_interactable : interactable_object
{
    [SerializeField]private string _dropOffText = "Press %interactButton% to drop off";
    [SerializeField] private string _pickupText = "Press %interactButton% to pick up";
    [SerializeField]private string _wagonFullText = "Wagon is full!";
    [SerializeField]private string _wagonEmptyText = "Wagon is empty!";
    [SerializeField]private wagon_storage _wagon;
    protected override string GetInteractionTextOverride(interactor context)
    {
        if(context.TryPeekInteraction(out interactable_object top))
        {
            switch(top)
            {
                case pickup_interactable:
                if(_wagon.IsFull()){return _wagonFullText;}
                else{return _dropOffText;}
                default: return "";
            }
        }
        else if(!_wagon.IsEmpty()){return _pickupText;}
        else return "";
    }

    public void HandleInteractTrigger(interactor context)
    {
        // Attempt drop off
        if(context.TryPeekInteraction(out interactable_object top))
        {
            if(!_wagon.IsFull())
            {
                switch(top){
                    case pickup_interactable:
                    if(context.TryForceInteractEnd(this,out interactable_object res))
                    {
                        _wagon.AddPackage(res.gameObject);
                    }
                    break;
                    default://nothing
                    break;
                }
            }
        }

        else// Attempt pickup. Does nothing if wagon is empty 
        {
            GameObject package = _wagon.RemovePackage();
            if (package!=null)
            {
                //package.layer = LayerMask.NameToLayer("Ignore Raycast");
                if(package.TryGetComponent<interactable_object>(out var interactable))
                {
                    context.TryInteractStart(interactable);
                } else {Debug.Log("Warning : Tried to pick up a package that is not interactable. Attach an " +
                typeof(interactable_object).ToString()+" component to the package gameobject '" + package.name +"'.");}
            }
        }
    }   
}
