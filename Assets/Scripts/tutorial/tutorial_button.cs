using UnityEngine;
using UnityEngine.InputSystem;

public class tutorial_button : interactable_object
{
    [SerializeField] private GameObject _off;
    [SerializeField] private GameObject _on;
    [SerializeField] private observable_value_collection _obvc;
    [SerializeField] private wagon_storage _wagon;
    [SerializeField] private garage_door _garageDoor;

    public void OnInteractTrigger(interactor context)
    {
        if(_wagon.IsFull())
        {
            _garageDoor.StartMoving();
            _obvc.InvokeBool("isActive",true);
            _off.SetActive(false);
            _on.SetActive(true);
            if(context.TryGetComponent<PlayerInput>(out var res))
            {
                res.actions["interact"].canceled += HandleCancel;
            }
        }
    }

    public void HandleCancel(InputAction.CallbackContext context)
    {
        _garageDoor.StopMoving();
        _obvc.InvokeBool("isActive",false);
        _off.SetActive(true);
        _on.SetActive(false);
        context.action.canceled-=HandleCancel;
    }
    void Start()
    {
        _off.SetActive(true);
        _on.SetActive(false);
        _obvc.AddObservableBool("isActive");
    }
    public observable_value_collection GetOBVC(){return _obvc;}

    protected override string GetInteractionTextOverride(interactor context)
    {
        if(!_wagon.IsFull()){return "Fill Wagon First!";} else return DefaultInteractionText;
    }
}
